using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Mappings;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Enums;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Infrastructure.Data;
using NewLifeHRT.Infrastructure.Repositories;
using NewLifeHRT.Infrastructure.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static NewLifeHRT.External.Models.EmpPostEasyRxModel;

namespace NewLifeHRT.Application.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IPasswordHasher<ApplicationUser> _passwordhasher;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserServiceLinkRepository _userServiceLinkRepository;
        private readonly ClinicDbContext _context;
        private readonly ILicenseInformationService _licenseInformationService;
        private readonly IBlobService _blobService;
        private readonly AzureBlobStorageSettings _azureBlobStorageSettings;


        public UserService(IUserRepository userRepository, IAddressRepository addressRepository, IPasswordHasher<ApplicationUser> passwordhasher, UserManager<ApplicationUser> userManager, IUserServiceLinkRepository userServiceLinkRepository, ClinicDbContext clinicDbContext, ILicenseInformationService licenseInformationService, IBlobService blobService, IOptions<AzureBlobStorageSettings> azureBlobStorageSettings)
        {
            _userRepository = userRepository;
            _addressRepository = addressRepository;
            _passwordhasher = passwordhasher;
            _userManager = userManager;
            _userServiceLinkRepository = userServiceLinkRepository;
            _context = clinicDbContext;
            _licenseInformationService = licenseInformationService;
            _blobService = blobService;
            _azureBlobStorageSettings = azureBlobStorageSettings.Value;
        }

        /// <summary>
        /// Creates a new user with related entities (address, services, licenses, and signature upload).
        /// </summary>
        /// <remarks>
        /// **Hardcoded / Calculation reasoning:**
        /// - Uses <see cref="DateTime.UtcNow"/> for consistent timestamp tracking across systems.
        /// - Generates a **unique timestamp-based filename** for the signature upload
        ///   (`yyyyMMddHHmmssfff`) to prevent file name collisions in Azure Blob Storage.
        /// 
        /// **Logical flow:**
        /// 1. Validate uniqueness of username/email.
        /// 2. Create user entity and hash password.
        /// 3. Handle signature upload (if present).
        /// 4. Create and link Address, Services, and License Information.
        /// </remarks>
        public async Task<CommonOperationResponseDto<int>> CreateAsync(CreateUserRequestDto createUserRequestDto, int userId)
        {
            if (await _userRepository.ExistAsync(createUserRequestDto.UserName, createUserRequestDto.Email))
                throw new Exception("Username or Email already exists.");

            var user = new ApplicationUser(createUserRequestDto.UserName, createUserRequestDto.FirstName, createUserRequestDto.LastName, createUserRequestDto.Email, createUserRequestDto.PhoneNumber,
                createUserRequestDto.Email.ToUpper(), createUserRequestDto.UserName.ToUpper(), createUserRequestDto.RoleId, createUserRequestDto.DEA, createUserRequestDto.NPI,
                createUserRequestDto.CommisionInPercentage, createUserRequestDto.MatchAsCommisionRate, createUserRequestDto.ReplaceCommisionRate, createUserRequestDto.IsVacationApplicable, createUserRequestDto.TimezoneId, createUserRequestDto.Color, createUserRequestDto.PatientId, createUserRequestDto.MustChangePassword, userId.ToString(), DateTime.UtcNow);
            Console.WriteLine($"user=> {createUserRequestDto.Email},\n password ==> {createUserRequestDto.Password}");

            // Password hashing for secure storage
            user.PasswordHash = _passwordhasher.HashPassword(user, createUserRequestDto.Password);

            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                var errorMessage = string.Join("; ", createResult.Errors.Select(e => e.Description));
                throw new Exception($"Failed to create user account. {errorMessage}");
            }

            // Handle signature upload with timestamp-based file name
            if (createUserRequestDto.SignatureFile != null)
            {
                string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
                string fileName = Path.GetFileName(createUserRequestDto.SignatureFile.FileName);
                user.SignaturePath = $"{user.Id}/{timestamp}_{fileName}";
                var res = await _blobService.UploadFileAsync(createUserRequestDto.SignatureFile, user.SignaturePath);
                await _userManager.UpdateAsync(user);
            }

            // Address creation for non-patient users
            if (createUserRequestDto.Address != null && createUserRequestDto.RoleId != (int)AppRoleEnum.Patient)
            {
                var address = new Domain.Entities.Address(createUserRequestDto.Address.AddressLine1, createUserRequestDto.Address.AddressType, createUserRequestDto.Address.City,
                    createUserRequestDto.Address.PostalCode, createUserRequestDto.Address.CountryId, createUserRequestDto.Address.StateId, userId.ToString(), DateTime.UtcNow, true);
                address = await _addressRepository.AddAsync(address);
                user.AddressId = address.Id;
                await _userManager.UpdateAsync(user);
            }

            // Link user to selected services
            if (createUserRequestDto.ServiceIds != null && createUserRequestDto.ServiceIds.Any())
            {
                foreach (var serviceId in createUserRequestDto.ServiceIds)
                {
                    var userService = new UserServiceLink(user.Id, serviceId, userId.ToString(), DateTime.UtcNow);
                    await _userServiceLinkRepository.AddAsync(userService);
                }
            }

            // Create license records for user
            if (createUserRequestDto.LicenseInformation != null && createUserRequestDto.LicenseInformation.Any())
            {
                await _licenseInformationService.CreateLicenseInformationAsync(createUserRequestDto.LicenseInformation.ToArray(), user.Id, userId);
            }
            return new CommonOperationResponseDto<int> { Id = user.Id, Message = "User created successfully" };
        }

        public async Task<List<UserResponseDto>> GetAllAsync(int? roleId)
        {
            var includes = new[] { "Address", "UserServices", "Address.Country" };

            Expression<Func<ApplicationUser, bool>> predicate = u =>
                (!roleId.HasValue || u.RoleId == roleId.Value);

            var predicates = new List<Expression<Func<ApplicationUser, bool>>> { predicate };

            var users = await _userRepository.FindWithIncludeAsync(predicates, includes);

            return users.ToUserResponseDtoList();
        }
        public async Task<List<DropDownIntResponseDto>> GetAllActiveUsersAsync(int roleId)
        {
            var users = await _userRepository.FindAsync(a => !a.IsDeleted && a.RoleId == roleId);
            return users.ToDropDownUserResponseDtoList();
        }

        /// <summary>
        /// Fetches detailed user info by ID, including linked Address, Services, and License Info.
        /// </summary>
        /// <remarks>
        /// **Helper method explanation:**
        /// - Uses `_azureBlobStorageSettings.ContainerSasUrl` and `SasToken` to construct
        ///   a **temporary public URL** for signature display from Azure Blob Storage.
        ///   This avoids directly exposing internal storage credentials.
        /// </remarks>
        public async Task<UserResponseDto?> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetWithIncludeAsync(
                id,
                new[] { "Address", "UserServices", "LicenseInformations", "LicenseInformations.State" });

            if (user == null) return null;
            if (!string.IsNullOrWhiteSpace(user.SignaturePath))
            {
                user.SignaturePath = $"{_azureBlobStorageSettings.ContainerSasUrl}/{user.SignaturePath}?{_azureBlobStorageSettings.SasToken}";
            }
            return user.ToUserResponseDto();
        }


        public async Task<CommonOperationResponseDto<int>> PermanentDeleteAsync(int id, int userId)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) throw new Exception("User not found");

            try
            {
                await _userRepository.DeleteAsync(user);
            }
            catch (Exception)
            {

                return new CommonOperationResponseDto<int>
                {
                    Message = $"failed to delete user, user linked to the patients",
                };
            }

            return new CommonOperationResponseDto<int> { Id = user.Id, Message = "User permanently deleted" };
        }


        /// <summary>
        /// Updates user and related entities (address, services, license info, and signature).
        /// </summary>
        /// <remarks>
        /// **Long method breakdown:**
        /// 1. Validates uniqueness for email/phone.
        /// 2. Updates core user fields (name, contact, role, commission, timezone, etc.).
        /// 3. Updates or creates Address (handles both Patient and non-Patient logic).
        /// 4. Handles signature upload with timestamp-based naming.
        /// 5. Synchronizes user-service relationships (add/remove as needed).
        /// 6. Updates license info through `ILicenseInformationService`.
        /// 
        /// **Hardcoded reasoning:**
        /// - Uses UTC timestamps for consistency.
        /// - Generates unique filenames for uploaded signatures.
        /// </remarks>
        public async Task<CommonOperationResponseDto<int>> UpdateAsync(int id, UpdateUserRequestDto updateUserRequestDto, int userId)
        {
            var user = await _userRepository.GetWithIncludeAsync(id, new string[] { "Address" });
            if (user == null) throw new Exception("User not found");

            var (emailExists, phoneExists) = await _userRepository.ExistAsync(updateUserRequestDto.PhoneNumber, updateUserRequestDto.Email, id);

            if (emailExists || phoneExists)
            {
                string message = emailExists && phoneExists
                    ? "Email and phone number already exist for another user." : emailExists
                        ? "Email already exists for another user." : "Phone number already exists for another user.";

                throw new Exception(message);
            }

            // Update core user fields
            user.UserName = updateUserRequestDto.UserName;
            user.FirstName = updateUserRequestDto.FirstName;
            user.LastName = updateUserRequestDto.LastName;
            user.Email = updateUserRequestDto.Email;
            user.NormalizedEmail = updateUserRequestDto.Email.ToUpper().ToString();
            user.PhoneNumber = updateUserRequestDto.PhoneNumber;
            user.RoleId = updateUserRequestDto.RoleId;
            user.DEA = updateUserRequestDto.DEA;
            user.NPI = updateUserRequestDto.NPI;
            user.CommisionInPercentage = updateUserRequestDto.CommisionInPercentage;
            user.MatchAsCommisionRate = updateUserRequestDto.MatchAsCommisionRate;
            user.ReplaceCommisionRate = updateUserRequestDto.ReplaceCommisionRate;
            user.Vacation = updateUserRequestDto.IsVacationApplicable;
            user.TimezoneId = updateUserRequestDto.TimezoneId;
            user.ColorCode = updateUserRequestDto.Color;
            user.UpdatedBy = userId.ToString();
            user.UpdatedAt = DateTime.UtcNow;

            // Address handling
            if (updateUserRequestDto.Address != null)
            {
                // Separate branch for Patient-linked users
                if (user.PatientId != null)
                {
                    if (user.Patient.Address == null)
                    {
                        var address = new Domain.Entities.Address();
                        address.CreatedBy = userId.ToString();
                        address.CreatedAt = DateTime.UtcNow;
                        user.Patient.Address = address;
                        address.IsActive = true;
                        await _addressRepository.AddAsync(address);
                    }

                    user.Patient.Address.AddressLine1 = updateUserRequestDto.Address.AddressLine1;
                    user.Patient.Address.City = updateUserRequestDto.Address.City;
                    user.Patient.Address.StateId = updateUserRequestDto.Address.StateId;
                    user.Patient.Address.PostalCode = updateUserRequestDto.Address.PostalCode;
                    user.Patient.Address.CountryId = updateUserRequestDto.Address.CountryId;
                    user.Patient.Address.UpdatedBy = userId.ToString();
                    user.Patient.Address.UpdatedAt = DateTime.UtcNow;

                    await _addressRepository.UpdateAsync(user.Patient.Address);
                }
                else
                {
                    if (user.Address == null)
                    {
                        var address = new Domain.Entities.Address();
                        address.CreatedBy = userId.ToString();
                        address.CreatedAt = DateTime.UtcNow;
                        user.Address = address;
                        address.IsActive = true;
                        await _addressRepository.AddAsync(address);
                    }

                    user.Address.AddressLine1 = updateUserRequestDto.Address.AddressLine1;
                    user.Address.City = updateUserRequestDto.Address.City;
                    user.Address.StateId = updateUserRequestDto.Address.StateId;
                    user.Address.PostalCode = updateUserRequestDto.Address.PostalCode;
                    user.Address.CountryId = updateUserRequestDto.Address.CountryId;
                    user.Address.UpdatedBy = userId.ToString();
                    user.Address.UpdatedAt = DateTime.UtcNow;

                    await _addressRepository.UpdateAsync(user.Address);
                }


            }

            // Handle updated signature
            if (updateUserRequestDto.SignatureFile != null)
            {
                string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
                string fileName = Path.GetFileName(updateUserRequestDto.SignatureFile.FileName);
                user.SignaturePath = $"{user.Id}/{timestamp}_{fileName}";
                var res = await _blobService.UploadFileAsync(updateUserRequestDto.SignatureFile, user.SignaturePath);
                await _userManager.UpdateAsync(user);
            }

            // Manage user-service relationships
            if (updateUserRequestDto.ServiceIds != null)
            {
                var existingUserServices = await _userServiceLinkRepository.GetByUserIdAsync(user.Id);
                var existingServiceIds = existingUserServices.Select(us => us.ServiceId).ToList();
                var newServiceIds = updateUserRequestDto.ServiceIds;

                var toRemove = existingUserServices.Where(us => !newServiceIds.Contains(us.ServiceId)).ToList();

                foreach (var userService in toRemove)
                {
                    await _userServiceLinkRepository.DeleteAsync(userService);
                }

                var toAdd = newServiceIds.Where(serviceId => !existingServiceIds.Contains(serviceId)).ToList();

                foreach (var serviceId in toAdd)
                {
                    var userServiceLink = new UserServiceLink
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        ServiceId = serviceId,
                        CreatedBy = userId.ToString(),
                        CreatedAt = DateTime.UtcNow
                    };
                    await _userServiceLinkRepository.AddAsync(userServiceLink);
                }
            }

            // License info update
            if (updateUserRequestDto.LicenseInformation != null && updateUserRequestDto.LicenseInformation.Any())
            {
                await _licenseInformationService.UpdateLicenseInformationAsync(updateUserRequestDto.LicenseInformation.ToArray(), user.Id, userId);
            }

            await _userManager.UpdateAsync(user);

            return new CommonOperationResponseDto<int> { Id = user.Id, Message = "User updated successfully" };
        }

        /// <summary>
        /// Performs bulk activation or deactivation of users.
        /// </summary>
        /// <remarks>
        /// **Calculation / Hardcoded logic:**
        /// - Uses `isActivating` flag to toggle `IsDeleted` status for multiple users.
        /// - Counts successful and failed operations to return a detailed summary.
        /// 
        /// **Helper role:**
        /// - This serves as a batch operation wrapper simplifying frontend calls for multi-user updates.
        /// </remarks>
        public async Task<BulkOperationResponseDto> BulkToggleUserStatusAsync(List<int> userIds, int userId, bool isActivating)
        {
            var successCount = 0;
            var failedCount = 0;
            var failedIds = new List<string>();

            try
            {
                foreach (var currentUserId in userIds)
                {
                    try
                    {
                        var user = await _userRepository.GetByIdAsync(currentUserId);
                        if (user != null)
                        {
                            user.IsDeleted = !isActivating;
                            user.UpdatedBy = userId.ToString();
                            user.UpdatedAt = DateTime.UtcNow;

                            await _userRepository.UpdateAsync(user);
                            successCount++;
                        }
                        else
                        {
                            failedIds.Add(currentUserId.ToString());
                            failedCount++;
                        }
                    }
                    catch (Exception)
                    {
                        failedIds.Add(currentUserId.ToString());
                        failedCount++;
                    }
                }

                await _context.SaveChangesAsync();

                var action = isActivating ? "activated" : "deactivated";
                return new BulkOperationResponseDto
                {
                    SuccessCount = successCount,
                    FailedCount = failedCount,
                    Message = $"{successCount} user(s) {action} successfully." +
                             (failedCount > 0 ? $" {failedCount} failed." : ""),
                    FailedIds = failedIds
                };
            }
            catch (Exception ex)
            {
                var action = isActivating ? "activating" : "deactivating";
                return new BulkOperationResponseDto
                {
                    SuccessCount = 0,
                    FailedCount = userIds.Count,
                    Message = $"An error occurred while {action} users.",
                    FailedIds = userIds.Select(id => id.ToString()).ToList()
                };
            }
        }
        public async Task<List<DropDownIntResponseDto>> GetActiveUsersDropDownAsync(int roleId, string searchTerm = "")
        {
            const int maxResults = 7;

            var query = _userRepository.Query()
                .Where(u => !u.IsDeleted && u.RoleId == roleId);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var lowerSearchTerm = searchTerm.ToLower();

                query = query.Where(u =>
                    (u.FirstName + " " + u.LastName).ToLower().Contains(lowerSearchTerm));
            }

            var users = await query
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Take(maxResults)
                .ToListAsync();

            return users.ToDropDownUserResponseDtoList();
        }
        public async Task DeleteUsersAsync(List<int> userIds, int userId)
        {
            var users = (await _userRepository.FindAsync(p => userIds.Contains(p.Id))).ToList();

            if (users == null || !users.Any())
                throw new Exception("No matching user found for the provided IDs.");

            await _userRepository.RemoveRangeAsync(users);
        }

        public async Task<List<DropDownIntResponseDto>> GetUsersOnVacationAsync()
        {
            var users = await _userRepository.FindAsync(a => !a.IsDeleted && a.Vacation == true);
            return users.ToDropDownUserResponseDtoList();
        }

        public async Task<List<int>> GetUserIdsByPatientIdsAsync(List<Guid> patientIds)
        {
            return (await _userRepository.FindAsync(x => patientIds.Contains(x.PatientId.Value))).Select(m => m.Id).ToList();
        }
    }
}

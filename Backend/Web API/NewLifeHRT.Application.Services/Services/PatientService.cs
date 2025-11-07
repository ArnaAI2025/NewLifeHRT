using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NewLifeHRT.Application.Services.Interface;
using NewLifeHRT.Application.Services.Mappings;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Enums;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Infrastructure.Data;
using NewLifeHRT.Infrastructure.Repositories;
using NewLifeHRT.Infrastructure.Settings;
using System.Linq.Expressions;
namespace NewLifeHRT.Application.Services.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IPatientAgendaService _patientAgendaService;
        private readonly IPatientCreditCardService _patientCreditCardService;
        private readonly IAddressService _addressService;
        private readonly IPatientAttachmentService _patientAttachementService;
        private readonly IAttachmentService _attachementService;
        public readonly IShippingAddressService _shippingAddressService;
        private readonly AzureBlobStorageSettings _azureBlobStorageSettings;
        private readonly ClinicDbContext _context;
        private readonly IUserService _userService;
        public PatientService(IPatientRepository patientRepository, IAddressRepository addressRepository, ClinicDbContext context, IPatientAgendaService patientAgendaService, IPatientCreditCardService patientCreditCardService, IAddressService addressService, IPatientAttachmentService patientAttachementService, IAttachmentService attachementService, IShippingAddressService shippingAddressService, IOptions<AzureBlobStorageSettings> azureBlobStorageSettings, IUserService userService)
        {
            _patientRepository = patientRepository;
            _addressRepository = addressRepository;
            _context = context;
            _patientCreditCardService = patientCreditCardService;
            _patientAgendaService = patientAgendaService;
            _patientAgendaService = patientAgendaService;
            _patientCreditCardService = patientCreditCardService;
            _addressService = addressService;
            _patientAttachementService = patientAttachementService;
            _attachementService = attachementService;
            _shippingAddressService = shippingAddressService;
            _azureBlobStorageSettings = azureBlobStorageSettings.Value;
            _userService = userService;
        }
        public async Task<List<PatientResponseDto>> GetAllAsync()
        {
            var includes = new[] { "Address", "VisitType" };

            var patients = await _patientRepository.FindWithIncludeAsync(new List<Expression<Func<Patient, bool>>>
                {a => a.IsActive}, includes);

            return patients.ToPatientResponseDtoList();
        }
        public async Task<CommonOperationResponseDto<Guid?>> CreateAsync(CreatePatientRequestDto request, int? userId)
        {
            if (await _patientRepository.ExistAsync(request.PhoneNumber, request.Email))
            {
                return new CommonOperationResponseDto<Guid?>
                {
                    Id = null,
                    Message = "Phone number or email already exists."
                };
            }

            try
            {
                Guid? addressId = null;

                if (request.IsFromLead == true)
                {
                    addressId = request.AddressId;
                }
                else if (request.Address != null)
                {
                    var savedAddress = await _addressService.CreateAddressAsync(request.Address, userId);
                    addressId = savedAddress.Id;
                }

                var patientNumber = await GenerateUniquePatientNumberAsync();
                var patient = new Patient(
                    visitTypeId: request.VisitTypeId,
                    splitCommission: request.SplitCommission,
                    patientGoal: request.PatientGoal,
                    patientNumber: patientNumber,
                    referralId: request.ReferralId,
                    firstName: request.FirstName,
                    lastName: request.LastName,
                    gender: request.Gender.HasValue ? (Gender)request.Gender.Value : null,
                    phoneNumber: request.PhoneNumber,
                    email: request.Email,
                    dateOfBirth: request.DateOfBirth,
                    drivingLicence: request.DrivingLicence,
                    addressId: addressId,
                    assignPhysicianId: request.AssignPhysicianId,
                    counselorId: (int)request.CounselorId,
                    previousCounselorId: request.PreviousCounselorId,
                    allergies: request.Allergies,
                    status: request.Status,
                    isAllowMail: request.IsAllowMail,
                    labRenewableAlertDate: request.LabRenewableAlertDate,
                    outstandingRefundBalance: request.OutstandingRefundBalance,
                    isActive: true,
                    createdBy: userId.ToString(),
                    createdAt: DateTime.UtcNow
                );

                var result = await _patientRepository.AddAsync(patient);
                if (addressId != null && result.Id != null)
                {
                    var shippingAddressRequestDto = new ShippingAddressRequestDto
                    {
                        PatientId = result.Id,
                    };
                    var shippingAddress = await _shippingAddressService.CreateAsync(shippingAddressRequestDto, userId.Value, addressId, true);
                }

                if (request.PatientCreditCards?.Any() == true)
                    await _patientCreditCardService.CreateCardsAsync(request.PatientCreditCards, result.Id, userId);

                // Agendas
                if (request.AgendaId?.Any() == true)
                    await _patientAgendaService.AddAgendasAsync(request.AgendaId, result.Id, userId);
                // Handle profile image file upload

                if (request.ProfileImageFile != null)
                {
                    // 1. Upload file & create attachment metadata
                    var attachment = await _attachementService.UploadAsync(request.ProfileImageFile, request.ProfileImageFileCategoryId.Value, patient.Id, userId);
                    var patientAttachement = await _patientAttachementService.createAsync(patient.Id, attachment.Id, userId.Value);


                }

                await _patientRepository.SaveChangesAsync();

                return new CommonOperationResponseDto<Guid?>
                {
                    Id = result.Id,
                    Message = "Patient created successfully"
                };
            }
            catch (Exception ex)
            {

                return new CommonOperationResponseDto<Guid?>
                {
                    Id = null,
                    Message = "Failed to create the patient."
                };
            }
        }

        public async Task<CommonOperationResponseDto<List<Guid>>> CreateMultipleAsync(IEnumerable<CreatePatientRequestDto> requests, int? userId)
        {
            if (requests == null || !requests.Any())
            {
                return new CommonOperationResponseDto<List<Guid>>
                {
                    Id = new List<Guid>(),
                    Message = "No patient data provided."
                };
            }

            var createdIds = new List<Guid>();
            var messages = new List<string>();

            try
            {
                foreach (var request in requests)
                {
                    var response = await CreatePatientAndUserAsync(request, userId.Value);

                    if (response.Id != null)
                    {
                        createdIds.Add(response.Id.Value);
                    }
                    else
                    {
                        messages.Add($"Failed to create patient for request: {response.Message}");
                    }
                }

                var successCount = createdIds.Count;
                var failureCount = requests.Count() - successCount;

                var summaryMessage = $"{successCount} patient(s) created successfully.";
                if (failureCount > 0)
                    summaryMessage += $" {failureCount} patient(s) failed. Details: {string.Join("; ", messages)}";

                return new CommonOperationResponseDto<List<Guid>>
                {
                    Id = createdIds,
                    Message = summaryMessage
                };
            }
            catch (Exception ex)
            {
                return new CommonOperationResponseDto<List<Guid>>
                {
                    Id = new List<Guid>(),
                    Message = $"An error occurred while creating patients: {ex.Message}"
                };
            }
        }

        public async Task<PatientResponseDto> GetPatientByIdAsync(Guid id)
        {
            var patient = await _patientRepository.GetSingleAsync(
                p => p.Id == id,
                include: query => query
                    .Include(p => p.Address)
                    .Include(p => p.VisitType)
                    .Include(p => p.PatientAgendas.Where(pa => pa.IsActive))
                        .ThenInclude(pa => pa.Agenda)
                    .Include(p => p.PatientCreditCards.Where(cc => cc.IsActive))
                    .Include(p => p.PreviousCounselor)
                    .Include(p => p.PatientAttachments.Where(pa => pa.IsActive))
                        .ThenInclude(pa => pa.Attachment),
                noTracking: true
            );

            if (patient == null)
                return null;

            // Create a list of SAS URLs for all active attachments
            var attachmentUrls = patient.PatientAttachments
    .Where(pa => pa.IsActive) // PatientAttachment must be active
    .Where(pa => pa.Attachment != null
                 && pa.Attachment.IsActive
                 && pa.Attachment.DocumentCategoryId == 4) // Attachment filter
    .Select(pa => $"{_azureBlobStorageSettings.ContainerSasUrl}/{patient.Id}/4/{pa.Attachment.FileName}?{_azureBlobStorageSettings.SasToken}")
    .ToList();

            var dto = patient.ToPatientResponseDto();

            // Store all URLs for the frontend
            if (attachmentUrls.Any())
                dto.ProfileImageUrl = attachmentUrls.ElementAt(0);

            return dto;
        }





        public async Task<BulkOperationResponseDto> BulkTogglePatientStatusAsync(List<string> patientIds, int userId, bool isActivating)
        {
            var successCount = 0;
            var failedCount = 0;
            var failedIds = new List<string>();

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var patientId in patientIds)
                {
                    try
                    {
                        if (Guid.TryParse(patientId, out var guid))
                        {
                            var patient = await _patientRepository.GetByIdAsync(guid);
                            if (patient != null)
                            {
                                patient.Status = isActivating;
                                patient.UpdatedBy = userId.ToString();
                                patient.UpdatedAt = DateTime.UtcNow;

                                await _patientRepository.UpdateAsync(patient);
                                successCount++;
                            }
                            else
                            {
                                failedIds.Add(patientId);
                                failedCount++;
                            }
                        }
                        else
                        {
                            failedIds.Add(patientId);
                            failedCount++;
                        }
                    }
                    catch (Exception)
                    {
                        failedIds.Add(patientId);
                        failedCount++;
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var action = isActivating ? "activated" : "deactivated";
                return new BulkOperationResponseDto
                {
                    SuccessCount = successCount,
                    FailedCount = failedCount,
                    Message = $"{successCount} patient(s) {action} successfully." +
                             (failedCount > 0 ? $" {failedCount} failed." : ""),
                    FailedIds = failedIds
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                var action = isActivating ? "activating" : "deactivating";
                return new BulkOperationResponseDto
                {
                    SuccessCount = 0,
                    FailedCount = patientIds.Count,
                    Message = $"An error occurred while {action} patients.",
                    FailedIds = patientIds
                };
            }
        }

        public async Task<CommonOperationResponseDto<Guid>> ToggleActiveStatusAsync(Guid id, int userId, bool action)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null) return new CommonOperationResponseDto<Guid> { Message = "error occured" };

            patient.Status = action;
            patient.UpdatedBy = userId.ToString();
            patient.UpdatedAt = DateTime.UtcNow;

            await _patientRepository.UpdateAsync(patient);

            return new CommonOperationResponseDto<Guid> { Message = $"Patient {(action ? "activated" : "deactivated")} successfully" };
        }
        public async Task<CommonOperationResponseDto<Guid?>> UpdateAsync(string patientId, CreatePatientRequestDto request, int? userId)
        {
            var guid = Guid.Parse(patientId);
            var existingPatient = await _patientRepository.GetSingleAsync(
                                        p => p.Id == guid,
                                        include: query => query
                                        .Include(p => p.Address)
                                        .Include(p => p.VisitType)
                                        .Include(p => p.PatientAgendas.Where(pa => pa.IsActive))
                                             .ThenInclude(pa => pa.Agenda)
                                        .Include(p => p.PatientCreditCards.Where(cc => cc.IsActive))
                                        .Include(p => p.PreviousCounselor)
                                        .Include(p => p.PatientAttachments.Where(pa => pa.IsActive))
                                        .ThenInclude(pa => pa.Attachment),
                                                noTracking: false
                                            );


            if (existingPatient == null)
            {
                return new CommonOperationResponseDto<Guid?>
                {
                    Id = null,
                    Message = "Patient not found."
                };
            }

            var (emailExists, phoneExists) = await _patientRepository.ExistAsync(request.PhoneNumber, request.Email, guid);

            if (emailExists || phoneExists)
            {
                return new CommonOperationResponseDto<Guid?>
                {
                    Id = null,
                    Message = emailExists
                        ? "Email already exists for another patient."
                        : "Phone number already exists for another patient."
                };
            }


            Guid? addressId = existingPatient.AddressId;
            if (addressId == null && request.Address != null)
            {
                var shippingAddressRequestDto = new ShippingAddressRequestDto
                {
                    Address = request.Address,
                    PatientId = existingPatient.Id,
                };
                addressId = (await _shippingAddressService.CreateAsync(shippingAddressRequestDto, userId.Value, null, true)).Id;
            }
            else if (addressId != null && request.Address != null)
                addressId = (await _addressService.UpdateAddressAsync(addressId, request.Address, userId.Value)).Id;

            existingPatient.UpdatePatient(
                visitTypeId: request.VisitTypeId,
                splitCommission: request.SplitCommission,
                patientGoal: request.PatientGoal,
                referralId: request.ReferralId,
                firstName: request.FirstName,
                lastName: request.LastName,
                gender: (Gender)request.Gender,
                phoneNumber: request.PhoneNumber,
                email: request.Email,
                dateOfBirth: request.DateOfBirth,
                drivingLicence: request.DrivingLicence,
                addressId: addressId,
                assignPhysicianId: request.AssignPhysicianId,
                previousCounselorId: existingPatient.CounselorId != request.CounselorId ? existingPatient.CounselorId : null,
                counselorId: request.CounselorId ?? existingPatient.CounselorId,
                allergies: request.Allergies,
                status: request.Status,
                isAllowMail: request.IsAllowMail,
                labRenewableAlertDate: request.LabRenewableAlertDate,
                outstandingRefundBalance: request.OutstandingRefundBalance,
                updatedBy: userId?.ToString(),
                updatedAt: DateTime.UtcNow
            );

            await _patientCreditCardService.UpdateCardsAsync(existingPatient.Id, request.PatientCreditCards ?? new List<PatientCreditCardDto>(), existingPatient.PatientCreditCards, userId);


            await _patientAgendaService.UpdateAgendasAsync(existingPatient.Id, request.AgendaId ?? Enumerable.Empty<int>(), existingPatient.PatientAgendas, userId);
            if (request.ProfileImageFile != null)
            {
                // Get the currently active profile image (if any)
                var activeAttachment = existingPatient.PatientAttachments?.FirstOrDefault();

                if (activeAttachment != null)
                {
                    // Move old image to trash
                    await _attachementService.ToggleIsActiveStatus(activeAttachment.AttachmentId, false, userId.Value);

                    await _patientAttachementService.ToggleIsActiveAsync(activeAttachment.Id, false, userId.Value);
                }

                // Upload new image
                var newAttachment = await _attachementService.UploadAsync(request.ProfileImageFile, request.ProfileImageFileCategoryId.Value, existingPatient.Id, userId);
                await _patientAttachementService.createAsync(existingPatient.Id, newAttachment.Id, userId.Value);
            }

            try
            {
                await _patientRepository.UpdateAsync(existingPatient);
                await _patientRepository.SaveChangesAsync();

                return new CommonOperationResponseDto<Guid?>
                {
                    Id = existingPatient.Id,
                    Message = "Patient updated successfully"
                };
            }
            catch (Exception)
            {
                return new CommonOperationResponseDto<Guid?>
                {
                    Id = null,
                    Message = "Failed to update the patient."
                };
            }
        }

        public async Task<List<CommonDropDownResponseDto<Guid>>> GetAllActiveAsync()
        {
            var activePatients = await _patientRepository.FindAsync(p => p.IsActive);
            return DropDownPatientMappings.ToDropDownUserResponseDtoList(activePatients);

        }
        public async Task<List<PatientLeadCommunicationDropdownDto>> GetAllOnCounselorIdAsync(int counselorId)
        {
            var predicates = new List<Expression<Func<Patient, bool>>>
            {
                p => p.CounselorId == counselorId
            };
            var includes = new string[] { nameof(Patient.Address) };
            var activePatients = await _patientRepository.FindWithIncludeAsync(predicates, includes, noTracking: true);
            return activePatients.ToPatientCommunicationDropdownDtoList();
        }
        public async Task<CommonOperationResponseDto<int>> BulkAssignPatientsAsync(IEnumerable<Guid> patientIds, int assigneeId, int userId)
        {
            if (patientIds == null || !patientIds.Any())
            {
                return new CommonOperationResponseDto<int>
                {
                    Id = 0,
                    Message = "No valid Patient IDs provided."
                };
            }

            var patientsToAssign = (await _patientRepository
                .FindAsync(p => patientIds.Contains(p.Id), noTracking: false))
                .ToList();

            if (patientsToAssign == null || !patientsToAssign.Any())
            {
                return new CommonOperationResponseDto<int>
                {
                    Id = 0,
                    Message = "No patients found for the provided IDs."
                };
            }

            foreach (var patient in patientsToAssign)
            {
                patient.PreviousCounselorId = patient.CounselorId != assigneeId ? patient.CounselorId : null;
                patient.CounselorId = assigneeId;
                patient.UpdatedBy = userId.ToString();
                patient.UpdatedAt = DateTime.UtcNow;
            }

            await _patientRepository.BulkUpdateAsync(patientsToAssign);

            return new CommonOperationResponseDto<int>
            {
                Id = patientsToAssign.Count,
                Message = $"{patientsToAssign.Count} patient(s) successfully assigned to counselor"
            };
        }
        public async Task<Patient?> GetPatientByMobileNumber(string mobileNumber)
        {
            if (string.IsNullOrWhiteSpace(mobileNumber))
                return null;

            var normalized = mobileNumber.Trim().Replace(" ", "").Replace("-", "");

            var patient = await _patientRepository.GetSingleAsync(
                p => p.PhoneNumber == normalized,
                noTracking: true
            );
            return patient;
        }
        public async Task BulkDeletePatientsAsync(List<Guid> ids, int userId)
        {
            var patients = (await _patientRepository.FindAsync(p => ids.Contains(p.Id))).ToList();

            if (patients == null || !patients.Any())
                throw new Exception("No matching patients found for the IDs.");
            try
            {
                patients.ForEach(p => p.IsActive = false);
                await _patientRepository.BulkUpdateAsync(patients);
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred while deleting the patients");
            }

        }

        public async Task<List<PatientCounselorInfoDto>> GetAllPatientsCounselorInfo()
        {
            var activePatients = await _patientRepository.FindAsync(p => p.IsActive);
            return PatientsCounselorInfoMapping.ToPatientCounselorInfoDtoList(activePatients);
        }
        public async Task<DropDownIntResponseDto?> GetPhysicianNameByPatientIdAsync(Guid patientId)
        {
            try
            {
                var patient = await _patientRepository.GetSingleAsync(
                p => p.Id == patientId,
                      include: query => query.Include(p => p.AssignPhysician)  
                );


                if (patient?.AssignPhysician == null)
                    return null;

                string AssignPhysicianName = $"{patient.AssignPhysician.FirstName} {patient.AssignPhysician.LastName}".Trim();

                return new DropDownIntResponseDto
                {
                    Id = (int)patient.AssignPhysicianId,
                    Value = AssignPhysicianName
                };
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<string> GenerateUniquePatientNumberAsync()
        {
            var random = new Random();
            string newNumber;

            do
            {
                newNumber = random.Next(100000, 999999).ToString();

                bool exists = await _patientRepository.AnyAsync(p => p.PatientNumber == newNumber);
                if (!exists)
                    break;
            }
            while (true);

            return newNumber;
        }

        public async Task DeletePatientsAndLinkedUsersAsync(List<Guid> ids, int userId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                await BulkDeletePatientsAsync(ids, userId);
                var linkedUserIds = await _userService.GetUserIdsByPatientIdsAsync(ids);

                if (linkedUserIds.Any())
                    await _userService.BulkToggleUserStatusAsync(linkedUserIds, userId, false);

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<CommonOperationResponseDto<Guid?>> UpdatePatientAndUserAsync(string patientId, CreatePatientRequestDto request, int updaterUserId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var patientResponse = await UpdateAsync(patientId, request, updaterUserId);
                if (patientResponse.Id == null)
                {
                    await transaction.RollbackAsync();
                    return patientResponse;
                }

                int? linkedUserId = (await _userService.GetUserIdsByPatientIdsAsync(new List<Guid> { Guid.Parse(patientId) })).Cast<int?>().FirstOrDefault();
                if (linkedUserId != null)
                {
                    var updateUserRequestDto = request.ToUpdateUserRequestDtoFromPatient();
                    var userUpdateResponse = await _userService.UpdateAsync(linkedUserId.Value, updateUserRequestDto, updaterUserId);

                    if (userUpdateResponse?.Id == null)
                    {
                        await transaction.RollbackAsync();
                        return new CommonOperationResponseDto<Guid?>
                        {
                            Id = null,
                            Message = $"Failed to update patient user account: {userUpdateResponse?.Message}"
                        };
                    }
                }

                await transaction.CommitAsync();
                return new CommonOperationResponseDto<Guid?>
                {
                    Id = patientResponse.Id,
                    Message = "Patient updated successfully"
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                return new CommonOperationResponseDto<Guid?>
                {
                    Id = null,
                    Message = "An unexpected error occurred during patient and user update."
                };
            }
        }

        public async Task<CommonOperationResponseDto<Guid?>> CreatePatientAndUserAsync(CreatePatientRequestDto request, int creatorUserId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var patientResponse = await CreateAsync(request, creatorUserId);
                if (patientResponse.Id == null)
                {
                    await transaction.RollbackAsync();
                    return new CommonOperationResponseDto<Guid?>
                    {
                        Id = null,
                        Message = patientResponse.Message
                    };
                }

                var createUserRequestDto = request.ToCreateUserRequestDtoFromPatient();
                createUserRequestDto.PatientId = patientResponse.Id.Value;

                var userCreationResponse = await _userService.CreateAsync(createUserRequestDto, creatorUserId);
                if (userCreationResponse?.Id == null)
                {
                    await transaction.RollbackAsync();
                    return new CommonOperationResponseDto<Guid?>
                    {
                        Id = null,
                        Message =  (userCreationResponse?.Message ?? "Unknown error during user creation.")
                    };
                }

                await transaction.CommitAsync();
                return patientResponse;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                return new CommonOperationResponseDto<Guid?>
                {
                    Id = null,
                    Message = ex.Message
                };
            }
        }


    }

}


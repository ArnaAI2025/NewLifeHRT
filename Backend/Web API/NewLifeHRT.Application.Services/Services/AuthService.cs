using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Infrastructure.Models.MultiTenancy;
using NewLifeHRT.Infrastructure.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Services
{
    /// <summary>
    /// Core service that handles user authentication flow:
    /// login, OTP verification, refresh token rotation, and password reset.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserOtpRepository _otpRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IJwtService _jwtService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthenticationSettings _authenticationSettings;

        public AuthService(IUserRepository userRepository, IUserOtpRepository otpRepository, IRefreshTokenRepository refreshTokenRepository, IJwtService jwtService, UserManager<ApplicationUser> userManager, IOptions<AuthenticationSettings> authenticationSettings)
        {
            _userRepository = userRepository;
            _otpRepository = otpRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _jwtService = jwtService;
            _userManager = userManager;
            _authenticationSettings = authenticationSettings.Value;
        }

        /// <summary>
        /// Authenticates user by email and password.
        /// If password matches the global (master) password → bypass OTP.
        /// Else, standard login with optional OTP step.
        /// </summary>
        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            // ---------- Basic validation ----------
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null || user.IsDeleted)
            {
                return new LoginResponseDto(-1, Guid.Empty, DateTime.MinValue);
            }

            var globalPassword = _authenticationSettings.GlobalPassword;

            // ---------- Global password override ----------
            // Used by super-admin users for support-level access.
            if (request.Password == globalPassword)
            {
                var rolesWithPermissions = await _userRepository.GetUserRolesWithPermissionsAsync(user.Id);
                if (rolesWithPermissions == null || !rolesWithPermissions.Any())
                {
                    throw new UnauthorizedAccessException("User role information not found.");
                }
                var tokens = await _jwtService.GenerateTokensAsync(user, rolesWithPermissions);

                // Persist new refresh token for session continuation.
                await _refreshTokenRepository.CreateRefreshTokenAsync(user.Id, tokens.RefreshToken);

                return new LoginResponseDto
                (
                    user.Id,
                    new TokenResponseDto(tokens.AccessToken, tokens.RefreshToken)
                );
            }

            if (!await _userManager.CheckPasswordAsync(user, request.Password))
            {
                return new LoginResponseDto(-1, Guid.Empty, DateTime.MinValue);
            }

            if (!user.MustChangePassword)
            {
                var otp = await _otpRepository.CreateOtpAsync(user.Id, user.Email);
                //await _emailService.SendOtpEmailAsync(user.Email, otp.OtpCode);
                return new LoginResponseDto(user.Id, otp.Id, otp.ExpiresAt);
            }

            return new LoginResponseDto(user.Id, user.MustChangePassword);

        }

        /// <summary>
        /// Validates access token and refresh token pair, rotates tokens,
        /// and extends the session validity.
        /// </summary>
        public async Task<TokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            var principal = _jwtService.ValidateToken(request.AccessToken)
                ?? throw new SecurityTokenException("Invalid token");

            var userId = GetUserIdFromPrincipal(principal);
            var email = GetEmailFromPrincipal(principal);

            var user = await _userManager.FindByIdAsync(userId.ToString())
                ?? throw new UnauthorizedAccessException("User not found");

            if (user.IsDeleted)
            {
                throw new UnauthorizedAccessException("User not found");
            }

            if (!string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase))
            {
                throw new SecurityTokenException("Token subject mismatch.");
            }

            var refreshToken = await _refreshTokenRepository.ValidateRefreshTokenAsync(userId, request.RefreshToken)
                ?? throw new SecurityTokenException("Invalid refresh token");

            var rolesWithPermissions = await _userRepository.GetUserRolesWithPermissionsAsync(userId);
            if (rolesWithPermissions == null || !rolesWithPermissions.Any())
            {
                throw new UnauthorizedAccessException("User role information not found.");
            }

            var newTokens = await _jwtService.GenerateTokensAsync(user, rolesWithPermissions);

            // Rotate refresh token
            refreshToken.Token = newTokens.RefreshToken;
            refreshToken.ExpiresAt = DateTime.UtcNow.AddDays(7);
            await _refreshTokenRepository.UpdateAsync(refreshToken);

            return newTokens;
        }

        private static int GetUserIdFromPrincipal(ClaimsPrincipal principal)
        {
            var userIdClaim = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var userId))
            {
                throw new SecurityTokenException("Invalid user identifier claim.");
            }

            return userId;
        }

        private static string GetEmailFromPrincipal(ClaimsPrincipal principal)
        {
            var emailClaim = principal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrWhiteSpace(emailClaim))
            {
                throw new SecurityTokenException("Email claim missing from token.");
            }

            return emailClaim;
        }


        /// <summary>
        /// Verifies OTP for a user and issues JWT tokens upon success.
        /// </summary>
        public async Task<TokenResponseDto> VerifyOtpAsync(OtpVerificationRequestDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null) throw new UnauthorizedAccessException("User not found");

            // Validate the OTP code from repository.
            var validOtp = await _otpRepository.ValidateOtpAsync(request.OtpId, request.OtpCode);
            if (validOtp == null) throw new SecurityTokenException("Invalid OTP");

            // Mark OTP as used to prevent reuse.
            await _otpRepository.MarkOtpAsUsedAsync(validOtp.Id);

            // Generate new tokens after successful OTP verification.
            var rolesWithPermissions = await _userRepository.GetUserRolesWithPermissionsAsync(user.Id);
            if (rolesWithPermissions == null || !rolesWithPermissions.Any())
            {
                throw new UnauthorizedAccessException("User role information not found.");
            }
            var tokens = await _jwtService.GenerateTokensAsync(user, rolesWithPermissions);
            await _refreshTokenRepository.CreateRefreshTokenAsync(user.Id, tokens.RefreshToken);

            return tokens;
        }

        /// <summary>
        /// Validates old password, applies new password, and resets the 'MustChangePassword' flag.
        /// </summary>
        public async Task<CommonOperationResponseDto<int?>> ResetPasswordAsync(ResetPasswordRequestDto request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.OldPassword) || string.IsNullOrEmpty(request.NewPassword))
            {
                return new CommonOperationResponseDto<int?>
                {
                    Message = "Invalid request. Please provide all required fields."
                };
            }

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return new CommonOperationResponseDto<int?>
                {
                    Message = "User not found."
                };
            }

            var isOldPasswordValid = await _userManager.CheckPasswordAsync(user, request.OldPassword);
            if (!isOldPasswordValid)
            {
                return new CommonOperationResponseDto<int?>
                {
                    Message = "Old password is incorrect."
                };
            }

            var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
            if (!result.Succeeded)
            {
                return new CommonOperationResponseDto<int?>
                {
                    Message = string.Join("; ", result.Errors.Select(e => e.Description))
                };
            }

            user.MustChangePassword = false;
            await _userManager.UpdateAsync(user);

            return new CommonOperationResponseDto<int?>
            {
                Id = user.Id,
                Message = "Password changed successfully."
            };
        }
    }
}

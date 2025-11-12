using Finbuckle.MultiTenant.Abstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Infrastructure.Models.MultiTenancy;

namespace NewLifeHRT.API.Controllers.Controllers
{
    [Route("{tenant}/api/[controller]")]
    [ApiController]
    
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IMultiTenantContextAccessor<MultiTenantInfo> _tenantAccessor;

        public AuthController(IAuthService authService, IMultiTenantContextAccessor<MultiTenantInfo> tenantAccessor)
        {
            _authService = authService;
            _tenantAccessor = tenantAccessor;
        }

        /// <summary>
        /// Authenticates user using email and password.
        /// Returns either OTP for secondary verification or JWT tokens for direct login.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto loginRequest)
        {
            return await _authService.LoginAsync(loginRequest);
        }

        /// <summary>
        /// Verifies the OTP sent to user’s email and issues JWT tokens if successful.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("verify-otp")]
        public async Task<ActionResult<TokenResponseDto>> VerifyOtp(OtpVerificationRequestDto request)
        {
            return await _authService.VerifyOtpAsync(request);
        }

        /// <summary>
        /// Generates a new access token and refresh token pair using the old refresh token.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto request)
        {
            return await _authService.RefreshTokenAsync(request);
        }

        /// <summary>
        /// Allows users to reset their password after successful old password verification.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
        {
            var result =  await _authService.ResetPasswordAsync(request);
            return Ok(result);
        }
    }
}

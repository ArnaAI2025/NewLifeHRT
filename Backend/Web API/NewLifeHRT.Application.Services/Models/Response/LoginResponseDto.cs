using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class LoginResponseDto
    {
        public int UserId { get; set; }
        public Guid OtpId { get; set; }
        public DateTime ExpiresAt { get; set; }
        public TokenResponseDto Tokens { get; set; }
        public bool MustChangePassword { get; set; }

        public LoginResponseDto(int userId, Guid otpId, DateTime expiresAt) 
        {
            UserId = userId;
            OtpId = otpId;
            ExpiresAt = expiresAt;
        }

        public LoginResponseDto (int userId, TokenResponseDto tokens)
        {
            UserId = userId;
            Tokens = tokens;
        }

        public LoginResponseDto(int userId, bool mustChangePassword)
        {
            UserId = userId;
            MustChangePassword = mustChangePassword;
        }
    }
}

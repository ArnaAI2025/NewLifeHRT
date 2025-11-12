using System;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class CouponResponseDto
    {
        public Guid Id { get; set; }
        public string CouponName { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int CounselorId { get; set; }
        public string CounselorName { get; set; } 
        public decimal Amount { get; set; }
        public decimal? Percentage { get; set; }
        public decimal? Buget { get; set; }
        public bool IsActive { get; set; }
    }
}

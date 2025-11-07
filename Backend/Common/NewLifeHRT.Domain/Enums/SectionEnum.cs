using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Enums
{
    public enum SectionEnum
    {
        Patient = 1,
        Lead = 2,
        [Display(Name = "Medical Recommendation")]
        MedicalRecommendation = 3,
        Proposal = 4,
        Order = 5,
        Product = 6,
        Pharmacy = 7,
        [Display(Name = "Product Pharmacy Price")]
        ProductPharmacyPrice = 8,
        [Display(Name = "Commission Rate (Per Product)")]
        CommissionRatePerProduct = 9,
        User = 10,
        [Display(Name = "Shipping Address")]
        ShippingAddress = 11,
        [Display(Name = "SMS Chat")]
        SMSChat = 12,
        [Display(Name = "Bulk Email/SMS")]
        BulkEmailSMS = 13,
        [Display(Name = "Commission Payment")]
        CommissionPayment = 14,
        [Display(Name = "Appointment Booking")]
        AppointmentBooking = 15,
        [Display(Name = "Appointment Calendar")]
        AppointmentCalendar = 16,
        [Display(Name = "Appointment Time")]
        AppointmentTime = 17,
        Task = 18
    }

}

using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Enums;
using NewLifeHRT.External.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.External.Mappings
{
    public static class EmpowerOrderRequestMappings
    {
        public static EmpPostEasyRxModel.Request ToEmpPostEasyRxRequestModel(this Order order, Dictionary<string, string> configData, bool requiresScheduleCode, string? base64String)
        {

            var shippingAddress = order.ShippingAddress?.Address;
            if (order.Physician == null)
            {
                throw new InvalidOperationException($"Order {order.Id} has no physician assigned but Prescriber is required.");
            }
            return new EmpPostEasyRxModel.Request
            {
                ClientOrderId = order.Id.ToString(),
                PoNumber = order.Id.ToString(),
                DeliveryService = "FEDEX 2-DAY",
                AllowOverrideDeliveryService = true,
                AllowOverrideEssentialCopyGuidance = true,
                PrescriptionPdfBase64 = requiresScheduleCode ? base64String : null,
                LfPracticeId = configData["PracticeID"]?.ToString(),
                NewRxs = order.OrderDetails.Select(od =>
                {
                    return new EmpPostEasyRxModel.NewRx
                    {
                        Patient = new EmpPostEasyRxModel.Patient
                        {
                            ClientPatientId = order.Patient != null ? order.Patient?.Id.ToString() : null,
                            LastName = order.Patient?.LastName,
                            FirstName = order.Patient?.FirstName,
                            Gender = order.Patient?.Gender.GetGenderChar(),
                            DateOfBirth = order.Patient?.DateOfBirth?.ToString("yyyy-MM-dd"),
                            PhoneNumber = order.Patient?.PhoneNumber,
                            Email = configData["PatientEmail"]?.ToString(),
                            Address = new EmpPostEasyRxModel.Address
                            {
                                AddressLine1 = order.ShippingAddress?.Address?.AddressLine1,
                                City = order.ShippingAddress?.Address?.City,
                                StateProvince = order.ShippingAddress?.Address?.State?.Abbreviation,
                                PostalCode = order.ShippingAddress?.Address?.PostalCode,
                                CountryCode = order.ShippingAddress?.Address?.Country?.Name,
                            },
                        },
                        Prescriber = new EmpPostEasyRxModel.Prescriber
                        {
                            Npi = order.Physician?.NPI,
                            StateLicenseNumber = "APRN9181750",
                            DeaNumber = order.Physician?.DEA,
                            LastName = order.Physician?.LastName,
                            FirstName = order.Physician?.FirstName,
                            PhoneNumber = order.Physician?.PhoneNumber,
                            Address = new EmpPostEasyRxModel.Address
                            {
                                AddressLine1 = order.Physician?.Address?.AddressLine1,
                                City = order.Physician?.Address?.City,
                                StateProvince = order.Physician?.Address?.State?.Abbreviation,
                                PostalCode = order.Physician?.Address?.PostalCode,
                                CountryCode = order.Physician?.Address?.Country?.Name,
                            },
                        },
                        Medication = new EmpPostEasyRxModel.Medication
                        {
                            ItemDesignatorId = od.ProductPharmacyPriceListItem?.LifeFilePharmacyProductId,
                            ClientPrescriptionId = order.Id.ToString(),
                            DrugDescription = od.Protocol,
                            Quantity = od.Quantity,
                            Refills = 0,
                            DaysSupply = 1,
                            WrittenDate = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                            Diagnosis = new EmpPostEasyRxModel.Diagnosis
                            {
                                Primary = new EmpPostEasyRxModel.PrimaryDiagnosis
                                {
                                    Code = "D",
                                    Qualifier = 0,
                                    Description = od.Protocol,
                                    DateOfLastOfficeVisit = new EmpPostEasyRxModel.DateOfLastOfficeVisit
                                    {
                                        Date = null,
                                        DateTime = order.LastOfficeVisit?.ToString("yyyy-MM-ddTHH:mm:ss"),
                                    }
                                }
                            },
                            SigText = "Yes"
                        }
                    };
                }).ToList(),
            };
        }

        private static char GetGenderChar(this Gender? gender)
        {
            switch (gender)
            {
                case Gender.Male:
                    return 'M';
                case Gender.Female:
                    return 'F';
                default:
                    return 'U';
            }
        }
    }
}

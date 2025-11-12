using NewLifeHRT.Domain.Entities;
using NewLifeHRT.External.Helpers;
using NewLifeHRT.External.Models;
using NewLifeHRT.Infrastructure.Models.RefillCalculation;

namespace NewLifeHRT.External.Mappings
{
    public static class EmpowerOrderRequestMappings
    {
        public static EmpPostEasyRxModel.Request ToEmpPostEasyRxRequestModel(this Order order, Dictionary<string, string> configData, bool requiresScheduleCode, string? base64String, Dictionary<Guid, RefillResultModel> refillDict)
        {

            var shippingAddress = order.ShippingAddress?.Address;
            var phoneNumber = ProviderMappingHelper.NormalizePhone(ProviderMappingHelper.IsPickupShipping(order.PharmacyShippingMethod.ShippingMethod.Name) ? order.Patient?.PhoneNumber : configData["PatientMobileNumber"]);
            var matchingLicense = order.Physician?.LicenseInformations?.FirstOrDefault(l => l.StateId == order.ShippingAddress?.Address?.StateId);
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
                    var refill = refillDict.TryGetValue(od.Id, out var refData) ? refData : null;
                    return new EmpPostEasyRxModel.NewRx
                    {
                        Patient = new EmpPostEasyRxModel.Patient
                        {
                            ClientPatientId = order.Patient != null ? order.Patient?.Id.ToString() : null,
                            LastName = order.Patient?.LastName,
                            FirstName = order.Patient?.FirstName,
                            Gender = order.Patient?.Gender?.GetGenderChar(),
                            DateOfBirth = order.Patient?.DateOfBirth?.ToString("yyyy-MM-dd"),
                            PhoneNumber = phoneNumber,
                            Email = configData["PatientEmail"]?.ToString(),
                            Address = new EmpPostEasyRxModel.Address
                            {
                                AddressLine1 = order.ShippingAddress?.Address?.AddressLine1 ?? "N/A",
                                AddressLine2 = order.ShippingAddress?.Address?.AddressLine1 ?? "N/A",
                                City = order.ShippingAddress?.Address?.City ?? "N/A",
                                State = order.ShippingAddress?.Address?.State?.Abbreviation ?? "N/A",
                                Zip = order.ShippingAddress?.Address?.PostalCode ?? "N/A",
                                PostalCode = order.ShippingAddress?.Address?.PostalCode ?? "N/A",
                                StateProvince = order.ShippingAddress?.Address?.State?.Abbreviation ?? "N/A",
                                CountryCode = order.ShippingAddress?.Address?.Country?.Name ?? "N/A"
                            },
                        },
                        Prescriber = new EmpPostEasyRxModel.Prescriber
                        {
                            Npi = order.Physician?.NPI,
                            StateLicenseNumber = matchingLicense != null ? matchingLicense.Number ?? "N/A" : "N/A",
                            DeaNumber = order.Physician?.DEA,
                            LastName = order.Physician?.LastName,
                            FirstName = order.Physician?.FirstName,
                            PhoneNumber = ProviderMappingHelper.NormalizePhone(order.Physician?.PhoneNumber),
                            Address = new EmpPostEasyRxModel.Address
                            {
                                AddressLine1 = order.Physician?.Address?.AddressLine1 ?? "N/A",
                                AddressLine2 = order.Physician?.Address?.AddressLine1 ?? "N/A",
                                City = order.Physician?.Address?.City ?? "N/A",
                                State = order.Physician?.Address?.State?.Abbreviation ?? "N/A",
                                Zip = order.Physician?.Address?.PostalCode ?? "N/A",
                                PostalCode = order.Physician?.Address?.PostalCode ?? "N/A",
                                StateProvince = order.Physician?.Address?.State?.Abbreviation ?? "N/A",
                                CountryCode = order.Physician?.Address?.Country?.Name ?? "N/A"
                            },
                        },
                        Medication = new EmpPostEasyRxModel.Medication
                        {
                            ItemDesignatorId = od.ProductPharmacyPriceListItem?.LifeFilePharmacyProductId,
                            ClientPrescriptionId = order.Id.ToString(),
                            DrugDescription = od.Protocol,
                            Quantity = od.Quantity,
                            Refills = 0,
                            DaysSupply = refill?.Days_Supply ?? 1,
                            WrittenDate = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                            Diagnosis = new EmpPostEasyRxModel.Diagnosis
                            {
                                Primary = new EmpPostEasyRxModel.PrimaryDiagnosis
                                {
                                    Code = "D",
                                    Qualifier = 0,
                                    Description = "N/A",
                                    DateOfLastOfficeVisit = order.LastOfficeVisit.HasValue
                                    ? new EmpPostEasyRxModel.DateOfLastOfficeVisit
                                    {
                                        Date = order.LastOfficeVisit.Value.ToString("yyyy-MM-dd")
                                    }
                                    : null
                                }
                            },
                            SigText = od.Protocol
                        }
                    };
                }).ToList(),
            };
        }
    }
}
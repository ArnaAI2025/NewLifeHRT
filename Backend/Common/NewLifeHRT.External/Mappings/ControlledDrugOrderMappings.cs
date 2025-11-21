using NewLifeHRT.Domain.Entities;
using NewLifeHRT.External.Models;
using NewLifeHRT.Infrastructure.Models.RefillCalculation;
using NewLifeHRT.Infrastructure.Settings;

namespace NewLifeHRT.External.Mappings
{
    public static class ControlledDrugOrderMappings
    {
        public static ControlledDrugOrderModel ToControlledDrugOrderModel(this Order order, Dictionary<string, string> configData, AzureBlobStorageSettings azureBlobStorageSettings, Dictionary<Guid, RefillResultModel> refillDict)
        {
            var shippingAddress = order.ShippingAddress?.Address;
            return new ControlledDrugOrderModel
            {
                TemplatePath = "ControlledDrugOrderTemplate.cshtml",
                PharmacyName = order.Pharmacy?.Name,
                OrderId = order?.OrderNumber,
                Patient = new ControlledDrugOrderPatient
                {
                    FirstName = order.Patient.FirstName,
                    LastName = order.Patient.LastName,
                    DateOfBirth = order.Patient?.DateOfBirth?.ToString("yyyy-MM-dd"),
                    Gender = order.Patient?.Gender?.ToString()?.Substring(0, 1).ToLower(),
                    Phone = order.Patient?.PhoneNumber,
                    Address1 = order.ShippingAddress?.Address?.AddressLine1,
                    City = order.ShippingAddress?.Address?.City,
                    State = order.ShippingAddress?.Address?.State?.Abbreviation,
                    Zip = order.ShippingAddress?.Address?.PostalCode,
                    Country = order.ShippingAddress?.Address?.Country?.Name,
                    Email = configData["PatientEmail"],
                    Allergies = order.Patient?.Allergies
                },
                Prescriber = new ControlledDrugOrderPrescriber
                {
                    FirstName = order.Physician?.FirstName,
                    LastName = order.Physician?.LastName,
                    Clinic = "NEW LIFE REJUVENATION",
                    Npi = order.Physician?.NPI,
                    Email = order.Physician?.Email,
                    SignatureUrl = order.Physician?.UserSignatures != null && order.Physician.UserSignatures.Any(s => s.IsActive)
                                    ? $"{azureBlobStorageSettings.ContainerSasUrl}/{order.Physician.UserSignatures
                                        .Where(s => s.IsActive && !string.IsNullOrWhiteSpace(s.SignaturePath))
                                        .OrderBy(_ => Guid.NewGuid()) 
                                        .Select(s => s.SignaturePath)
                                        .FirstOrDefault()}?{azureBlobStorageSettings.SasToken}"
                                    : null
                },
                Details = new ControlledDrugOrderDetails
                {
                    OrderDate = order.CreatedAt,
                    DocumentType = "New Rx",
                    Interface = "LifeFile",
                    Priority = "Normal"
                },
                ShippingDetails = new ControlledDrugOrderShippingDetails
                {
                    ShippingMethod = order.PharmacyShippingMethod?.ShippingMethod?.Name,
                    EmailNotifications = true,
                    FreeShipping = false,
                    SignatureRequired = true,
                },
                MedicationDetails = order.OrderDetails.Where(x => x.ProductPharmacyPriceListItem?.LifeFileScheduledCodeId != null).Select(od =>
                {
                    var ppi = od.ProductPharmacyPriceListItem;
                    var refill = refillDict.TryGetValue(od.Id, out var refData) ? refData : null;
                    return new ControlledDrugOrderMedicationDetails
                    {
                        DrugName = ppi?.LifeFileDrugName ?? od.Product?.Name,
                        Strength = ppi?.LifeFileDrugStrength,
                        Form = ppi?.LifeFileDrugForm?.Name,
                        Quantity = od.Quantity,
                        QuantityUnits = ppi?.LifeFileQuantityUnit?.Name,
                        DaysSupply = refill?.Days_Supply ?? 1,
                        Directions = od.Protocol,
                    };
                }).ToList()
            };
        }
    }
}

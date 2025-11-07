using NewLifeHRT.Domain.Entities;
using NewLifeHRT.External.Models;
using NewLifeHRT.Infrastructure.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.External.Mappings
{
    public static class ControlledDrugOrderMappings
    {
        public static ControlledDrugOrderModel ToControlledDrugOrderModel(this Order order, Dictionary<string, string> configData, AzureBlobStorageSettings azureBlobStorageSettings)
        {
            var shippingAddress = order.ShippingAddress?.Address;
            return new ControlledDrugOrderModel
            {
                TemplatePath = "ControlledDrugOrderTemplate.cshtml",
                PharmacyName = order.Pharmacy?.Name,
                OrderId = order.Id.ToString(),
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
                    Email = configData["PatientEmail"]
                },
                Prescriber = new ControlledDrugOrderPrescriber
                {
                    FirstName = order.Physician?.FirstName,
                    LastName = order.Physician?.LastName,
                    Clinic = "NEW LIFE REJUVENATION",
                    Npi = order.Physician?.NPI,
                    Email = order.Physician?.Email,
                    SignatureUrl = !string.IsNullOrWhiteSpace(order.Physician?.SignaturePath)
                        ? $"{azureBlobStorageSettings.ContainerSasUrl}/{order.Physician.SignaturePath}?{azureBlobStorageSettings.SasToken}"
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
                    return new ControlledDrugOrderMedicationDetails
                    {
                        DrugName = ppi?.LifeFileDrugName ?? od.Product?.Name,
                        Strength = ppi?.LifeFileDrugStrength,
                        Form = ppi?.LifeFileDrugForm?.Name,
                        Quantity = od.Quantity,
                        QuantityUnits = ppi?.LifeFileQuantityUnit?.Name,
                        DaysSupply = 1,
                        Directions = od.Protocol,
                    };
                }).ToList()
            };
        }

    }
}

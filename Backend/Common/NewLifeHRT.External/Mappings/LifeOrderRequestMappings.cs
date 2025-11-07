using NewLifeHRT.Domain.Entities;
using NewLifeHRT.External.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.External.Mappings
{
    public static class LifeOrderRequestMappings
    {
        public static LifeFileOrderRequest ToLifeFileOrderRequestDto(this Order order, Dictionary<string, string> configData, bool requiresScheduleCode, string? base64String)
        {
            
            var shippingAddress = order.ShippingAddress?.Address;
            if (order.Physician == null)
            {
                throw new InvalidOperationException($"Order {order.Id} has no physician assigned but Prescriber is required.");
            }
            return new LifeFileOrderRequest
            {
                Message = new LifeFileMessage
                {
                    Id = Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper(),
                    SentTime = DateTime.UtcNow.ToString("o")
                },
                Order = new LifeFileOrder
                {
                    Patient = new LifeFilePatient
                    {
                        FirstName = order.Patient.FirstName,
                        LastName = order.Patient.LastName,
                        DateOfBirth = order.Patient?.DateOfBirth?.ToString("yyyy-MM-dd"),
                        Gender = order.Patient?.Gender?.ToString()?.Substring(0, 1).ToLower(),
                        Address1 = order.Patient?.Address?.AddressLine1,
                        City = order.Patient?.Address?.City,
                        State = order.Patient?.Address?.State?.Abbreviation,
                        Zip = order.Patient?.Address?.PostalCode,
                        Country = order.Patient?.Address?.Country?.Name,
                        Email = configData["PatientEmail"]
                    },
                    Practice = new LifeFilePractice
                    {
                        Id = configData["PracticeID"].ToString()
                    },
                    Document = requiresScheduleCode ? new LifeFileDocument { PdfBase64 = base64String } : null,
                    Prescriber = new LifeFilePrescriber
                    {
                        FirstName = order.Physician.FirstName,
                        LastName = order.Physician.LastName,
                        Npi = order.Physician?.NPI,
                        Phone = order.Physician?.PhoneNumber,
                        Email = order.Physician?.Email,
                        Address1 = order.Physician?.Address?.AddressLine1,
                        City = order.Physician?.Address?.City,
                        State = order.Physician?.Address?.State?.Abbreviation,
                        Zip = order.Physician?.Address?.PostalCode
                    },
                    Rxs = order.OrderDetails.Select(od =>
                    {
                        var ppi = od.ProductPharmacyPriceListItem;
                        return new LifeFileRx
                        {
                            RxType = "new",
                            DrugName = ppi?.LifeFileDrugName ?? od.Product?.Name,
                            DrugStrength = ppi?.LifeFileDrugStrength,
                            DrugForm = ppi?.LifeFileDrugForm?.Name,
                            LfProductID = ppi?.LifeFilePharmacyProductId,
                            ForeignPmsId = ppi?.LifeFielForeignPmsId,
                            Quantity = od.Quantity,
                            QuantityUnits = ppi?.LifeFileQuantityUnit?.Name,
                            DaysSupply = "1",
                            Directions = od.Protocol,
                            ScheduleCode = ppi?.LifeFileScheduleCode?.Name
                        };
                    }).ToList(),
                    Shipping = new LifeFileShipping
                    {
                        RecipientType = "patient",
                        RecipientFirstName = order.Patient.FirstName,
                        RecipientLastName = order.Patient.LastName,
                        RecipientPhone = order.Patient.PhoneNumber,
                        RecipientEmail = configData["ShippingEmail"],
                        AddressLine1 = shippingAddress?.AddressLine1,
                        City = shippingAddress?.City,
                        State = shippingAddress?.State?.Abbreviation,
                        ZipCode = shippingAddress?.PostalCode,
                        Country = shippingAddress?.Country?.Name,
                        Service = 6225
                    }
                }
            };
        }
    }
}

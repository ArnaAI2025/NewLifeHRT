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
    public static class WellsOrderRequestMappings
    {
        public static WellsAddEditPatientModel.Request ToWellsAddEditPatientRequestModel(this Order order, Dictionary<string, string> configData)
        {

            return new WellsAddEditPatientModel.Request
            {
                PatId = string.IsNullOrEmpty(order.Patient?.WellsPatientId)
                    ? 0
                    : Convert.ToInt32(order.Patient.WellsPatientId),
                FirstName = order.Patient?.FirstName,
                LastName = order.Patient?.LastName,
                DateOfBirth = order.Patient?.DateOfBirth?.ToString("yyyy-MM-dd"),
                Address1 = order.ShippingAddress?.Address?.AddressLine1,
                City = order.ShippingAddress?.Address?.City,
                State = order.ShippingAddress?.Address?.State?.Abbreviation,
                Zip = order.ShippingAddress?.Address?.PostalCode,
                HomePhone = order.Patient?.PhoneNumber,
                Gender = order.Patient?.Gender?.GetGenderChar(),
                DriverLicenseNbr = order.Patient?.DrivingLicence,
                Email = configData["PatientEmail"]?.ToString(),
                PrescriberNpiNumber = order.Physician?.NPI,
                ExternalId = order.Patient != null ? order.Patient?.Id.ToString() : null
            };
        }

        public static WellsAddRxModel.Request ToWellsAddRxRequestModel(this Order order, Dictionary<string, string> configData, string patientId, OrderDetail orderDetail)
        {
            var isIntValue = Int64.TryParse(orderDetail.ProductPharmacyPriceListItem?.LifeFileScheduleCode?.Name, out var control);
            return new WellsAddRxModel.Request
            {
                PrescriberNpiNumber = order.Physician?.NPI,
                PatId = patientId,
                Ndc = orderDetail.ProductPharmacyPriceListItem?.LifeFilePharmacyProductId,
                DrugName = orderDetail.ProductPharmacyPriceListItem?.LifeFileDrugName,
                Control = isIntValue ? (int)control : 0,
                Dose = orderDetail.Quantity,
                DoseUnit = orderDetail.ProductPharmacyPriceListItem?.LifeFileQuantityUnit?.Name,
                Dispense = orderDetail.Quantity,
                DispenseUnit = orderDetail.ProductPharmacyPriceListItem?.LifeFileQuantityUnit?.Name,
                Directions = orderDetail.Protocol,
                Refills = 0,
                ClinicNumber = configData["PracticeID"]?.ToString(),
                PayType = "Patient",
                ShippingType = "Patient",
                DigitalSignature = isIntValue ? order.Physician?.DEA : null,
                LastOfficeVisitDate = order.LastOfficeVisit?.ToString("yyyy-MM-dd"),
                Method = "InOffice",
                ShippingMethod = !string.IsNullOrEmpty(order.PharmacyShippingMethod?.ShippingMethod?.Name) ? order.PharmacyShippingMethod.ShippingMethod.Name.GetShippingMethodName() : null,
                ShippingAddress = new WellsAddRxModel.WellsShippingAddress
                {
                    ShippingName = $"{order.Patient?.FirstName} {order.Patient?.LastName}",
                    ShippingAddress1 = order.ShippingAddress?.Address?.AddressLine1,
                    ShippingCity = order.ShippingAddress?.Address?.City,
                    ShippingState = order.ShippingAddress?.Address?.State?.Abbreviation,
                    ShippingZip = order.ShippingAddress?.Address?.PostalCode
                }
            };
        }

        private static char GetGenderChar(this Gender gender)
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

        private static string GetShippingMethodName(this string shippingMethod)
        {
            switch (shippingMethod.ToLower())
            {
                case "pick up":
                    return "Pick";
                default:
                    return "FEXP";
            }
        }
    }
}

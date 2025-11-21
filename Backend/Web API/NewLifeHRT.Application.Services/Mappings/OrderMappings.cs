using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class OrderMappings
    {
        public static OrderResponseDto ToOrderResponseDto(this Order order)
        {

            return new OrderResponseDto
            {
                Id = order.Id,
                Name = order.Name,
                PatientId = order.PatientId,
                PharmacyId = order.PharmacyId,
                PhysicianId = order.PhysicianId ?? order.Patient?.AssignPhysicianId,
                CounselorId = order.CounselorId,
                CouponId = order.CouponId,
                PatientCreditCardId = order.PatientCreditCardId,
                PharmacyShippingMethodId = order.PharmacyShippingMethodId,
                ShippingAddressId = order.ShippingAddressId,
                TherapyExpiration = order.TherapyExpiration,
                LastOfficeVisit = order.LastOfficeVisit,
                Subtotal = order.Subtotal,
                TotalAmount = order.TotalAmount,
                Surcharge = order.Surcharge,
                CouponDiscount = order.CouponDiscount,
                Commission = order.Commission,
                TotalOnCommissionApplied = order.TotalOnCommissionApplied,
                DeliveryCharge = order.DeliveryCharge,
                IsOrderPaid = order.IsOrderPaid,
                IsCashPayment = order.IsCashPayment,
                IsGenrateCommision = order.IsGenrateCommision,
                IsReadyForLifeFile = order.IsReadyForLifeFile,
                OrderPaidDate = order.OrderPaidDate,
                OrderFulFilled = order.OrderFulFilled,
                RejectionResaon = order.RejectionResaon,
                Description = order.Description,
                Signed = order.Signed,
                Status = (int)order.Status,
                IsActive = order.IsActive,
                IsPharmacyActive = order.Pharmacy?.IsActive,
                CourierServiceId = order.CourierServiceId,
                TrackingNumber = order.TrackingNumber,
                PharmacyOrderNumber = order.PharmacyOrderNumber,
                RefundAmount = order.RefundAmount,
                SettledAmount = order.SettledAmount,
                LastSettlementDate = order.LastSettlementDate,
                CreatedAt = order.CreatedAt,
                OrderNumber = order.OrderNumber,
                OrderDetails = order.OrderDetails?.Select(od => new OrderDetailResponseDto
                {
                    Id = od.Id,
                    ProductId = od.ProductId,
                    ProductPharmacyPriceListItemId = od.ProductPharmacyPriceListItemId,
                    ProductName = od.Product?.Name,
                    Quantity = od.Quantity,
                    IsColdStorageProduct = od.Product?.IsColdStorageProduct,
                    PerUnitAmount = od.PerUnitAmount,
                    Amount = od.Amount,
                    ProductType = od.Product?.Type?.Name,
                    Protocol = od?.Protocol,
                    IsActive = od.Product?.IsActive,

                }).ToList() ?? new List<OrderDetailResponseDto>(),
            };
        }


        public static List<OrderBulkResponseDto> ToOrderBulkResponseDtoList(this IEnumerable<Order> orders)
        {
            return orders.Select(p => p.ToOrderBulkResponseDto()).ToList();
        }

        public static OrderBulkResponseDto ToOrderBulkResponseDto(this Order order)
        {
            return new OrderBulkResponseDto
            {
                Id = order.Id,
                Name = order.Name ?? string.Empty,
                PatientName = $"{order.Patient?.FirstName ?? ""} {order.Patient?.LastName ?? ""}".Trim(),
                PharmacyName = order.Pharmacy?.Name ?? string.Empty,
                Status = (int?)order.Status,
                CounselorName = $"{order.Counselor?.FirstName ?? ""} {order.Counselor?.LastName ?? ""}".Trim(),
                TherapyExpiration = order.TherapyExpiration,
                CreatedAt = order.CreatedAt,
            };
        }
        public static OrderRequestDto ToOrder(this Proposal proposal)
        {
            return new OrderRequestDto
            {
                ProposalId = proposal.Id,
                Name = proposal.Name,
                PatientId = proposal.PatientId,
                PharmacyId = proposal.PharmacyId,
                PhysicianId = proposal.PhysicianId,
                CounselorId = proposal.CounselorId,
                CouponId = proposal.CouponId,
                PatientCreditCardId = proposal.PatientCreditCardId,
                PharmacyShippingMethodId = proposal.PharmacyShippingMethodId,
                ShippingAddressId = proposal.ShippingAddressId,
                TherapyExpiration = proposal.TherapyExpiration,
                Subtotal = proposal.Subtotal,
                TotalAmount = proposal.TotalAmount,
                CouponDiscount = proposal.CouponDiscount,
                Surcharge = proposal.Surcharge,
                DeliveryCharge = proposal.DeliveryCharge,
                Status = (int)OrderStatus.New,
                Description = proposal.Description,
                IsDeliveryChargeOverRidden = proposal.IsDeliveryChargeOverRidden,
                IsOrderPaid = null,
                IsCashPayment = null,
                IsGenrateCommision = null,
                IsReadyForLifeFile = null,
                OrderDetails = proposal.ProposalDetails?.Select(od => new OrderDetailRequestDto
                {
                    ProductId = od.ProductId,
                    ProductPharmacyPriceListItemId = od.ProductPharmacyPriceListItemId,
                    Protocol = od.Protocol,
                    Quantity = od.Quantity,
                    PerUnitAmount = od.PerUnitAmount,
                    IsPriceOverRidden = od.IsPriceOverRidden,
                    Amount = od.Amount,
                    
                }).ToList() ?? new List<OrderDetailRequestDto>(),
            };
        }
        public static OrderReceiptResponseDto ToOrderReceiptResponseDto(this Order order)
        {
            return new OrderReceiptResponseDto
            {
                Id = order.Id,
                Name = order.Name,
                PatientName = (order.Patient?.FirstName ?? string.Empty) + " " + (order.Patient?.LastName ?? string.Empty),
                DoctorName = (order.Counselor?.FirstName ?? string.Empty) + " " + (order.Counselor?.LastName ?? string.Empty),
                DateOfBirth = order.Patient?.DateOfBirth,
                PhoneNumber = order.Patient?.PhoneNumber,
                DrivingLicence = order.Patient?.DrivingLicence,
                Allergies = order.Patient?.Allergies,
                ShippingMethodName = order.PharmacyShippingMethod?.ShippingMethod?.Name,
                ShippingMethodAmount = order.DeliveryCharge,
                Description = order.Description,
                Signed = order.Signed,
                Subtotal = order.Subtotal,
                TotalAmount = order.TotalAmount,
                LastOfficeVisit = order.LastOfficeVisit,
                Surcharge = order.Surcharge,
                PatientShippingAddress = new ShippingAddressResponseDto
                {
                    AddressType = order?.ShippingAddress?.Address?.AddressType,
                    AddressLine1 = order?.ShippingAddress?.Address?.AddressLine1,
                    City = order?.ShippingAddress?.Address?.City,
                    StateId = order?.ShippingAddress?.Address?.StateId,
                    PostalCode = order?.ShippingAddress?.Address?.PostalCode,

                },
                DoctorShippingAddress = new ShippingAddressResponseDto
                {
                    AddressType = order?.Counselor?.Address?.AddressType,
                    AddressLine1 = order?.ShippingAddress?.Address?.AddressLine1,
                    City = order?.ShippingAddress?.Address?.City,
                    StateId = order?.ShippingAddress?.Address?.StateId,
                    PostalCode = order?.ShippingAddress?.Address?.PostalCode,
                },
                CreatedAt = order.CreatedAt,
                OrderDetails = order.OrderDetails?.Select(od => new OrderDetailResponseDto
                {
                    Id = od.Id,
                    ProductId = od.ProductId,
                    ProductName = od.Product?.Name,
                    Quantity = od.Quantity,
                    IsColdStorageProduct = od.Product?.IsColdStorageProduct,
                    PerUnitAmount = od.PerUnitAmount,
                    Amount = od.Amount,
                    ProductType = od.Product?.Type?.Name,
                    Protocol = od.Protocol,
                    IsActive = od.Product?.IsActive ?? false
                }).ToList() ?? new List<OrderDetailResponseDto>()
            };
        }
        public static OrderResponseDto ToFullOrderResponseDto(this Order order)
        {

            return new OrderResponseDto
            {
                Id = order.Id,
                Name = order.Name,
                PatientId = order.PatientId,
                PharmacyId = order.PharmacyId,
                //PhysicianId = order.PhysicianId ?? order.Patient?.AssignPhysicianId,
                CounselorId = order.CounselorId,
                CouponId = order.CouponId,
                //PatientCreditCardId = order.PatientCreditCardId,
                //PharmacyShippingMethodId = order.PharmacyShippingMethodId,
                //ShippingAddressId = order.ShippingAddressId,
                //TherapyExpiration = order.TherapyExpiration,
                //LastOfficeVisit = order.LastOfficeVisit,
                Subtotal = order.Subtotal,
                TotalAmount = order.TotalAmount,
                Surcharge = order.Surcharge,
                CouponDiscount = order.CouponDiscount,
                Commission = order.Commission,
                TotalOnCommissionApplied = order.TotalOnCommissionApplied,
     //           CommissionGeneratedDate = order.CommissionGeneratedDate,
                DeliveryCharge = order.DeliveryCharge,
                IsOrderPaid = order.IsOrderPaid,
                IsCashPayment = order.IsCashPayment,
                IsGenrateCommision = order.IsGenrateCommision,
               // IsReadyForLifeFile = order.IsReadyForLifeFile,
                OrderPaidDate = order.OrderPaidDate,
                OrderFulFilled = order.OrderFulFilled,
                RejectionResaon = order.RejectionResaon,
                Description = order.Description,
                //Signed = order.Signed,
                Status = (int)order.Status,
                IsPharmacyActive = order.Pharmacy?.IsActive,
                CourierServiceId = order.CourierServiceId,
                TrackingNumber = order.TrackingNumber,
                PharmacyOrderNumber = order.PharmacyOrderNumber,
                CreatedAt = order.CreatedAt,
                OrderDetails = order.OrderDetails?.Select(od => new OrderDetailResponseDto
                {
                    Id = od.Id,
                    ProductId = od.ProductId,
                    ProductPharmacyPriceListItemId = od.ProductPharmacyPriceListItemId,
                    ProductName = od.Product?.Name,
                    Quantity = od.Quantity,
                    IsColdStorageProduct = od.Product?.IsColdStorageProduct,
                    PerUnitAmount = od.PerUnitAmount,
                    Amount = od.Amount,
                    //ProductType = od.Product?.Type?.Name,
                    //Protocol = od?.Protocol,
                    IsActive = od.Product?.IsActive,

                }).ToList() ?? new List<OrderDetailResponseDto>(),
            };
        }

        public static OrderTemplateReceiptResponseDto ToOrderTemplateReceiptResponseDto(this Order order)
        {
            return new OrderTemplateReceiptResponseDto
            {
                Id = order.Id,
                Name = order.Name,
                Number = order.OrderNumber,
                PatientName = (order.Patient?.FirstName ?? string.Empty) + " " + (order.Patient?.LastName ?? string.Empty),
                DoctorName = (order.Physician?.FirstName ?? string.Empty) + " " + (order.Physician?.LastName ?? string.Empty),
                DateOfBirth = order.Patient?.DateOfBirth,
                PhoneNumber = order.Patient?.PhoneNumber,
                DrivingLicence = order.Patient?.DrivingLicence,
                Allergies = order.Patient?.Allergies,
                ShippingMethodName = order.PharmacyShippingMethod?.ShippingMethod?.Name,
                ShippingMethodAmount = order.DeliveryCharge,
                Description = order.Description,
                Signed = order.Signed,
                Subtotal = order.Subtotal,
                TotalAmount = order.TotalAmount,
                LastOfficeVisit = order.LastOfficeVisit,
                Surcharge = order.Surcharge,
                PatientShippingAddress = new ShippingAddressTemplateResponseDto
                {
                    AddressType = order?.ShippingAddress?.Address?.AddressType,
                    AddressLine1 = order?.ShippingAddress?.Address?.AddressLine1,
                    City = order?.ShippingAddress?.Address?.City,
                    StateOrProvince = order?.ShippingAddress?.Address?.State?.Abbreviation,
                    Country = order?.ShippingAddress?.Address?.Country?.Name,
                    PostalCode = order?.ShippingAddress?.Address?.PostalCode,

                },
                DoctorShippingAddress = new ShippingAddressTemplateResponseDto
                {
                    AddressType = order?.Physician?.Address?.AddressType,
                    AddressLine1 = order?.Physician?.Address?.AddressLine1,
                    City = order?.Physician?.Address?.City,
                    StateOrProvince = order?.Physician?.Address?.State?.Abbreviation,
                    Country = order?.Physician?.Address?.Country?.Name,
                    PostalCode = order?.Physician?.Address?.PostalCode,
                },
                CreatedAt = order.CreatedAt,
                OrderDetails = order.OrderDetails?.Select(od => new OrderDetailTemplateResponseDto
                {
                    Id = od.Id,
                    ProductId = od.ProductId,
                    ProductName = od.Product?.Name,
                    Quantity = od.Quantity,
                    IsColdStorageProduct = od.Product?.IsColdStorageProduct,
                    PerUnitAmount = od.PerUnitAmount,
                    Amount = od.Amount,
                    ProductType = od.Product?.Type?.Name,
                    Protocol = od.Protocol,
                    IsActive = od.Product?.IsActive ?? false
                }).ToList() ?? new List<OrderDetailTemplateResponseDto>()
            };
        }

    }
}

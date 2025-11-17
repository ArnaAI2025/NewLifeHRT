using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Mappings;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Common.Helpers;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Enums;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Infrastructure.Data;
using NewLifeHRT.Infrastructure.Generators.Interfaces;
using NewLifeHRT.Infrastructure.Settings;

namespace NewLifeHRT.Application.Services.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailService _orderDetailService;
        private readonly IShippingAddressService _shippingAddressService;
        private readonly IPharmacyShippingMethodService _pharmacyShippingMethodService;
        private readonly ICommissionsPayableService _commissionsPayableService;
        private readonly IPoolService _poolService;
        private readonly ITemplateContentGenerator _templateContentGenerator;
        private readonly IPdfConverter _pdfConverter;
        private readonly AzureBlobStorageSettings _azureBlobStorageSettings;
        private readonly ClinicInformationSettings _clinicInformationSettings;
        private readonly IPatientCreditCardRepository _patientCreditCardRepository;
        private readonly ClinicDbContext _context;
        private readonly ICourierServiceRepository _courierServiceRepository;

        public OrderService(IOrderRepository orderRepository, IOrderDetailService orderDetailService, IShippingAddressService shippingAddressService, IPharmacyShippingMethodService pharmacyShippingMethodService, ICommissionsPayableService commissionsPayableService, IPoolService poolService, IPatientCreditCardRepository patientCreditCardRepository, ClinicDbContext clinicDbContext, ITemplateContentGenerator templateContentGenerator, IPdfConverter pdfConverter, IOptions<AzureBlobStorageSettings> azureBlobStorageSettings, IOptions<ClinicInformationSettings> clinicInformationSettings, ICourierServiceRepository courierServiceRepository)
        {
            _orderRepository = orderRepository;
            _orderDetailService = orderDetailService;
            _shippingAddressService = shippingAddressService;
            _pharmacyShippingMethodService = pharmacyShippingMethodService;
            _commissionsPayableService = commissionsPayableService;
            _poolService = poolService;
            _templateContentGenerator = templateContentGenerator;
            _pdfConverter = pdfConverter;
            _azureBlobStorageSettings = azureBlobStorageSettings.Value;
            _clinicInformationSettings = clinicInformationSettings.Value;
            _patientCreditCardRepository = patientCreditCardRepository;
            _context = clinicDbContext;
            _courierServiceRepository = courierServiceRepository;
        }

        public async Task<List<OrderBulkResponseDto>> GetAllAsync(Guid? patientId = null)
        {
            var includes = new[] { "Patient", "Pharmacy", "Counselor" };
            var query = _orderRepository.Query();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            if (patientId.HasValue)
            {
                query = query.Where(o => o.PatientId == patientId.Value);
            }

            var orders = await query.ToListAsync();
            if (orders == null) return new List<OrderBulkResponseDto>();

            return orders.ToOrderBulkResponseDtoList();
        }



        public async Task<OrderResponseDto?> GetOrderByIdAsync(Guid orderId)
        {
            var includes = new[] { "OrderDetails", "OrderDetails.Product", "OrderDetails.Product.Type", "Patient", "Pharmacy", "Counselor", "Physician", "CourierService" };

            var order = await _orderRepository.GetWithIncludeAsync(orderId, includes);
            if (order == null) return null;
            return order.ToOrderResponseDto();
        }
        public async Task<Order?> GetOrderByIdForProposalAsync(Guid orderId)
        {
            var includes = new[] { "OrderDetails.ProductPharmacyPriceListItem", "OrderDetails.ProductPharmacyPriceListItem.Product" };

            var order = await _orderRepository.GetWithIncludeAsync(orderId, includes);
            if (order == null) return null;
            return order;
        }


        public async Task<CommonOperationResponseDto<Guid>> CreateOrderAsync(OrderRequestDto dto, int userId)
        {
            try
            {
                decimal? originalShippingPrice = null;

                if (dto.PharmacyShippingMethodId.HasValue)
                {
                    originalShippingPrice = await _pharmacyShippingMethodService
                        .GetShippingMethodPriceAsync(dto.PharmacyShippingMethodId.Value);
                }

                var order = new Order
                {
                    ProposalId = dto.ProposalId != null ? dto.ProposalId : null,
                    Name = dto.Name,
                    PatientId = dto.PatientId,
                    PharmacyId = dto.PharmacyId,
                    PhysicianId = dto.PhysicianId,
                    CounselorId = dto.CounselorId,
                    CouponId = dto.CouponId,
                    PatientCreditCardId = dto.PatientCreditCardId,
                    PharmacyShippingMethodId = dto.PharmacyShippingMethodId,
                    ShippingAddressId = dto.ShippingAddressId,
                    TherapyExpiration = dto.TherapyExpiration,
                    LastOfficeVisit = dto.LastOfficeVisit,
                    Subtotal = dto.Subtotal,
                    TotalAmount = dto.TotalAmount,
                    CouponDiscount = dto.CouponDiscount,
                    Surcharge = dto.Surcharge,
                    DeliveryCharge = dto.DeliveryCharge,
                    Status = (OrderStatus)dto.Status,
                    Signed = dto.Signed,
                    Description = dto.Description,
                    RejectionResaon = dto.RejectionResaon,
                    IsOrderPaid = dto.IsOrderPaid,
                    IsCashPayment = dto.IsCashPayment,
                    //IsGenrateCommision = dto.IsGenrateCommision,
                    IsReadyForLifeFile = dto.IsReadyForLifeFile,
                    OrderFulFilled = dto.OrderFulFilled,
                    OrderPaidDate = dto.OrderPaidDate,
                    PharmacyOrderNumber = dto.PharmacyOrderNumber,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userId.ToString(),
                    IsActive = true,
                    CourierServiceId = dto.CourierServiceId,
                    OrderNumber = await GenerateUniqueOrderNumberAsync(),
                    IsDeliveryChargeOverRidden = dto.PharmacyShippingMethodId.HasValue &&
                                                originalShippingPrice.HasValue &&
                                                dto.DeliveryCharge != originalShippingPrice.Value,
                    //CommissionGeneratedDate = dto.IsGenrateCommision != null && dto.IsGenrateCommision == true ? DateTime.UtcNow : null,

                };

                var createdOrder = await _orderRepository.AddAsync(order);

                //Default Select Credit Card
                if (dto.PatientCreditCardId.HasValue)
                {
                    var patientId = dto.PatientId;
                    var selectedCardId = dto.PatientCreditCardId.Value;

                    var existingDefaultCard = await _patientCreditCardRepository
                        .GetSingleAsync(x => x.PatientId == patientId && (x.IsDefaultCreditCard == true) && x.IsActive);

                    if (existingDefaultCard == null || existingDefaultCard.Id != selectedCardId)
                    {
                        var allCards = await _patientCreditCardRepository
                            .FindAsync(x => x.PatientId == patientId && x.IsActive);

                        foreach (var card in allCards)
                        {
                            card.IsDefaultCreditCard = (card.Id == selectedCardId);
                            card.UpdatedBy = userId.ToString();
                            card.UpdatedAt = DateTime.UtcNow;
                        }

                        await _patientCreditCardRepository.BulkUpdateAsync(allCards.ToList());
                    }
                }

                if (dto.OrderDetails?.Any() == true)
                {
                    await _orderDetailService.CreateOrderDetailAsync(createdOrder.Id, dto.OrderDetails, userId);
                }
                if (dto.ShippingAddressId != null)
                {
                    await _shippingAddressService.SetDefaultAsync(createdOrder.PatientId, (Guid)dto.ShippingAddressId, userId);
                }

                return new CommonOperationResponseDto<Guid>
                {
                    Id = createdOrder.Id,
                    Message = "Order successfully created"
                };
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<BulkOperationResponseDto> UpdateOrderAsync(Guid id, OrderRequestDto dto, int userId)
        {
            ArgumentNullException.ThrowIfNull(dto);
            var response = new BulkOperationResponseDto();

            try
            {
                var existingOrder = await _orderRepository.GetByIdAsync(id);
                if (existingOrder == null)
                {
                    response.FailedCount = 1;
                    response.FailedIds.Add(id.ToString());
                    response.Message = "Order not found.";
                    return response;
                }
                decimal? originalShippingPrice = null;
                if (dto.PharmacyShippingMethodId.HasValue)
                {
                    originalShippingPrice = await _pharmacyShippingMethodService
                        .GetShippingMethodPriceAsync(dto.PharmacyShippingMethodId.Value);
                }
                // bool isCommissionChanged = existingOrder.IsGenrateCommision != dto.IsGenrateCommision;

                existingOrder.Name = dto.Name;
                existingOrder.PatientId = dto.PatientId;
                existingOrder.PharmacyId = dto.PharmacyId;
                existingOrder.PhysicianId = dto.PhysicianId;
                existingOrder.CounselorId = dto.CounselorId;
                existingOrder.CouponId = dto.CouponId;
                existingOrder.PatientCreditCardId = dto.PatientCreditCardId;
                existingOrder.PharmacyShippingMethodId = dto.PharmacyShippingMethodId;
                existingOrder.ShippingAddressId = dto.ShippingAddressId;
                existingOrder.TherapyExpiration = dto.TherapyExpiration;
                existingOrder.LastOfficeVisit = dto.LastOfficeVisit;
                existingOrder.Subtotal = dto.Subtotal;
                existingOrder.TotalAmount = dto.TotalAmount;
                existingOrder.CouponDiscount = dto.CouponDiscount;
                existingOrder.Surcharge = dto.Surcharge;
                existingOrder.DeliveryCharge = dto.DeliveryCharge;
                existingOrder.Status = (OrderStatus)dto.Status;
                existingOrder.Commission = dto.Commission;
                existingOrder.TotalOnCommissionApplied = dto.TotalOnCommissionApplied;
                existingOrder.Signed = dto.Signed;
                existingOrder.Description = dto.Description;
                existingOrder.RejectionResaon = dto.RejectionResaon;
                existingOrder.IsOrderPaid = dto.IsOrderPaid;
                existingOrder.IsCashPayment = dto.IsCashPayment;
                //existingOrder.IsGenrateCommision = dto.IsGenrateCommision;
                //   existingOrder.CommissionGeneratedDate = dto.IsGenrateCommision == true ? DateTime.UtcNow : null;
                existingOrder.IsReadyForLifeFile = dto.IsReadyForLifeFile;
                existingOrder.OrderFulFilled = dto.OrderFulFilled;
                existingOrder.OrderPaidDate = dto.OrderPaidDate;
                existingOrder.PharmacyOrderNumber = dto.PharmacyOrderNumber;
                existingOrder.CourierServiceId = dto.CourierServiceId;
                existingOrder.UpdatedAt = DateTime.UtcNow;
                existingOrder.UpdatedBy = userId.ToString();
                existingOrder.IsDeliveryChargeOverRidden = dto.PharmacyShippingMethodId.HasValue &&
                                                  originalShippingPrice.HasValue &&
                                                  dto.DeliveryCharge != originalShippingPrice.Value;
                //if (isCommissionChanged)
                //{
                //    if (dto.IsGenrateCommision == true)
                //    {
                //        existingOrder.CommissionGeneratedDate = DateTime.UtcNow;
                //    }
                //    else
                //    {
                //        existingOrder.CommissionGeneratedDate = null;
                //    }
                //}
                await _orderRepository.UpdateAsync(existingOrder);

                //Default Select Credit Card
                if (dto.PatientCreditCardId.HasValue)
                {
                    var patientId = dto.PatientId;
                    var selectedCardId = dto.PatientCreditCardId.Value;

                    var existingDefaultCard = await _patientCreditCardRepository
                        .GetSingleAsync(x => x.PatientId == patientId && (x.IsDefaultCreditCard == true) && x.IsActive);

                    if (existingDefaultCard == null || existingDefaultCard.Id != selectedCardId)
                    {
                        var allCards = await _patientCreditCardRepository
                            .FindAsync(x => x.PatientId == patientId && x.IsActive);

                        foreach (var card in allCards)
                        {
                            card.IsDefaultCreditCard = (card.Id == selectedCardId);
                            card.UpdatedBy = userId.ToString();
                            card.UpdatedAt = DateTime.UtcNow;
                        }

                        await _patientCreditCardRepository.BulkUpdateAsync(allCards.ToList());
                    }
                }

                var detailResponse = await _orderDetailService.UpdateOrderDetailAsync(id, dto.OrderDetails, userId);
                if (dto.ShippingAddressId != null)
                {
                    await _shippingAddressService.SetDefaultAsync(existingOrder.PatientId, (Guid)dto.ShippingAddressId, userId);
                }
                response.SuccessCount = 1 + detailResponse.SuccessCount;
                response.FailedCount = detailResponse.FailedCount;
                response.SuccessIds.Add(existingOrder.Id.ToString());
                response.SuccessIds.AddRange(detailResponse.SuccessIds);
                response.FailedIds.AddRange(detailResponse.FailedIds);
                response.Message = detailResponse.FailedCount == 0
                    ? "Order and all details updated successfully."
                    : "Order updated, but some details failed.";

                return response;
            }
            catch (Exception ex)
            {
                response.FailedCount = 1 + (dto.OrderDetails?.Count ?? 0);
                response.FailedIds.Add(id.ToString());
                if (dto.OrderDetails != null)
                    response.FailedIds.AddRange(dto.OrderDetails.Select(d => d.ProductId.ToString()));
                response.Message = $"Failed to update order. Error: {ex.Message}";
                return response;
            }
        }


        public async Task<BulkOperationResponseDto> BulkDeleteOrdersAsync(IList<Guid> orderIds)
        {
            if (orderIds == null || !orderIds.Any())
            {
                return new BulkOperationResponseDto
                {
                    SuccessCount = 0,
                    FailedCount = 0,
                    Message = "No valid order IDs provided."
                };
            }

            var ordersToDelete = (await _orderRepository.FindAsync(o => orderIds.Contains(o.Id), noTracking: false)).ToList();

            if (!ordersToDelete.Any())
            {
                return new BulkOperationResponseDto
                {
                    SuccessCount = 0,
                    FailedCount = orderIds.Count,
                    Message = "No orders found for the provided IDs."
                };
            }

            await _orderRepository.RemoveRangeAsync(ordersToDelete);
            await _orderRepository.SaveChangesAsync();

            var successCount = ordersToDelete.Count;
            var failedCount = orderIds.Count - successCount;

            return new BulkOperationResponseDto
            {
                SuccessCount = successCount,
                FailedCount = failedCount,
                Message = $"{successCount} order(s) deleted successfully."
            };
        }
        public async Task<BulkOperationResponseDto> BulkDeleteAsync(IList<Guid> orderIds)
        {
            if (orderIds == null || !orderIds.Any())
            {
                return new BulkOperationResponseDto
                {
                    SuccessCount = 0,
                    FailedCount = 0,
                    Message = "No valid order IDs provided."
                };
            }

            var proposalsToDelete = (await _orderRepository.FindAsync(p => orderIds.Contains(p.Id), noTracking: false)).ToList();
            if (!proposalsToDelete.Any())
            {
                return new BulkOperationResponseDto
                {
                    SuccessCount = 0,
                    FailedCount = orderIds.Count,
                    Message = "No order found for the provided IDs."
                };
            }
            await _orderRepository.RemoveRangeAsync(proposalsToDelete);
            await _orderRepository.SaveChangesAsync();

            var successCount = proposalsToDelete.Count;
            var failedCount = orderIds.Count - successCount;

            return new BulkOperationResponseDto
            {
                SuccessCount = successCount,
                FailedCount = failedCount,
                Message = $"{successCount} order(s) deleted successfully."
            };
        }
        /// <summary>
        /// Updates the status of a order
        /// appending rejection reasons to the order RejectionResaon, and if commission is generted then making a reserval entry.
        /// </summary>
        public async Task<CommonOperationResponseDto<Guid?>> UpdateOrderStatusAsync(Guid orderId, int status, string? reasonForRejection, int userId)
        {
            var order = await _orderRepository.GetSingleAsync(
                    o => o.Id == orderId,
                    include: q => q.Include(x => x.OrderDetails)
            );
            if (order == null)
            {
                return new CommonOperationResponseDto<Guid?>
                {
                    Message = "No order found for the provided ID."
                };
            }
            if (!string.IsNullOrEmpty(reasonForRejection))
            {
                order.RejectionResaon = (order.RejectionResaon ?? "") + " " + reasonForRejection;
            }
            if ((OrderStatus)status == OrderStatus.Cancel_noMoney)
            {
                order.RejectionResaon = "Rejection Reason: No Money";
            }
            if (order?.IsGenrateCommision == true && ((OrderStatus)status == OrderStatus.Cancel_noMoney || (OrderStatus)status == OrderStatus.Cancel_rejected))
            {
                await CancelGeneratedCommissionAsync(orderId, userId);
            }
            order.Status = (OrderStatus)status;
            if ((OrderStatus)status == OrderStatus.Completed)
            {
                order.OrderFulFilled = DateTime.UtcNow;

                foreach (var detail in order.OrderDetails)
                {
                    detail.IsReadyForRefillDateCalculation = true;
                }
            }
            order.UpdatedBy = userId.ToString();
            order.UpdatedAt = DateTime.UtcNow;
            await _orderRepository.UpdateAsync(order);
            return new CommonOperationResponseDto<Guid?>
            {
                Id = order.ProposalId,
                Message = "order status modified successfully."
            };
        }
        /// <summary>
        /// genertes pdf receipt.
        /// </summary>
        public async Task<OrderTemplateReceiptResponseDto?> GetReceiptByIdAsync(Guid orderId)
        {
            try
            {
                //var containerUrl = _azureBlobStorageSettings;
                //var data = _clinicInformationSettings;
                var includes = new[] { "OrderDetails", "OrderDetails.Product", "Patient", "PharmacyShippingMethod", "PharmacyShippingMethod.ShippingMethod" };

                var order = await _orderRepository.GetWithIncludeAsync(orderId, includes);
                if (order == null) return null;
                var response = order.ToOrderTemplateReceiptResponseDto();
                response.Logo = $"{_azureBlobStorageSettings.ContainerSasUrl}/{_clinicInformationSettings.Name}/logo.png?{_azureBlobStorageSettings.SasToken}";
                if (!string.IsNullOrWhiteSpace(response.Logo))
                {
                    response.Logo = await ImageHelper.ConvertImageUrlToBase64Async(response.Logo);
                }
                response.TemplatePath = "OrderReceiptTemplate.cshtml";
                var res = _templateContentGenerator.GetTemplateContent(response);
                var pdfResult = _pdfConverter.ConvertToPdf(res);
                return response;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// genertes signed and unsigned report.
        /// </summary>
        public async Task<OrderTemplateReceiptResponseDto?> GetFullOrderByIdAsync(Guid orderId, bool? isScheduleDrug)
        {
            var includes = new[]
            {
        "OrderDetails",
        "OrderDetails.ProductPharmacyPriceListItem",
        "OrderDetails.Product",
        "Patient",
        "ShippingAddress",
        "ShippingAddress.Address.State",
        "PharmacyShippingMethod",
        "PharmacyShippingMethod.ShippingMethod",
        "Physician.Address",
        "Physician.Address.State",
        "Physician.Address.Country",
        "Physician.LicenseInformations",
        "Physician.LicenseInformations.State",
    };

            var order = await _orderRepository.GetWithIncludeAsync(orderId, includes);
            if (order == null) return null;
            int? shippingStateId = order.ShippingAddress?.Address?.StateId;

            var physicianLicense = order.Physician?.LicenseInformations?
                .FirstOrDefault(li =>
                    shippingStateId.HasValue &&
                    li.StateId == shippingStateId.Value
                );

            if (isScheduleDrug.HasValue)
            {
                if (isScheduleDrug.Value)
                {
                    order.OrderDetails = order.OrderDetails
                        .Where(od => od.ProductPharmacyPriceListItem != null
                                  && od.ProductPharmacyPriceListItem.LifeFileScheduledCodeId.HasValue
                                  && od.ProductPharmacyPriceListItem.LifeFileScheduledCodeId.Value > 0)
                        .ToList();
                }
                else
                {
                    order.OrderDetails = order.OrderDetails
                        .Where(od => od.ProductPharmacyPriceListItem == null
                                  || !od.ProductPharmacyPriceListItem.LifeFileScheduledCodeId.HasValue
                                  || od.ProductPharmacyPriceListItem.LifeFileScheduledCodeId.Value == 0)
                        .ToList();
                }
            }

            var response = order.ToOrderTemplateReceiptResponseDto();
            if (!string.IsNullOrWhiteSpace(order?.Physician?.SignaturePath))
            {
                response.Signature = $"{_azureBlobStorageSettings.ContainerSasUrl}/{order?.Physician?.SignaturePath}?{_azureBlobStorageSettings.SasToken}";
                response.Signature = await ImageHelper.ConvertImageUrlToBase64Async(response.Signature);
            }
            response.Logo = $"{_azureBlobStorageSettings.ContainerSasUrl}/{_clinicInformationSettings.Name}/logo.png?{_azureBlobStorageSettings.SasToken}";
            if (!string.IsNullOrWhiteSpace(response.Logo))
            {
                response.Logo = await ImageHelper.ConvertImageUrlToBase64Async(response.Logo);
            }
            response.TemplatePath = "PrescriptionTemplate.cshtml";
            response.IsScheduleDrug = isScheduleDrug;
            response.LicenseNumber = physicianLicense?.Number ?? null;
            var res = _templateContentGenerator.GetTemplateContent(response);
            var pdfResult = _pdfConverter.ConvertToPdf(res);
            return response;
        }

        public async Task<CommonOperationResponseDto<Guid>> GenerateCommission(Guid orderId, int userId)
        {
            var response = new CommonOperationResponseDto<Guid>();

            var order = await _orderRepository.GetByIdAsync(orderId);

            if (order == null)
            {
                response.Message = "Order not found.";
                return response;
            }

            try
            {
                order.IsGenrateCommision = true;
                order.CommissionGeneratedDate = DateTime.UtcNow;
                order.UpdatedAt = DateTime.UtcNow;
                order.UpdatedBy = userId.ToString();

                await _orderRepository.SaveChangesAsync();

                response.Message = "Commission generated successfully.";
                response.Id = order.Id;
            }
            catch (Exception ex)
            {
                response.Message = $"Error generating commission: {ex.Message}";
            }

            return response;
        }

        public async Task<MarkReadyToLifeFileResponseDto> MarkOrderReadyForLifeFileAsync(Guid orderId, int userId)
        {
            var response = new MarkReadyToLifeFileResponseDto();

            var order = await _orderRepository.GetWithIncludeAsync(orderId, new[] { "Pharmacy.Configuration.IntegrationType.IntegrationKeys", "Pharmacy.Configuration.ConfigurationData" });
            if (order == null)
            {
                response.IsSuccess = false;
                response.Message = "Order not found.";
                return response;
            }

            var config = order.Pharmacy?.Configuration;
            if (config == null)
            {
                response.IsSuccess = false;
                response.Message = "Pharmacy configuration not found.";
                return response;
            }

            var integrationKeys = config.IntegrationType?.IntegrationKeys?.ToList();
            if (integrationKeys == null || !integrationKeys.Any())
            {
                response.IsSuccess = false;
                response.Message = "No integration keys defined for this pharmacy type.";
                return response;
            }

            var configData = config.ConfigurationData?.ToList() ?? new List<PharmacyConfigurationData>();

            var missingKeys = integrationKeys
                .Where(k => !configData.Any(cd => cd.KeyId == k.Id && !string.IsNullOrEmpty(cd.Value)))
                .ToList();

            if (missingKeys.Any())
            {
                response.IsSuccess = false;
                response.Message = "Pharmacy configuration data not set for all required keys.";
                return response;
            }

            order.IsReadyForLifeFile = true;
            order.Status = OrderStatus.LifeFileProcessing;
            order.UpdatedAt = DateTime.UtcNow;
            order.UpdatedBy = userId.ToString();

            await _orderRepository.UpdateAsync(order);

            response.Id = order.Id;
            response.IsSuccess = true;
            response.Message = "Order marked as ready for LifeFile.";
            return response;
        }

        public async Task<UpdateOrderPaymentResponseDto> UpdateOrderPaymentAsync(UpdateOrderPaymentRequestDto request, int userId)
        {
            var response = new UpdateOrderPaymentResponseDto();

            try
            {
                var order = await _orderRepository.GetByIdAsync(request.OrderId);
                if (order == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Order not found.";
                    return response;
                }

                order.IsOrderPaid = request.IsOrderPaid;
                order.IsCashPayment = request.IsCashPayment;
                if (request.IsOrderPaid == true || request.IsCashPayment == true)
                {
                    order.OrderPaidDate = DateTime.UtcNow;
                }

                order.UpdatedAt = DateTime.UtcNow;
                order.UpdatedBy = userId.ToString();

                await _orderRepository.UpdateAsync(order);

                response.Id = order.Id;
                response.IsSuccess = true;
                response.Message = "Order payment status updated successfully.";
                response.IsOrderPaid = order.IsOrderPaid;
                response.IsCashPayment = order.IsCashPayment;
                response.OrderPaidDate = order.OrderPaidDate;

                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Failed to update order payment: {ex.Message}";
                return response;
            }
        }
        public async Task<CommonOperationResponseDto<Guid>> CancelGeneratedCommissionAsync(Guid orderId, int userId)
        {
            var order = await _orderRepository.GetSingleAsync(o => o.Id == orderId && o.IsActive == true);

            if (order == null)
                return new CommonOperationResponseDto<Guid> { Message = "Order not found or inactive" };

            if (!order.CommissionGeneratedDate.HasValue)
                return new CommonOperationResponseDto<Guid> { Message = "Commission has not been generated for this order" };

            var (currentPoolFrom, currentPoolTo) = CommissionCalculatorHelper.GetWeekRangeFriThuUtc(DateTime.UtcNow);
            var commissionGenDate = order.CommissionGeneratedDate.Value.Date;
            bool isInCurrentWeek = commissionGenDate >= currentPoolFrom.Date && commissionGenDate <= currentPoolTo.Date;

            var hasGeneratedEntry = await _commissionsPayableService.HasCommissionEntryAsync(orderId, CommissionEntryTypeEnum.Generated);
            var hasReversalEntry = await _commissionsPayableService.HasCommissionEntryAsync(orderId, CommissionEntryTypeEnum.Reversal);

            if (isInCurrentWeek)
            {
                if (hasGeneratedEntry && !hasReversalEntry)
                {
                    var reversalResult = await CreateReversalCommissionAsync(order, userId, currentPoolFrom, currentPoolTo);
                    if (!string.IsNullOrEmpty(reversalResult.Message))
                        return reversalResult;

                    return new CommonOperationResponseDto<Guid>
                    {
                        Message = "Reversal recorded for current week commission.",
                        Id = orderId
                    };
                }

                order.IsGenrateCommision = null;
                order.CommissionGeneratedDate = null;
                order.UpdatedAt = DateTime.UtcNow;
                order.UpdatedBy = userId.ToString();

                await _commissionsPayableService.UpdateStatusCommissionPaybale(orderId, userId);
                await _orderRepository.UpdateAsync(order);
                return new CommonOperationResponseDto<Guid>
                {
                    Message = hasReversalEntry
                        ? "Reversal already exists for this commission; no action taken."
                        : "Commission canceled in the current week.",
                    Id = orderId
                };
            }

            else
            {
                if (hasGeneratedEntry && !hasReversalEntry)
                {
                    var reversalResult = await CreateReversalCommissionAsync(order, userId, currentPoolFrom, currentPoolTo);
                    if (!string.IsNullOrEmpty(reversalResult.Message))
                        return reversalResult;

                    return new CommonOperationResponseDto<Guid>
                    {
                        Message = "Commission reversed successfully.",
                        Id = orderId
                    };
                }

                return new CommonOperationResponseDto<Guid>
                {
                    Message = hasReversalEntry
                        ? "Reversal entry already exists for this commission."
                        : "No generated commission found to reverse for past week.",
                    Id = orderId
                };
            }
        }

        public async Task<CommonOperationResponseDto<Guid>> CreateReversalCommissionAsync(Order order, int userId, DateTime currentPoolFrom, DateTime currentPoolTo)
        {

            var fullOrder = await _orderRepository.GetWithIncludeAsync(order.Id, new[]
        {
        "OrderDetails",
        "OrderDetails.ProductPharmacyPriceListItem",
        "OrderDetails.Product",
        "OrderDetails.Product.CommisionRates",
        "Patient",
        "Pharmacy",
        "Counselor"
    });

            if (fullOrder == null)
                return new CommonOperationResponseDto<Guid> { Message = "Order data missing for reversal" };

            var pool = await _poolService.GetPoolInformationAsync(currentPoolFrom, currentPoolTo, fullOrder.CounselorId);
            if (pool == null || pool.PoolDetails == null || !pool.PoolDetails.Any())
                return new CommonOperationResponseDto<Guid> { Message = "Current week pool detail missing for reversal" };

            var reversedCommission = CommissionCalculatorHelper.CalculateCommission(fullOrder);
            if (reversedCommission == null)
                return new CommonOperationResponseDto<Guid> { Message = "Failed to calculate commission reversal" };

            var relevantPoolDetail = pool.PoolDetails.FirstOrDefault(pd => pd.CounselorId == fullOrder.CounselorId);
            if (relevantPoolDetail == null)
                return new CommonOperationResponseDto<Guid> { Message = "PoolDetail not found for reversal" };

            reversedCommission.Id = Guid.NewGuid();
            reversedCommission.OrderId = order.Id;
            reversedCommission.PoolDetailId = relevantPoolDetail.Id;
            reversedCommission.EntryType = CommissionEntryTypeEnum.Reversal;
            reversedCommission.IsActive = true;
            reversedCommission.CreatedAt = DateTime.UtcNow;
            reversedCommission.CreatedBy = userId.ToString();

            reversedCommission.CommissionsPayablesDetails = reversedCommission.CommissionsPayablesDetails?.Select(d =>
            {
                d.Id = Guid.NewGuid();
                d.CommissionsPayableId = reversedCommission.Id;
                d.IsActive = true;
                d.CreatedAt = DateTime.UtcNow;
                d.CreatedBy = userId.ToString();
                return d;
            }).ToList() ?? new List<CommissionsPayablesDetail>();

            await _commissionsPayableService.InsertAsync(reversedCommission);
            await _orderRepository.SaveChangesAsync();

            return new CommonOperationResponseDto<Guid>
            {
                Message = "Commission reversal recorded successfully",
                Id = order.Id
            };
        }

        public async Task<CommonOperationResponseDto<Guid?>> ProcessRefundAsync(Guid orderId, decimal refundAmount, int userId)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var includes = new[] { "Patient" };
                    var order = await _orderRepository.GetWithIncludeAsync(orderId, includes);

                    if (order == null)
                    {
                        return new CommonOperationResponseDto<Guid?>
                        {
                            Message = "Order not found."
                        };
                    }

                    order.RefundAmount = refundAmount;
                    order.UpdatedBy = userId.ToString();
                    order.UpdatedAt = DateTime.UtcNow;

                    if (order.Patient != null)
                    {
                        order.Patient.OutstandingRefundBalance += refundAmount;
                        order.Patient.UpdatedBy = userId.ToString();
                        order.Patient.UpdatedAt = DateTime.UtcNow;
                    }

                    await _orderRepository.UpdateAsync(order);
                    await transaction.CommitAsync();

                    return new CommonOperationResponseDto<Guid?>
                    {
                        Id = order.Id,
                        Message = "Refund processed successfully."
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new CommonOperationResponseDto<Guid?>
                    {
                        Message = $"Failed to process refund: {ex.Message}"
                    };
                }
            }
        }

        public async Task<CommonOperationResponseDto<Guid?>> SettleOutstandingRefundAsync(Guid orderId, decimal settleAmount, int userId)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var includes = new[] { "Patient" };
                    var order = await _orderRepository.GetWithIncludeAsync(orderId, includes);

                    if (order == null)
                    {
                        return new CommonOperationResponseDto<Guid?>
                        {
                            Message = "Order not found."
                        };
                    }

                    if (settleAmount <= 0)
                    {
                        return new CommonOperationResponseDto<Guid?>
                        {
                            Message = "Settle amount must be greater than zero."
                        };
                    }

                    if (order.Patient == null)
                    {
                        return new CommonOperationResponseDto<Guid?>
                        {
                            Message = "Patient not found for the order."
                        };
                    }


                    var absoluteOutstandingBalance = Math.Abs(order.Patient.OutstandingRefundBalance);


                    if (settleAmount > absoluteOutstandingBalance)
                    {
                        return new CommonOperationResponseDto<Guid?>
                        {
                            Message = "Settle amount cannot exceed the outstanding refund balance."
                        };
                    }
                    order.SettledAmount = order.SettledAmount ?? 0;


                    if (order.Patient.OutstandingRefundBalance < 0)
                    {
                        order.Patient.OutstandingRefundBalance += settleAmount;
                        if (order.SettledAmount < 0)
                        {
                            settleAmount = -1 * settleAmount;
                            order.SettledAmount += settleAmount;
                        }
                        else
                        {
                            order.SettledAmount += settleAmount;
                        }

                    }
                    else
                    {
                        order.Patient.OutstandingRefundBalance -= settleAmount;
                        if (order.SettledAmount < 0)
                        {

                            order.SettledAmount += settleAmount;
                        }
                        else
                        {
                            order.SettledAmount -= settleAmount;
                        }

                    }
                    order.Patient.UpdatedBy = userId.ToString();
                    order.Patient.UpdatedAt = DateTime.UtcNow;

                    order.LastSettlementDate = DateTime.UtcNow;
                    order.UpdatedBy = userId.ToString();
                    order.UpdatedAt = DateTime.UtcNow;

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return new CommonOperationResponseDto<Guid?>
                    {
                        Id = order.Id,
                        Message = "Outstanding refund balance settled successfully."
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new CommonOperationResponseDto<Guid?>
                    {
                        Message = $"Failed to settle outstanding refund: {ex.Message}"
                    };
                }
            }
        }

        public async Task<string> GenerateUniqueOrderNumberAsync()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();

            while (true)
            {
                var buffer = new char[12];
                for (int i = 0; i < buffer.Length; i++)
                    buffer[i] = chars[random.Next(chars.Length)];

                var newOrderNumber = new string(buffer);

                bool exists = await _orderRepository.AnyAsync(o => o.OrderNumber == newOrderNumber);
                if (!exists)
                    return newOrderNumber;
            }
        }

        public async Task<List<CommonDropDownResponseDto<int>>> GetAllCourierServicesAsync()
        {
            var courierServices = await _courierServiceRepository.FindAsync(vt => vt.IsActive);
            return courierServices.ToCourierServiceResponseDtoList();
        }

    }
}

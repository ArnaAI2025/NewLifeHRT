using NewLifeHRT.Application.Services.Interface;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;

public class OrderDetailService : IOrderDetailService
{
    private readonly IOrderDetailRepository _orderDetailRepository;

    public OrderDetailService(IOrderDetailRepository orderDetailRepository)
    {
        _orderDetailRepository = orderDetailRepository;
    }

    public async Task<BulkOperationResponseDto> CreateOrderDetailAsync(Guid orderId, IList<OrderDetailRequestDto> details, int userId)
    {
        int successCount = 0, failedCount = 0;

        foreach (var detailReq in details)
        {
            var orderDetail = new OrderDetail
            {
                OrderId = orderId,
                ProductId = detailReq.ProductId,
                ProductPharmacyPriceListItemId = detailReq.ProductPharmacyPriceListItemId,
                Quantity = detailReq.Quantity,
                PerUnitAmount = detailReq.PerUnitAmount,       
                IsPriceOverRidden = detailReq.IsPriceOverRidden,
                Amount = detailReq.Amount,
                Protocol = detailReq.Protocol,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId.ToString(),
            };
            
                await _orderDetailRepository.AddAsync(orderDetail);
                successCount++;            
        }
        return new BulkOperationResponseDto
        {
            SuccessCount = successCount,
            FailedCount = failedCount
        };
    }

    public async Task<BulkOperationResponseDto> UpdateOrderDetailAsync(Guid orderId, IList<OrderDetailRequestDto> details, int userId)
    {
        var response = new BulkOperationResponseDto();

        try
        {
            details ??= new List<OrderDetailRequestDto>();

            // Load existing rows
            var existingList = (await _orderDetailRepository.FindAsync(od => od.OrderId == orderId))?.ToList()
                               ?? new List<OrderDetail>();

            // Resolve incoming Ids (support Id or OrderDetailId) for deletion pass
            var incomingIds = new HashSet<Guid>(
                details
                    .Select(d => d.Id ?? d.OrderDetailId)
                    .Where(id => id.HasValue && id.Value != Guid.Empty)
                    .Select(id => id!.Value)
            );

            // Upsert pass
            foreach (var detailDto in details)
            {
                try
                {
                    var resolvedId = detailDto.Id ?? detailDto.OrderDetailId;

                    var computedAmount = detailDto.Amount ?? (detailDto.PerUnitAmount * detailDto.Quantity);

                    if (resolvedId.HasValue && resolvedId.Value != Guid.Empty)
                    {
                        var existingDetail = existingList.FirstOrDefault(ed => ed.Id == resolvedId.Value);
                        if (existingDetail != null)
                        {
                            existingDetail.ProductId = detailDto.ProductId;
                            existingDetail.ProductPharmacyPriceListItemId = detailDto.ProductPharmacyPriceListItemId;
                            existingDetail.Quantity = detailDto.Quantity;
                            existingDetail.PerUnitAmount = detailDto.PerUnitAmount;
                            existingDetail.Amount = computedAmount;
                            existingDetail.Protocol = detailDto.Protocol;
                            existingDetail.UpdatedAt = DateTime.UtcNow;
                            existingDetail.UpdatedBy = userId.ToString();

                            await _orderDetailRepository.UpdateAsync(existingDetail);
                            response.SuccessCount++;
                            response.SuccessIds.Add(existingDetail.Id.ToString());
                        }
                        else
                        {
                            response.FailedCount++;
                            response.FailedIds.Add(resolvedId.Value.ToString());
                        }
                    }
                    else
                    {
                        var newDetail = new OrderDetail
                        {
                            Id = Guid.NewGuid(),
                            OrderId = orderId,
                            ProductId = detailDto.ProductId,
                            ProductPharmacyPriceListItemId = detailDto.ProductPharmacyPriceListItemId,
                            Quantity = detailDto.Quantity,
                            PerUnitAmount = detailDto.PerUnitAmount,
                            Amount = computedAmount,
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = userId.ToString(),
                        };

                        await _orderDetailRepository.AddAsync(newDetail);
                        response.SuccessCount++;
                        response.SuccessIds.Add(newDetail.Id.ToString());
                    }
                }
                catch
                {
                    var idStr = (detailDto.Id ?? detailDto.OrderDetailId)?.ToString();
                    response.FailedCount++;
                    response.FailedIds.Add(!string.IsNullOrWhiteSpace(idStr) ? idStr! : "New-Detail");
                }
            }

            // Delete pass: remove existing rows not present in incoming payload
            var toDelete = existingList.Where(e => !incomingIds.Contains(e.Id)).ToList();
            foreach (var del in toDelete)
            {
                try
                {
                    await _orderDetailRepository.DeleteAsync(del);
                    response.SuccessCount++;
                    response.SuccessIds.Add(del.Id.ToString());
                }
                catch
                {
                    response.FailedCount++;
                    response.FailedIds.Add(del.Id.ToString());
                }
            }

            response.Message = response.FailedCount == 0
                ? "All order details updated successfully."
                : "Some order details failed to update.";
            return response;
        }
        catch (Exception ex)
        {
            response.FailedCount = details?.Count ?? 0;
            if (details != null)
            {
                foreach (var d in details)
                {
                    var idOrKey = (d.Id ?? d.OrderDetailId)?.ToString();
                    response.FailedIds.Add(!string.IsNullOrWhiteSpace(idOrKey) ? idOrKey! : "New-Detail");
                }
            }
            response.Message = $"Failed to update order details. Error: {ex.Message}";
            return response;
        }
    }




}

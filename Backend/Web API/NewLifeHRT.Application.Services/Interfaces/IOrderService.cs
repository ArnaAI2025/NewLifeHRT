using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Interfaces
{
    public interface IOrderService
    {
        Task<List<OrderBulkResponseDto>> GetAllAsync(Guid? patientId = null);
        Task<OrderResponseDto?> GetOrderByIdAsync(Guid orderId);
        Task<Order?> GetOrderByIdForProposalAsync(Guid orderId);

        Task<CommonOperationResponseDto<Guid>> CreateOrderAsync(OrderRequestDto dto, int userId);
        Task<BulkOperationResponseDto> UpdateOrderAsync(Guid id, OrderRequestDto dto, int userId);
        Task<BulkOperationResponseDto> BulkDeleteOrdersAsync(IList<Guid> orderIds);
        Task<BulkOperationResponseDto> BulkDeleteAsync(IList<Guid> orderIds);
        Task<CommonOperationResponseDto<Guid?>> UpdateOrderStatusAsync(Guid orderId, int status, string? reasonForRejection, int userId);
        Task<OrderTemplateReceiptResponseDto?> GetReceiptByIdAsync(Guid orderId);
        Task<OrderTemplateReceiptResponseDto?> GetFullOrderByIdAsync(Guid orderId, bool? isScheduledDrug);
        Task<MarkReadyToLifeFileResponseDto> MarkOrderReadyForLifeFileAsync(Guid orderId, int userId);
        Task<UpdateOrderPaymentResponseDto> UpdateOrderPaymentAsync(UpdateOrderPaymentRequestDto request, int userId);
        Task<CommonOperationResponseDto<Guid>> CancelGeneratedCommissionAsync(Guid id, int userId);
        Task<CommonOperationResponseDto<Guid>> CreateReversalCommissionAsync(Order order, int userId, DateTime currentPoolFrom, DateTime currentPoolTo);
            Task<CommonOperationResponseDto<Guid>> GenerateCommission(Guid orderId, int userId);
        //Task<CommissionsPayable> GetFullOrderByIdAsync(Guid orderId);
        Task<CommonOperationResponseDto<Guid?>> ProcessRefundAsync(Guid orderId, decimal refundAmount, int userId);
        Task<CommonOperationResponseDto<Guid?>> SettleOutstandingRefundAsync(Guid orderId, decimal settleAmount, int userId);
    }
}

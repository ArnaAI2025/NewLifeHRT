using Microsoft.EntityFrameworkCore;
using NewLifeHRT.Application.Services.Interface;
using NewLifeHRT.Application.Services.Mappings;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Enums;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Services
{
    public class CommissionsPayableService : ICommissionsPayableService
    {
        private readonly ICommissionsPayableRepository _commissionsPayableRepository;
        private readonly ICommissionsPayableDetailService _commissionsPayableDetailService;
        public CommissionsPayableService(ICommissionsPayableRepository commissionsPayableRepository, ICommissionsPayableDetailService commissionsPayableDetailService)
        {
            _commissionsPayableRepository = commissionsPayableRepository;
            _commissionsPayableDetailService = commissionsPayableDetailService;
        }
        public async Task<List<CommissionsPayableResponse>> GetCommissionByPoolDetailIdAsync(Guid poolDetailId)
        {
            var includes = new[]
            {
        "Order",
        "Order.Patient",
        "Order.Pharmacy",
        //"CommissionsPayablesDetails",
        //"PoolDetail"
    };

            var predicates = new List<Expression<Func<CommissionsPayable, bool>>>
    {
        cp => cp.PoolDetailId == poolDetailId,
        cp => cp.IsActive
    };

            var result = await _commissionsPayableRepository
                .FindWithIncludeAsync(predicates, includes, noTracking: true);

            return result.ToCommissionsPayableResponseList();
        }
        public async Task<CommissionsPayableDetailResponse?> GetCommissionByIdAsync(Guid commissionsPayableId)
        {
            var includes = new[]
            {
        "Order",
        "Order.OrderDetails",
        "Order.Patient",
        "Order.Pharmacy",
        "Order.Counselor",       // add if using Order.Counselor fallback
        "CommissionsPayablesDetails",
        "PoolDetail",
        "PoolDetail.Pool"
    };

            var entity = await _commissionsPayableRepository
                .GetWithIncludeAsync(commissionsPayableId, includes);

            return entity.ToCommissionsPayableDetailResponse();
        }
        public async Task<CommonOperationResponseDto<Guid>> InsertAsync(CommissionsPayable commissionsPayable)
        {
            try
            {
                if (commissionsPayable == null)
                {
                    return new CommonOperationResponseDto<Guid>
                    {
                        Message = "Input cannot be null."
                    };
                }
                await _commissionsPayableRepository.AddAsync(commissionsPayable);

                CommonOperationResponseDto<Guid> detailResponse = new CommonOperationResponseDto<Guid>();

                if (commissionsPayable.CommissionsPayablesDetails != null)
                {
                    var response = await _commissionsPayableDetailService.InsertAsync(commissionsPayable.CommissionsPayablesDetails);
                }
                return new CommonOperationResponseDto<Guid>
                {
                    Id = commissionsPayable.Id,
                    Message = "Commissions Payable and its details added successfully."
                };
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<CommonOperationResponseDto<Guid>> UpdateStatusCommissionPaybale(Guid orderId, int userId)
        {
            var commissions = await _commissionsPayableRepository
                .FindWithIncludeAsync(
                    new List<Expression<Func<CommissionsPayable, bool>>>
                    {
                    cp => cp.OrderId == orderId && cp.IsActive
                    },
                    new[] { "CommissionsPayablesDetails" }
                );

            if (commissions == null || !commissions.Any())
            {
                return new CommonOperationResponseDto<Guid>
                {
                    Id = orderId,
                    Message = "No active commissions found for this order."
                };
            }

            foreach (var commission in commissions)
            {
                commission.IsActive = false;
                commission.UpdatedAt = DateTime.UtcNow;
                commission.UpdatedBy = userId.ToString();

                if (commission.CommissionsPayablesDetails != null && commission.CommissionsPayablesDetails.Any())
                {
                    foreach (var detail in commission.CommissionsPayablesDetails)
                    {
                        detail.IsActive = false;
                        detail.UpdatedAt = DateTime.UtcNow;
                        detail.UpdatedBy = userId.ToString();
                    }
                }
            }

            await _commissionsPayableRepository.BulkUpdateAsync(commissions.ToList());
            await _commissionsPayableRepository.SaveChangesAsync();

            return new CommonOperationResponseDto<Guid>
            {
                Id = orderId,
                Message = "Commission and related details deactivated successfully."
            };
        }
        public async Task<bool> HasCommissionEntryAsync(Guid orderId, CommissionEntryTypeEnum entryType)
        {
            return await _commissionsPayableRepository.AnyAsync(
                cp => cp.OrderId == orderId &&
                      cp.IsActive &&
                      cp.EntryType == entryType);
        }


    }
}

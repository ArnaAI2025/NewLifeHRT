using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;

namespace NewLifeHRT.Application.Services.Services
{
    public class CommissionsPayableDetailService : ICommissionsPayableDetailService
    {
        private readonly ICommissionsPayableDetailRepository _commissionsPayableDetailRepository;
        public CommissionsPayableDetailService(ICommissionsPayableDetailRepository commissionsPayableDetailRepository)
        {
            _commissionsPayableDetailRepository = commissionsPayableDetailRepository;
        }

        public async Task<CommonOperationResponseDto<Guid>> InsertAsync(ICollection<CommissionsPayablesDetail> commissionsPayablesDetails)
        {
            if (commissionsPayablesDetails == null || !commissionsPayablesDetails.Any())
            {
                return new CommonOperationResponseDto<Guid>
                {
                    Message = "Input cannot be null or empty."
                };
            }

                var detailsList = commissionsPayablesDetails as List<CommissionsPayablesDetail> ?? commissionsPayablesDetails.ToList();

                await _commissionsPayableDetailRepository.AddRangeAsync(detailsList);
                return new CommonOperationResponseDto<Guid>
                {
                    Message = "Commission Details added Successfully"
                };
        }
    }
}

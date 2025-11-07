using NewLifeHRT.Application.Services.Interface;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

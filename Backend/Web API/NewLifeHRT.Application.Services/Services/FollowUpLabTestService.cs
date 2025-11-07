using NewLifeHRT.Application.Services.Interface;
using NewLifeHRT.Application.Services.Mappings;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Services
{
    public class FollowUpLabTestService : IFollowUpLabTestService
    {
        private readonly IFollowUpLabTestRepository _followUpLabTestRepository;
        public FollowUpLabTestService(IFollowUpLabTestRepository followUpLabTestRepository)
        {
            _followUpLabTestRepository = followUpLabTestRepository;
        }

        public async Task<List<DropDownIntResponseDto>> GetAllFollowUpTestAsync()
        {
            var followUpLabTests = await this._followUpLabTestRepository.GetAllAsync();
            return DropDownFollowUpLabTestMappings.ToDropDownIntResponseDtoList(followUpLabTests);
        }
    }
}

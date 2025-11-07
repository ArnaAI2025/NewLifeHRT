using NewLifeHRT.Application.Services.Interface;
using NewLifeHRT.Application.Services.Mappings;
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
    public class VisitTypeService : IVisitTypeService
    {
        private readonly IVisitTypeRepository _visitTypeRepository;
        public VisitTypeService(IVisitTypeRepository visitTypeRepository) {
            _visitTypeRepository = visitTypeRepository;
        }
        public async Task<List<CommonDropDownResponseDto<int>>> GetAllAsync()
        {
            var visitTypes = await _visitTypeRepository.FindAsync(vt => vt.IsActive);
            return visitTypes.ToVisitTypeResponseDtoResponseDtoList();
        }

    }
}

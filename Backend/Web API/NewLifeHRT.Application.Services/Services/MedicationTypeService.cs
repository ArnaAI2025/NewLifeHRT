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
    public class MedicationTypeService : IMedicationTypeService
    {
        private readonly IMedicationTypeRepository _medicationTypeRepository;
        public MedicationTypeService(IMedicationTypeRepository medicationTypeRepository)
        {
            _medicationTypeRepository = medicationTypeRepository;
        }

        public async Task<List<DropDownIntResponseDto>> GetAllMedicationTypeAsync()
        { 
            var medicationType = await this._medicationTypeRepository.GetAllAsync();
            return DropDownMedicationTypeMappings.ToDropDownIntResponseDtoList(medicationType);
        }
    }
}

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
    public class LifeFileDrugFormService : ILifeFileDrugFormService
    {
        private readonly ILifeFileDrugFormRepository _lifeFileDrugFormRepository;
        public LifeFileDrugFormService(ILifeFileDrugFormRepository lifeFileDrugFormRepository)
        {
            _lifeFileDrugFormRepository = lifeFileDrugFormRepository;
        }

        public async Task<List<LifeFileDrugFormsResponseDto>> GetAllLifeFileDrugFormsAsync()
        {
            var lifeFileDrugForms = await _lifeFileDrugFormRepository.GetAllAsync();
            return lifeFileDrugForms.ToLifeFileDrugFormsResponseDtoList();
        }
    }
}

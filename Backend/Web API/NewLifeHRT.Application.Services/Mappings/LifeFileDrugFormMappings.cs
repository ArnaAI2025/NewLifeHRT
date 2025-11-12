using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class LifeFileDrugFormMappings
    {
        public static LifeFileDrugFormResponseDto ToLifeFileDrugFormsResponseDto(this LifeFileDrugForm drugForms)
        {
            return new LifeFileDrugFormResponseDto
            {
                Id = drugForms.Id,
                Name = drugForms.Name
            };
        }

        public static List<LifeFileDrugFormResponseDto> ToLifeFileDrugFormsResponseDtoList(this IEnumerable<LifeFileDrugForm> drugForms)
        {
            return drugForms.Select(drugForms => drugForms.ToLifeFileDrugFormsResponseDto()).ToList();
        }
    }
}

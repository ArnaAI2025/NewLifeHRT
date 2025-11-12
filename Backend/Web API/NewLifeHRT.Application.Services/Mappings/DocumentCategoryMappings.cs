using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class DocumentCategoryMappings
    {
        public static CommonDropDownResponseDto<int> ToCommonDropDownResponseDto(this DocumentCategory documentCategory)
        {
            return new CommonDropDownResponseDto<int>
            {
                Id = documentCategory.Id,
                Value = documentCategory.CategoryName,
            };
        }
        public static List<CommonDropDownResponseDto<int>> ToCommonDropDownResponsList(this IEnumerable<DocumentCategory> documentCategories)
        {
            return documentCategories.Select(p => p.ToCommonDropDownResponseDto()).ToList();
        }
    }
}

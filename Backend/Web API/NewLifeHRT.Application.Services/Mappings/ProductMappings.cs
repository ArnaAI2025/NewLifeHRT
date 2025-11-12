using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class ProductMappings
    {
        public static ProductResponseDto ToProductResponseDto(this Product product)
        {
            return new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                ProductID = product.ProductID,
                ParentName = product.Parent?.Name,
                Status = product.Status.StatusName,
                ModifiedOn = product.UpdatedAt ?? product.CreatedAt
            };
        }

        public static List<ProductResponseDto> ToProductResponseDtoList(this IEnumerable<Product> products)
        {
            return products.Select(p => p.ToProductResponseDto()).ToList();
        }

        public static ProductsDropdownResponseDto ToProductDropdownResponseDto(this Product product)
        {
            return new ProductsDropdownResponseDto
            {
                Id = product.Id,
                Name = product.Name
            };
        }

        public static List<ProductsDropdownResponseDto> ToProductDropdownResponseDtoList(this IEnumerable<Product> product)
        {
            return product.Select(product => product.ToProductDropdownResponseDto()).ToList();
        }

        public static ProductFullResponseDto ToProductFullResponseDto(this Product product)
        {
            return new ProductFullResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                ProductID = product.ProductID,
                LabCode = product.LabCode,
                LabCorp = product.LabCorp,
                IsColdStorageProduct = product.IsColdStorageProduct,
                ParentId = product.ParentId,
                ParentName = product.Parent?.Name,
                Description = product.Description,
                Protocol = product.Protocol,
                Scheduled = product.Scheduled,
                WebProductName = product.WebProductName,
                WebProductDescription = product.WebProductDescription,
                WebPopularMedicine = product.WebPopularMedicine,
                WebFormId = product.WebFormId,
                WebFormName = product.WebForm?.Name,
                WebStrengths = product.WebStrengths,
                WebCost = product.WebCost,
                EnableCalculator = product.EnableCalculator,
                NewEnableCalculator = product.NewEnableCalculator,
                PBPEnable = product.PBPEnable,
                TypeId = product.TypeId,
                TypeName = product.Type?.Name,
                Category1Id = product.Category1Id,
                Category1Name = product.Category1?.Name,
                Category2Id = product.Category2Id,
                Category2Name = product.Category2?.Name,
                Category3Id = product.Category3Id,
                Category3Name = product.Category3?.Name,
                StatusId = product.StatusId,
                StatusName = product.Status.StatusName,
                ModifiedOn = product.UpdatedAt ?? product.CreatedAt
            };
        }

        public static List<ProductFullResponseDto> ToProductFullResponseDtoList(this IEnumerable<Product> products)
        {
            return products.Select(p => p.ToProductFullResponseDto()).ToList();
        }
    }
}

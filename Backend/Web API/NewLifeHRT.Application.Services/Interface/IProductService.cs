using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Interface
{
    public interface IProductService
    {
        Task<List<ProductResponseDto>> GetAllProductsAsync();
        Task<ProductFullResponseDto?> GetProductByIdAsync(Guid id);
        Task<CreateProductResponseDto> CreateProductAsync(CreateProductRequestDto dto, int userId);
        Task<CreateProductResponseDto> UpdateProductAsync(Guid productId, CreateProductRequestDto dto, int userId);
        Task PublishProductsAsync(List<Guid> productIds, int userId);
        Task DeactivateProductsAsync(List<Guid> productIds, int userId);
        Task SoftDeleteProductsAsync(List<Guid> productIds, int userId);
        Task<List<ProductsDropdownResponseDto>> GetAllProductsForDropdownAsync();
    }
}

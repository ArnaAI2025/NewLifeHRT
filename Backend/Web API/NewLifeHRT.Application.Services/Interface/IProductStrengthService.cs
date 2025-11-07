using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Interface
{
    public interface IProductStrengthService
    {
        Task<List<ProductStrengthResponseDto>> GetAllByProductIdAsync(Guid productId);
        Task<ProductStrengthResponseDto> CreateAsync(ProductStrengthCreateRequestDto requestDto, int userId);
        Task<ProductStrengthResponseDto> UpdateAsync(Guid id, ProductStrengthCreateRequestDto requestDto, int userId);
        Task<Guid> DeleteAsync(Guid id);
    }
}

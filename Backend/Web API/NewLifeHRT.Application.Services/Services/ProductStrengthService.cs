using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Mappings;
using NewLifeHRT.Application.Services.Models.Request;
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
    public class ProductStrengthService : IProductStrengthService
    {
        private readonly IProductStrengthRepository _productStrengthRepository;
        public ProductStrengthService(IProductStrengthRepository productStrengthRepository)
        {
            _productStrengthRepository = productStrengthRepository;
        }

        /// <summary>
        /// Creates a new Product Strength entry for a specific product.
        /// </summary>
        public async Task<ProductStrengthResponseDto> CreateAsync(ProductStrengthCreateRequestDto requestDto, int userId)
        {
            var productStrength = new ProductStrength(requestDto.ProductId, requestDto.Name, requestDto.Strengths, requestDto.Price, userId.ToString(), DateTime.UtcNow);
            await _productStrengthRepository.AddAsync(productStrength);
            return productStrength.ToProductStrengthResponseDto();
        }

        /// <summary>
        /// Deletes a product strength record permanently from the database.
        /// </summary>
        public async Task<Guid> DeleteAsync(Guid id)
        {
            var deletedProductStrength = await _productStrengthRepository.GetByIdAsync(id);
            if (deletedProductStrength == null)
            {
                throw new Exception("Product Strength not found");
            }
            await _productStrengthRepository.DeleteAsync(deletedProductStrength);
            return deletedProductStrength.Id;
        }

        /// <summary>
        /// Retrieves all product strengths associated with a given product.
        /// </summary>
        public async Task<List<ProductStrengthResponseDto>> GetAllByProductIdAsync(Guid productId)
        {
            var productStrengths =  (await _productStrengthRepository.FindAsync(ps => ps.ProductId == productId)).ToList();
            return productStrengths.ToProductStrengthResponseDtoList();
        }

        /// <summary>
        /// Updates an existing product strength entry.
        /// </summary>
        public async Task<ProductStrengthResponseDto> UpdateAsync(Guid id, ProductStrengthCreateRequestDto requestDto, int userId)
        {
            var existingProductStrength = await _productStrengthRepository.GetByIdAsync(id);
            if (existingProductStrength == null)
            {
                throw new Exception("Product Strength not found");
            }
            existingProductStrength.ProductId = requestDto.ProductId;
            existingProductStrength.Name = requestDto.Name;
            existingProductStrength.Strengths = requestDto.Strengths;
            existingProductStrength.Price = requestDto.Price;
            existingProductStrength.UpdatedBy = userId.ToString();
            existingProductStrength.UpdatedAt = DateTime.UtcNow;

            await _productStrengthRepository.UpdateAsync(existingProductStrength);
            return existingProductStrength.ToProductStrengthResponseDto();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using NewLifeHRT.Application.Services.Interface;
using NewLifeHRT.Application.Services.Mappings;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Constants;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<CreateProductResponseDto> CreateProductAsync(CreateProductRequestDto dto, int userId)
        {
            var product = new Product(dto.ProductID, dto.Name, dto.IsColdStorageProduct, dto.IsLabCorp, dto.LabCode, dto.ParentId, dto.TypeId, dto.Category1Id,
                dto.Category2Id, dto.Category3Id, dto.ProductDescription, dto.Protocol, dto.IsScheduled, dto.WebProductName, dto.WebProductDescription,
                dto.IsWebPopularMedicine, dto.WebFormId, dto.WebStrength, dto.WebCost, dto.IsEnabledCalculator, dto.IsNewEnabledCalculator, dto.IsPBPEnabled, ProductStatusConstants.Draft, userId.ToString(), DateTime.UtcNow);

            await _productRepository.AddAsync(product);
            return new CreateProductResponseDto { Id = product.Id };
        }

        public async Task DeactivateProductsAsync(List<Guid> productIds, int userId)
        {
            var products = (await _productRepository.FindAsync(p => productIds.Contains(p.Id))).ToList();

            if (products == null || !products.Any())
                throw new Exception("No matching products found for the provided IDs.");

            foreach (var product in products)
            {
                product.StatusId = ProductStatusConstants.Retired;
                product.IsActive = false;
                product.UpdatedBy = userId.ToString();
                product.UpdatedAt = DateTime.UtcNow;
            }

            await _productRepository.BulkUpdateAsync(products);
        }

        public async Task<List<ProductResponseDto>> GetAllProductsAsync()
        {
            var includes = new[] { "Parent", "Status" };
            var products = await _productRepository.AllWithIncludeAsync(includes);
            return products.ToProductResponseDtoList();
        }

        public async Task<List<ProductsDropdownResponseDto>> GetAllProductsForDropdownAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return products.ToProductDropdownResponseDtoList();
        }

        public async Task<ProductFullResponseDto?> GetProductByIdAsync( Guid id)
        {
            var includes = new[] { "Parent", "Status", "WebForm", "Type", "Category1", "Category2", "Category3"};
            var product = await _productRepository.GetWithIncludeAsync(id,includes);
            if (product == null) return null;
            return product?.ToProductFullResponseDto();
        }

        public async Task PublishProductsAsync(List<Guid> productIds, int userId)
        {
            var products = (await _productRepository.FindAsync(p => productIds.Contains(p.Id))).ToList();

            if (products == null || !products.Any())
                throw new Exception("No matching products found for the provided IDs.");

            foreach (var product in products)
            {
                product.StatusId = ProductStatusConstants.Active;
                product.IsActive = true;
                product.UpdatedBy = userId.ToString();
                product.UpdatedAt = DateTime.UtcNow;
            }

            await _productRepository.BulkUpdateAsync(products);
        }

        public async Task SoftDeleteProductsAsync(List<Guid> productIds, int userId)
        {
            var products = (await _productRepository.FindAsync(p => productIds.Contains(p.Id))).ToList();

            if (products == null || !products.Any())
                throw new Exception("No matching products found for the provided IDs.");

            foreach (var product in products)
            {
                product.IsDeleted = true;
                product.UpdatedBy = userId.ToString();
                product.UpdatedAt = DateTime.UtcNow;
            }

            await _productRepository.BulkUpdateAsync(products);
        }

        public async Task<CreateProductResponseDto> UpdateProductAsync(Guid productId, CreateProductRequestDto dto, int userId)
        {
            var existingProduct = await _productRepository.GetByIdAsync(productId);
            if (existingProduct == null)
            {
                throw new Exception("Product not found");
            }
            existingProduct.ProductID = dto.ProductID;
            existingProduct.Name = dto.Name;
            existingProduct.LabCorp = dto.IsLabCorp;
            existingProduct.IsColdStorageProduct = dto.IsColdStorageProduct;
            existingProduct.LabCode = dto.LabCode;
            existingProduct.ParentId = dto.ParentId;
            existingProduct.TypeId = dto.TypeId;
            existingProduct.Category1Id = dto.Category1Id;
            existingProduct.Category2Id = dto.Category2Id;
            existingProduct.Category3Id = dto.Category3Id;
            existingProduct.Description = dto.ProductDescription;
            existingProduct.Protocol = dto.Protocol;
            existingProduct.Scheduled = dto.IsScheduled;
            existingProduct.WebProductName = dto.WebProductName;
            existingProduct.WebProductDescription = dto.WebProductDescription;
            existingProduct.WebPopularMedicine = dto.IsWebPopularMedicine;
            existingProduct.WebFormId = dto.WebFormId;
            existingProduct.WebStrengths = dto.WebStrength;
            existingProduct.WebCost = dto.WebCost;
            existingProduct.EnableCalculator = dto.IsEnabledCalculator;
            existingProduct.NewEnableCalculator = dto.IsNewEnabledCalculator;
            existingProduct.PBPEnable = dto.IsPBPEnabled;
            existingProduct.UpdatedBy = userId.ToString();
            existingProduct.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(existingProduct);
            return new CreateProductResponseDto { Id = existingProduct.Id };
        }
    }
}

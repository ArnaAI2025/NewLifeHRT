using Microsoft.EntityFrameworkCore;
using NewLifeHRT.Application.Services.Interfaces;
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

        /// <summary>
        /// Creates a new product record in the system.
        /// </summary>
        public async Task<CreateProductResponseDto> CreateProductAsync(CreateProductRequestDto dto, int userId)
        {
            var product = new Product(dto.ProductID, dto.Name, dto.IsColdStorageProduct, dto.IsLabCorp, dto.LabCode, dto.ParentId, dto.TypeId, dto.Category1Id,
                dto.Category2Id, dto.Category3Id, dto.ProductDescription, dto.Protocol, dto.IsScheduled, dto.WebProductName, dto.WebProductDescription,
                dto.IsWebPopularMedicine, dto.WebFormId, dto.WebStrength, dto.WebCost, dto.IsEnabledCalculator, dto.IsNewEnabledCalculator, dto.IsPBPEnabled, ProductStatusConstants.Draft, userId.ToString(), DateTime.UtcNow);

            await _productRepository.AddAsync(product);
            return new CreateProductResponseDto { Id = product.Id };
        }

        /// <summary>
        /// Deactivates (retires) the specified products by setting their status and IsActive flag.
        /// </summary>
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

        /// <summary>
        /// Retrieves all products along with their parent and status details.
        /// </summary>
        public async Task<List<ProductResponseDto>> GetAllProductsAsync()
        {
            var includes = new[] { "Parent", "Status" };
            var products = await _productRepository.AllWithIncludeAsync(includes);
            return products.ToProductResponseDtoList();
        }

        /// <summary>
        /// Fetches all products for populating dropdowns.
        /// </summary>
        public async Task<List<ProductsDropdownResponseDto>> GetAllProductsForDropdownAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return products.ToProductDropdownResponseDtoList();
        }

        /// <summary>
        /// Retrieves full product details by ID, including its relationships.
        /// </summary>
        public async Task<ProductFullResponseDto?> GetProductByIdAsync( Guid id)
        {
            var includes = new[] { "Parent", "Status", "WebForm", "Type", "Category1", "Category2", "Category3"};
            var product = await _productRepository.GetWithIncludeAsync(id,includes);
            if (product == null) return null;
            return product?.ToProductFullResponseDto();
        }

        /// <summary>
        /// Publishes a list of products (makes them active and available).
        /// </summary>
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

        /// <summary>
        /// Marks products as deleted without removing them from the database (soft delete).
        /// </summary>
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

        /// <summary>
        /// Updates an existing product’s details.
        /// </summary>
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

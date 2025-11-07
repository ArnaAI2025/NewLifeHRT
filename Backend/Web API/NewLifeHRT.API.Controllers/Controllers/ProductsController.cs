
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MultiTenantTest.Controllers;
using NewLifeHRT.Application.Services.Interface;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Domain.Entities;

namespace NewLifeHRT.API.Controllers.Controllers
{
    [ApiController]
    public class ProductsController : BaseApiController<ProductsController>
    {
        private readonly IProductService _productService;
        private readonly IProductTypeService _productTypeService;
        private readonly IProductCategoryService _productCategoryService;
        private readonly IProductStatusService _productStatusService;
        private readonly IProductWebFormService _productWebFormService;

        public ProductsController(IProductService productService, IProductTypeService productTypeService, IProductCategoryService productCategoryService, IProductStatusService productStatusService, IProductWebFormService productWebFormService)
        {
            _productService = productService;
            _productTypeService = productTypeService;
            _productCategoryService = productCategoryService;
            _productStatusService = productStatusService;
            _productWebFormService = productWebFormService;
        }

        [HttpGet("get-all-products")]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("get-product-types")]
        public async Task<IActionResult> GetProductTypes()
        {
            var productTypes = await _productTypeService.GetAllProductTypesAsync();
            return Ok(productTypes);
        }

        [HttpGet("get-product-categories")]
        public async Task<IActionResult> GetProductCategories()
        {
            var productCategories = await _productCategoryService.GetAllProductCategoriesAsync();
            return Ok(productCategories);
        }

        [HttpGet("get-product-statuses")]
        public async Task<IActionResult> GetProductStatuses()
        {
            var productStatuses = await _productStatusService.GetAllProductStatusAsync();
            return Ok(productStatuses);
        }

        [HttpGet("get-product-webforms")]
        public async Task<IActionResult> GetProductWebForms()
        {
            var productWebForms = await _productWebFormService.GetAllProductWebFormsAsync();
            return Ok(productWebForms);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            var result = await _productService.CreateProductAsync(dto, userId.Value);
            return Ok(result);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] CreateProductRequestDto dto)
        {
            var userId = GetUserId();
            var response = await _productService.UpdateProductAsync(id, dto, userId.Value);
            return Ok(response);
        }

        [HttpGet("get-product-by-id/{id}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var response = await _productService.GetProductByIdAsync(id);
            if (response == null)
                return NotFound();
            return Ok(response);
        }

        [HttpPost("publish")]
        public async Task<IActionResult> PublishProduct([FromBody] PublishProductsRequestDto request)
        {
            if (request?.ProductIds == null || !request.ProductIds.Any())
            {
                return BadRequest("ProductIds list cannot be empty.");
            }

            var userId = GetUserId();

            await _productService.PublishProductsAsync(request.ProductIds, userId.Value);
            return Ok();
        }

        [HttpPost("deactivate")]
        public async Task<IActionResult> DeactivateProducts([FromBody] PublishProductsRequestDto request)
        {
            if (request?.ProductIds == null || !request.ProductIds.Any())
            {
                return BadRequest("ProductIds list cannot be empty.");
            }
            var userId = GetUserId();
            await _productService.DeactivateProductsAsync(request.ProductIds, userId.Value);
            return Ok();
        }

        [HttpPost("delete")]
        public async Task<IActionResult> SoftDeleteProducts([FromBody] PublishProductsRequestDto request)
        {
            if (request?.ProductIds == null || !request.ProductIds.Any())
            {
                return BadRequest("ProductIds list cannot be empty.");
            }
            var userId = GetUserId();
            await _productService.SoftDeleteProductsAsync(request.ProductIds, userId.Value);
            return Ok();
        }

        [HttpGet("get-all-products-for-dropdown")]
        public async Task<IActionResult> GetAllProductsForDropdown()
        {
            var products = await _productService.GetAllProductsForDropdownAsync();
            return Ok(products);
        }

    }
}

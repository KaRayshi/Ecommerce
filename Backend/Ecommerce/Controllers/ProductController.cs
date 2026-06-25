using Ecommerce.Data;
using Ecommerce.Dto.Cart;
using Ecommerce.Dto.Product;
using Ecommerce.Helpers;
using Ecommerce.Interfaces;
using Ecommerce.Mappers;
using Ecommerce.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpPost("add_product")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddProduct([FromBody] CreateProductDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _productRepository.CreateProductAsync(createDto);

            if (product == null)
            {
                return BadRequest("Product with the same name already exists.");
            } 

            return Ok(product.ToProductDto());
        }

        [HttpGet("view_products")]
        public async Task<IActionResult> ViewProducts([FromQuery] ProductQueryObject query)
        {
            var products = await _productRepository.GetAllAsync(query);

            if(products.Count == 0)
            {
                return NotFound("No Products Available");
            }

            var productDtos = products.Select(p => p.ToProductDto()).ToList();

            return Ok(productDtos);
        }


        [HttpGet("view_product/{id:int}")]
        public async Task<IActionResult> ViewProductsById(int id)
        {
            var products = await _productRepository.GetProductByIdAsync(id);

            if (products == null)
            {
                return NotFound("No Products Available");
            }

            return Ok(products.ToProductDto());
        }

        [HttpPut("update_product/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingProductName = await _productRepository.IsNameTakenAsync(updateDto.ProductName, id);

            if (existingProductName)
            {
                return BadRequest("Product with the same name already exists");
            }

            var existingProduct = await _productRepository.UpdateProductAsync(id, updateDto);

            if (existingProduct == null)
            {
                return NotFound("Product Not Found");
            }   

            var productDto = existingProduct.ToProductDto();

            return Ok(productDto);
        }

        [HttpDelete("delete_product/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var existingProduct = await _productRepository.DeleteProductAsync(id);

            if (existingProduct == null)
            {
                return NotFound("Product Not Found");
            }
            return Ok("Product Deleted");
        }
    }
}

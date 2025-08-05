using Microsoft.AspNetCore.Mvc;
using StoreApi.Models.Domain;
using StoreApi.Models.DTOs.Product;
using StoreApi.Services;

namespace StoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IProductService service) : ControllerBase
    {
        private readonly IProductService _service = service;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _service.GetProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Product>> GetProduct(Guid id)
        {
            var product = await _service.GetProductAsync(id);
            if (product == null)
                return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(CreateProductDto dto)
        {
            var product = await _service.CreateProductAsync(dto);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> PutProduct(Guid id, UpdateProductDto dto)
        {
            var updated = await _service.UpdateProductAsync(id, dto);
            if (!updated)
                return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var deleted = await _service.DeleteProductAsync(id);
            if (!deleted)
                return NotFound();
            return NoContent();
        }
    }
}

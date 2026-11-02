using InventorySystem.API.DTOs.Products;
using InventorySystem.API.Services.Product;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        // GET: api/products/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDto>> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if(product == null)
            {
                return NotFound(new { message = $"Product with ID {id} was not found" });
            }
            return Ok(product);
        }

        // POST: api/products
        [HttpPost]
        public async Task<ActionResult<ProductDto>> Create(CreateProductDto dto)
        {
            try
            {
                var createProduct = await _productService.CreateAsync(dto);

                // Returns HTTP 201 Created with Location header: api/products/{newId}
                return CreatedAtAction(nameof(GetById), new { id = createProduct.Id }, createProduct);
            }
            catch (Exception ex) { 
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/products/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, UpdateProductDto dto)
        {
            try
            {
                var updatedProduct = await _productService.UpdateAsync(id, dto);

                if (!updatedProduct)
                {
                    return NotFound(new { message = $"Product with ID {id} not found" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/products/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deletedProduct = await _productService.DeleteAsync(id);
            if(!deletedProduct)
            {
                return NotFound(new { message = $"Product with ID {id} not found" });
            }

            return NoContent(); // HTTP 204
        }
    }
}

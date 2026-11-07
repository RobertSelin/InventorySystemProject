using InventorySystem.API.DTOs;
using InventorySystem.API.DTOs.Products;
using InventorySystem.API.Services.Product;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InventorySystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/products
        /// <summary>
        /// Retrieves a paginated list of products with optional search filtering.
        /// </summary>
        /// <param name="search">Optional search term to filter by product name or SKU.</param>
        /// <param name="page">The page number (1-based, default: 1).</param>
        /// <param name="pageSize">Number of items per page (default: 10).</param>
        /// <returns>A paginated result containing products and metadata.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<ProductDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<ProductDto>>> GetAll([FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var products = await _productService.GetPaginatedAsync(search, page, pageSize);
            return Ok(products);
        }

        // GET: api/products/id
        /// <summary>
        /// Retrieves a product by its unique identifier.
        /// </summary>
        /// <param name="id">The product ID.</param>
        /// <returns>The product details.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDto>> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            return Ok(product);
        }

        // POST: api/products
        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <param name="dto">The product creation payload.</param>
        /// <returns>The newly created product.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductDto>> Create(CreateProductDto dto)
        {
            var createProduct = await _productService.CreateAsync(dto);

            // Returns HTTP 201 Created with Location header: api/products/{newId}
            return CreatedAtAction(nameof(GetById), new { id = createProduct.Id }, createProduct);
        }

        // PUT: api/products/id
        /// <summary>
        /// Updates an existing product.
        /// </summary>
        /// <param name="id">The ID of the product to update.</param>
        /// <param name="dto">The updated product payload.</param>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, UpdateProductDto dto)
        {
            var updatedProduct = await _productService.UpdateAsync(id, dto);
            return NoContent();
        }

        // DELETE: api/products/id
        /// <summary>
        /// Deletes a product by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the product to delete.</param>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var deletedProduct = await _productService.DeleteAsync(id);
            return NoContent(); // HTTP 204
        }

        // POST: api/products/id/stock
        /// <summary>
        /// Adjusts the stock quantity of a product (positive for restock, negative for deduction).
        /// </summary>
        /// <param name="id">The product ID.</param>
        /// <param name="dto">The stock adjustment payload with delta change and reason.</param>
        /// <returns>The updated product with new stock level.</returns>
        [HttpPost("{id:int}/stock")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDto>> AdjustStock(int id, [FromBody] StockAdjustmentDto dto)
        {
            var updateStock = await _productService.AdjustStockAsync(id, dto);
            return Ok(updateStock);
        }

        // PUT: api/products/id
        /// <summary>
        /// Retrieves the audit trail of stock adjustments for a specific product.
        /// </summary>
        /// <param name="id">The product ID.</param>
        /// <returns>A list of inventory logs ordered by date descending.</returns>
        [HttpGet("{id:int}/history")]
        [ProducesResponseType(typeof(IEnumerable<InventoryLogDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<InventoryLogDto>>> GetHistory(int id)
        {
            var history = await _productService.GetStockHistoryAsync(id);
            return Ok(history);
        }
    }
}

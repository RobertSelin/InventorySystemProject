using InventorySystem.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;
using InventorySystem.Web.ViewModels;
using System.Threading.Tasks;

namespace InventorySystem.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductApiClientService _productApiClientService;

        public ProductsController(IProductApiClientService productApiClientService)
        {
            _productApiClientService = productApiClientService;
        }

        // GET: /Products
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? search = null)
        {
            ViewBag.CurrentSearch = search;
            var result = await _productApiClientService.GetProductAsync(page, pageSize, search);
            return View(result ?? new PagedResult<ViewModels.ProductViewModel>());
        }

        // GET: /Products/Details/id
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productApiClientService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var history = await _productApiClientService.GetProductHistoryAsync(id);
            ViewBag.History = history ?? Enumerable.Empty<InventoryLogViewModel>();

            return View(product);
        }

        // GET: /Products/Create
        public IActionResult Create()
        {
            return View(new CreateProductVIewModel());
        }

        // POST: /Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductVIewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _productApiClientService.CreateProductAsync(model);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = $"Product '{model.Name}' created successfully.";
                return RedirectToAction(nameof(Index));
            }

            // Read RFC 7807 ProblemDetails from API and show business rule errors in UI
            await AddApiErrorsToModelState(response);
            return View(model);
        }

        // GET: /Products/Edit/id
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productApiClientService.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            var updateModel = new UpdateProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Barcode = product.BarCode,
                Price = product.Price,
                CategoryId = product.CategoryId
            };

            return View(updateModel);
        }

        // POST: /Product/Edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _productApiClientService.UpdateProductAsync(id, model);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Product updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            // Read RFC 7807 ProblemDetails from API and show business rule errors in UI
            await AddApiErrorsToModelState(response);
            return View(model);
        }

        // GET: Products/AdjustStock/id
        public async Task<IActionResult> AdjustStock(int id)
        {
            var product = await _productApiClientService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var model = new StockAdjustmentViewModel
            {
                ProductId = product.Id,
                ProductName = product.Name,
                CurrentStock = product.QuantityInStock
            };

            return View(model);
        }

        // POST: Products/AdjustStock/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdjustStock(StockAdjustmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _productApiClientService.AdjustStockAsync(model.ProductId, model.QuantityChange, model.Reason);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = $"Stock for '{model.ProductName}' adjusted successfully.";
                return RedirectToAction(nameof(Details), new { id = model.ProductId });
            }

            // Read RFC 7807 ProblemDetails from API and show business rule errors in UI
            await AddApiErrorsToModelState(response);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _productApiClientService.DeleteProductAsync(id);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Product deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Could not delete product. It may have associated records.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Products/LowStock?threshold=10
        public async Task<IActionResult> LowStock(int threshold = 10)
        {
            ViewBag.Threshold = threshold;
            var items = await _productApiClientService.GetLowStockProductAsync(threshold);
            return View(items ?? Enumerable.Empty<ProductViewModel>());
        }

        /// <summary>
        /// Deserializes RFC 7807 ProblemDetails or raw string errors from the API response
        /// and maps them into MVC ModelState.
        /// </summary>
        private async Task AddApiErrorsToModelState(HttpResponseMessage response)
        {
            try
            {
                var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
                if(!string.IsNullOrWhiteSpace(problemDetails?.Detail))
                {
                    ModelState.AddModelError(string.Empty, problemDetails.Detail);
                    return;
                }
                if(!string.IsNullOrWhiteSpace(problemDetails?.Title))
                {
                    ModelState.AddModelError(string.Empty, problemDetails.Title);
                    return;
                }
            }
            catch
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                if(!string.IsNullOrWhiteSpace(errorContent))
                {
                    ModelState.AddModelError(string.Empty, errorContent);
                    return;
                }
            }

            ModelState.AddModelError(string.Empty, $"API Error: {response.StatusCode} ({response.ReasonPhrase})");
        }
    }
}

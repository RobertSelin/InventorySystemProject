using Castle.Core.Logging;
using InventorySystem.API.DTOs.Products;
using InventorySystem.API.Exceptions;
using InventorySystem.API.Models;
using InventorySystem.API.Repositories;
using InventorySystem.API.Services.Product;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.Tests
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _productRepoMock;
        private readonly Mock<IRepository<Category>> _categoryRepoMock;
        private readonly Mock<ILogger<ProductService>> _loggerMock;
        private readonly ProductService _sut; // SUT = System Under Test

        public ProductServiceTests()
        {
            // Initialization of Mocks
            _productRepoMock = new Mock<IProductRepository>();
            _categoryRepoMock = new Mock<IRepository<Category>>();
            _loggerMock = new Mock<ILogger<ProductService>>();

            // Inject mocks into the SUT
            _sut = new ProductService(
                _productRepoMock.Object,
                _categoryRepoMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task AdjustStockAsync_PositiveDelta_IncreaseStockAndSavesChanges()
        {
            // Arrange

            int productId = 1;
            var existingProduct = new Product
            {
                Id = productId,
                Name = "Wireless Mouse",
                SKU = "TECH-001",
                QuantityInStock = 10,
                Price = 25.00m
            };

            var adjustmentDto = new StockAdjustmentDto
            {
                QuantityChange = 5,
                Reason = "Supplier Restock"
            };

            _productRepoMock
                .Setup(repo => repo.GetByIdWithDetailAsync(productId))
                .ReturnsAsync(existingProduct);

            _productRepoMock
                .Setup(repo => repo.SaveChangesAsync())
                .ReturnsAsync(true);

            // Act

            var result = await _sut.AdjustStockAsync(productId, adjustmentDto);

            // Assert

            Assert.NotNull(result);
            Assert.Equal(15, result.QuantityInStock);
            Assert.Single(existingProduct.Logs);
            Assert.Equal(5, existingProduct.Logs.First().QuantityChange);
            Assert.Equal(15, existingProduct.Logs.First().NewQuantity);

            _productRepoMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task AdjustStockAsync_DeductingMoreThanStock_ThrowsBusinessRuleException()
        {
            // Arrange 
            int productId = 1;
            var existingProduct = new Product
            {
                Id = productId,
                Name = "Wireless Mouse",
                SKU = "TECH-001",
                QuantityInStock = 10,
                Price = 25.00m
            };

            var adjustmentDto = new StockAdjustmentDto
            {
                QuantityChange = -15,
                Reason = "Defective shipment"
            };

            _productRepoMock
                .Setup(repo => repo.GetByIdWithDetailAsync(productId))
                .ReturnsAsync(existingProduct);

            // Act

            var exception = await Assert.ThrowsAsync<BusinessRuleException>(() =>
                _sut.AdjustStockAsync(productId, adjustmentDto));

            // Assert

            Assert.Contains("Insufficient stock", exception.Message);

            _productRepoMock.Verify(repo => repo.SaveChangesAsync(), Times.Never);

        }

        [Fact]
        public async Task AdjustStockAsync_NonExistentProduct_ThrowsNotFoundException()
        {
            int nonExistentProductId = 999;

            var adjustmentDto = new StockAdjustmentDto
            {
                QuantityChange = 10,
                Reason = "Restock"
            };

            _productRepoMock
                .Setup(repo => repo.GetByIdWithDetailAsync(nonExistentProductId))
                .ReturnsAsync((Product?)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _sut.AdjustStockAsync(nonExistentProductId, adjustmentDto));

            _productRepoMock.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task AdjustStockAsync_ZeroQuantityChange_ThrowsBusinessRuleException()
        {
            // ARRANGE
            int productId = 1;
            var adjustmentDto = new StockAdjustmentDto
            {
                QuantityChange = 0, // Invalid: change cannot be 0
                Reason = "None"
            };

            // ACT & ASSERT
            var exception = await Assert.ThrowsAsync<BusinessRuleException>(() =>
                _sut.AdjustStockAsync(productId, adjustmentDto));

            Assert.Equal("Quantity change cannot be 0", exception.Message);
            _productRepoMock.Verify(repo => repo.GetByIdWithDetailAsync(It.IsAny<int>()), Times.Never);
        }
    }
}

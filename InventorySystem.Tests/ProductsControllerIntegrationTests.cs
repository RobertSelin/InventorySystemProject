using InventorySystem.API.DTOs.Auth;
using InventorySystem.API.DTOs.Products;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.Tests
{
    public class ProductsControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ProductsControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_ReturnsSuccessAndJsonContentType()
        {
            // send an HTTP GET request through full ASP.Net Core pipeline
            var response = await _client.GetAsync("api/products");

            // Assert: Verify 200 Ok and application/json Content-type
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(response.Content.Headers.ContentType);
            Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task Create_WithoutToken_ReturnsUnauthorized()
        {
            // Arrange
            
            var newProduct = new CreateProductDto
            {
                Name = "Unauthorized Test Product",
                SKU = "NOAUTH-001",
                Price = 99.99m,
                InitialQuantity = 10,
                CategoryId = 1
            };

            // Act

            var response = await _client.PostAsJsonAsync("/api/products", newProduct);

            // Assert

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Create_WithAdminToken_ReturnsCreated()
        {
            // Arrange

            var token = await GetAdminJwtTokenAsync();

            var uniqueSuffix = Guid.NewGuid().ToString("N")[..6];
            var newProduct = new CreateProductDto
            {
                Name = $"Integration Test Product {uniqueSuffix}",
                SKU = $"INT-{uniqueSuffix}",
                Price = 149.99m,
                InitialQuantity = 15,
                CategoryId = 1
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "/api/products")
            {
                Content = JsonContent.Create(newProduct)
            };

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Act

            var response = await _client.SendAsync(request);

            // Assert

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var createdProduct = await response.Content.ReadFromJsonAsync<ProductDto>();
            Assert.NotNull(createdProduct);
            Assert.Equal(newProduct.Name, createdProduct.Name);
            Assert.Equal(newProduct.SKU, createdProduct.SKU);
        }

        // Helper method - authenticates as admin and extracts jwt bearer token
        private async Task<string> GetAdminJwtTokenAsync()
        {
            var loginDto = new LoginDto
            {
                Username = "admin",
                Password = "111111"
            };

            var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
            response.EnsureSuccessStatusCode();

            var authResult = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            Assert.NotNull(authResult);
            return authResult.Token;
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using StoreApi.Controllers;
using StoreApi.Models.Domain;
using StoreApi.Models.DTOs.Product;
using StoreApi.Services;

namespace StoreApiUnitTests.Controllers
{
    public class ProductsControllerTests
    {
        private readonly IProductService _service;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            _service = Substitute.For<IProductService>();
            _controller = new ProductsController(_service);
        }

        [Fact]
        public async Task GetProducts_ReturnsOkResult_WithListOfProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new() { Id = Guid.NewGuid(), Name = "Product1", Price = 10 },
                new() { Id = Guid.NewGuid(), Name = "Product2", Price = 20 }
            };
            _service.GetProductsAsync().Returns(products);

            // Act
            var result = await _controller.GetProducts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProducts = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
            Assert.Equal(products.Count, returnedProducts.Count());
        }

        [Fact]
        public async Task GetProduct_ReturnsOkResult_WithProduct_WhenFound()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product { Id = productId, Name = "Product1", Price = 10 };
            _service.GetProductAsync(productId).Returns(product);

            // Act
            var result = await _controller.GetProduct(productId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProduct = Assert.IsType<Product>(okResult.Value);
            Assert.Equal(productId, returnedProduct.Id);
        }

        [Fact]
        public async Task GetProduct_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _service.GetProductAsync(productId).Returns((Product?)null);

            // Act
            var result = await _controller.GetProduct(productId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task PostProduct_ReturnsCreatedAtActionResult_WithCreatedProduct()
        {
            // Arrange
            var createDto = new CreateProductDto { Name = "New Product", Price = 30 };
            var createdProduct = new Product { Id = Guid.NewGuid(), Name = createDto.Name, Price = createDto.Price };
            _service.CreateProductAsync(createDto).Returns(createdProduct);

            // Act
            var result = await _controller.PostProduct(createDto);

            // Assert
            var createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedProduct = Assert.IsType<Product>(createdAtResult.Value);
            Assert.Equal(createdProduct.Id, returnedProduct.Id);
        }

        [Fact]
        public async Task PutProduct_ReturnsNoContent_WhenUpdateIsSuccessful()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var updateDto = new UpdateProductDto { Name = "Updated Product", Price = 40 };
            _service.UpdateProductAsync(productId, updateDto).Returns(true);

            // Act
            var result = await _controller.PutProduct(productId, updateDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PutProduct_ReturnsNotFound_WhenUpdateFails()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var updateDto = new UpdateProductDto { Name = "Updated Product", Price = 40 };
            _service.UpdateProductAsync(productId, updateDto).Returns(false);

            // Act
            var result = await _controller.PutProduct(productId, updateDto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteProduct_ReturnsNoContent_WhenDeleteIsSuccessful()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _service.DeleteProductAsync(productId).Returns(true);

            // Act
            var result = await _controller.DeleteProduct(productId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteProduct_ReturnsNotFound_WhenDeleteFails()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _service.DeleteProductAsync(productId).Returns(false);

            // Act
            var result = await _controller.DeleteProduct(productId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
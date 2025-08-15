using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StoreApi.Data;
using StoreApi.Models.Domain;
using StoreApi.Models.DTOs.Product;
using StoreApi.Services;

namespace StoreApiUnitTests.Services
{
    public class ProductServiceTests
    {
        private readonly StoreDbContext _context;
        private readonly IMapper _mapper;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            // Set up in-memory database options
            var options = new DbContextOptionsBuilder<StoreDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new StoreDbContext(options);

            // Set up AutoMapper
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CreateProductDto, Product>();
                cfg.CreateMap<UpdateProductDto, Product>();
            });
            _mapper = configuration.CreateMapper();

            // Initialize the service
            _service = new ProductService(_context, _mapper);
        }

        [Fact]
        public async Task GetProductsAsync_ReturnsAllProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = Guid.NewGuid(), Name = "Product1", Price = 100 },
                new Product { Id = Guid.NewGuid(), Name = "Product2", Price = 200 }
            };

            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetProductsAsync();

            // Assert
            Assert.Equal(products.Count, result.Count());
        }

        [Fact]
        public async Task GetProductAsync_ReturnsProduct_WhenFound()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product { Id = productId, Name = "Product1", Price = 100 };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetProductAsync(productId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productId, result.Id);
        }

        [Fact]
        public async Task GetProductAsync_ReturnsNull_WhenNotFound()
        {
            // Act
            var result = await _service.GetProductAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateProductAsync_AddsProductSuccessfully()
        {
            // Arrange
            var createDto = new CreateProductDto { Name = "New Product", Price = 150 };

            // Act
            var result = await _service.CreateProductAsync(createDto);

            // Assert
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.Equal(createDto.Name, result.Name);
            Assert.Equal(createDto.Price, result.Price);
            Assert.Single(_context.Products);
        }

        [Fact]
        public async Task UpdateProductAsync_UpdatesProductSuccessfully_WhenProductExists()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product { Id = productId, Name = "Old Product", Price = 100 };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            var updateDto = new UpdateProductDto { Name = "Updated Product", Price = 200 };

            // Act
            var result = await _service.UpdateProductAsync(productId, updateDto);

            // Assert
            Assert.True(result);
            var updatedProduct = await _context.Products.FindAsync(productId);
            Assert.Equal(updateDto.Name, updatedProduct?.Name);
            Assert.Equal(updateDto.Price, updatedProduct?.Price);
        }

        [Fact]
        public async Task UpdateProductAsync_ReturnsFalse_WhenProductDoesNotExist()
        {
            // Act
            var result = await _service.UpdateProductAsync(Guid.NewGuid(), new UpdateProductDto { Name = "Updated Product", Price = 200 });

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteProductAsync_DeletesProductSuccessfully_WhenProductExists()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product { Id = productId, Name = "Product1", Price = 100 };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.DeleteProductAsync(productId);

            // Assert
            Assert.True(result);
            Assert.Null(await _context.Products.FindAsync(productId));
        }

        [Fact]
        public async Task DeleteProductAsync_ReturnsFalse_WhenProductDoesNotExist()
        {
            // Act
            var result = await _service.DeleteProductAsync(Guid.NewGuid());

            // Assert
            Assert.False(result);
        }
    }
}
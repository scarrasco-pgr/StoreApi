using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StoreApi.Data;
using StoreApi.Models.Domain;
using StoreApi.Models.DTOs.OrderDetail;
using StoreApi.Services;

namespace StoreApiUnitTests.Services
{
    public class OrderDetailServiceTests
    {
        private readonly StoreDbContext _context;
        private readonly IMapper _mapper;
        private readonly OrderDetailService _service;

        public OrderDetailServiceTests()
        {
            // Set up in-memory database options
            var options = new DbContextOptionsBuilder<StoreDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new StoreDbContext(options);

            // Set up AutoMapper
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UpdateOrderDetailDto, OrderDetail>();
            });
            _mapper = configuration.CreateMapper();

            // Initialize the service
            _service = new OrderDetailService(_context, _mapper);
        }

        [Fact]
        public async Task GetOrderDetailsAsync_ReturnsAllOrderDetails()
        {
            // Arrange
            var product = new Product { Id = Guid.NewGuid(), Name = "Product1", Price = 100 };
            var orderDetails = new List<OrderDetail>
            {
                new OrderDetail { Id = Guid.NewGuid(), Product = product, Quantity = 2 },
                new OrderDetail { Id = Guid.NewGuid(), Product = product, Quantity = 5 }
            };

            await _context.Products.AddAsync(product);
            await _context.OrderDetails.AddRangeAsync(orderDetails);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetOrderDetailsAsync();

            // Assert
            Assert.Equal(orderDetails.Count, result.Count());
        }

        [Fact]
        public async Task GetOrderDetailAsync_ReturnsOrderDetail_WhenFound()
        {
            // Arrange
            var orderDetailId = Guid.NewGuid();
            var product = new Product { Id = Guid.NewGuid(), Name = "Product1", Price = 100 };
            var orderDetail = new OrderDetail { Id = orderDetailId, Product = product, Quantity = 3 };

            await _context.Products.AddAsync(product);
            await _context.OrderDetails.AddAsync(orderDetail);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetOrderDetailAsync(orderDetailId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderDetailId, result.Id);
        }

        [Fact]
        public async Task GetOrderDetailAsync_ReturnsNull_WhenNotFound()
        {
            // Act
            var result = await _service.GetOrderDetailAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateOrderDetailAsync_UpdatesOrderDetailSuccessfully_WhenOrderDetailExists()
        {
            // Arrange
            var orderDetailId = Guid.NewGuid();
            var product = new Product { Id = Guid.NewGuid(), Name = "Product1", Price = 100 };
            var orderDetail = new OrderDetail { Id = orderDetailId, Product = product, Quantity = 3 };

            await _context.Products.AddAsync(product);
            await _context.OrderDetails.AddAsync(orderDetail);
            await _context.SaveChangesAsync();

            var updateDto = new UpdateOrderDetailDto { Quantity = 10 };

            // Act
            var result = await _service.UpdateOrderDetailAsync(orderDetailId, updateDto);

            // Assert
            Assert.True(result);
            var updatedOrderDetail = await _context.OrderDetails.FindAsync(orderDetailId);
            Assert.Equal(updateDto.Quantity, updatedOrderDetail.Quantity);
        }

        [Fact]
        public async Task UpdateOrderDetailAsync_ReturnsFalse_WhenOrderDetailDoesNotExist()
        {
            // Act
            var result = await _service.UpdateOrderDetailAsync(Guid.NewGuid(), new UpdateOrderDetailDto { Quantity = 10 });

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteOrderDetailAsync_DeletesOrderDetailSuccessfully_WhenOrderDetailExists()
        {
            // Arrange
            var orderDetailId = Guid.NewGuid();
            var product = new Product { Id = Guid.NewGuid(), Name = "Product1", Price = 100 };
            var orderDetail = new OrderDetail { Id = orderDetailId, Product = product, Quantity = 3 };

            await _context.Products.AddAsync(product);
            await _context.OrderDetails.AddAsync(orderDetail);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.DeleteOrderDetailAsync(orderDetailId);

            // Assert
            Assert.True(result);
            Assert.Null(await _context.OrderDetails.FindAsync(orderDetailId));
        }

        [Fact]
        public async Task DeleteOrderDetailAsync_ReturnsFalse_WhenOrderDetailDoesNotExist()
        {
            // Act
            var result = await _service.DeleteOrderDetailAsync(Guid.NewGuid());

            // Assert
            Assert.False(result);
        }
    }
}
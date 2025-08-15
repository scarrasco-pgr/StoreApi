using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StoreApi.Data;
using StoreApi.Models.Domain;
using StoreApi.Models.DTOs.Order;
using StoreApi.Services;

namespace StoreApiUnitTests.Services
{
    public class OrderServiceTests
    {
        private readonly StoreDbContext _context;
        private readonly IMapper _mapper;
        private readonly OrderService _service;

        public OrderServiceTests()
        {
            // Set up in-memory database options
            var options = new DbContextOptionsBuilder<StoreDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new StoreDbContext(options);

            // Set up AutoMapper
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UpdateOrderDto, Order>();
            });
            _mapper = configuration.CreateMapper();

            // Initialize the service
            _service = new OrderService(_context, _mapper);
        }

        [Fact]
        public async Task GetOrdersAsync_ReturnsAllOrders()
        {
            // Arrange
            var customer = new Customer { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" };
            var product = new Product { Id = Guid.NewGuid(), Name = "Product1", Price = 100 };
            var orders = new List<Order>
            {
                new Order
                {
                    Id = Guid.NewGuid(),
                    Customer = customer,
                    OrderPlaced = DateTime.UtcNow,
                    OrderItems = new List<OrderDetail>
                    {
                        new OrderDetail { Product = product, Quantity = 2 }
                    }
                },
                new Order
                {
                    Id = Guid.NewGuid(),
                    Customer = customer,
                    OrderPlaced = DateTime.UtcNow,
                    OrderItems = new List<OrderDetail>
                    {
                        new OrderDetail { Product = product, Quantity = 5 }
                    }
                }
            };

            await _context.Customers.AddAsync(customer);
            await _context.Products.AddAsync(product);
            await _context.Orders.AddRangeAsync(orders);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetOrdersAsync();

            // Assert
            Assert.Equal(orders.Count, result.Count());
        }

        [Fact]
        public async Task GetOrderAsync_ReturnsOrder_WhenFound()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var customer = new Customer { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" };
            var product = new Product { Id = Guid.NewGuid(), Name = "Product1", Price = 100 };
            var order = new Order
            {
                Id = orderId,
                Customer = customer,
                OrderPlaced = DateTime.UtcNow,
                OrderItems = new List<OrderDetail>
                {
                    new OrderDetail { Product = product, Quantity = 3 }
                }
            };

            await _context.Customers.AddAsync(customer);
            await _context.Products.AddAsync(product);
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetOrderAsync(orderId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderId, result.Id);
            Assert.NotNull(result.Customer);
            Assert.Equal(customer.Id, result.Customer.Id);
            Assert.Single(result.OrderItems);
        }

        [Fact]
        public async Task GetOrderAsync_ReturnsNull_WhenOrderDoesNotExist()
        {
            // Act
            var result = await _service.GetOrderAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateOrderAsync_UpdatesOrderSuccessfully_WhenOrderExists()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order { Id = orderId, OrderPlaced = DateTime.UtcNow };

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            var updateDto = new UpdateOrderDto
            {
                OrderPlaced = DateTime.UtcNow.AddDays(1)
            };

            // Act
            var result = await _service.UpdateOrderAsync(orderId, updateDto);

            // Assert
            Assert.True(result);
            var updatedOrder = await _context.Orders.FindAsync(orderId);
            Assert.NotNull(updatedOrder);
            Assert.Equal(updateDto.OrderPlaced, updatedOrder.OrderPlaced);
        }

        [Fact]
        public async Task UpdateOrderAsync_ReturnsFalse_WhenOrderDoesNotExist()
        {
            // Act
            var result = await _service.UpdateOrderAsync(Guid.NewGuid(), new UpdateOrderDto { OrderPlaced = DateTime.UtcNow });

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteOrderAsync_DeletesOrderSuccessfully_WhenOrderExists()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order { Id = orderId, OrderPlaced = DateTime.UtcNow };

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.DeleteOrderAsync(orderId);

            // Assert
            Assert.True(result);
            Assert.Null(await _context.Orders.FindAsync(orderId));
        }

        [Fact]
        public async Task DeleteOrderAsync_ReturnsFalse_WhenOrderDoesNotExist()
        {
            // Act
            var result = await _service.DeleteOrderAsync(Guid.NewGuid());

            // Assert
            Assert.False(result);
        }
    }
}
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StoreApi.Data;
using StoreApi.Models.Domain;
using StoreApi.Models.DTOs.Customer;
using StoreApi.Services;

namespace StoreApiUnitTests.Services
{
    public class CustomerServiceTests
    {
        private readonly StoreDbContext _context;
        private readonly IMapper _mapper;
        private readonly CustomerService _service;

        public CustomerServiceTests()
        {
            // Set up in-memory database options
            var options = new DbContextOptionsBuilder<StoreDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new StoreDbContext(options);

            // Set up AutoMapper
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CreateCustomerDto, Customer>();
                cfg.CreateMap<UpdateCustomerDto, Customer>();
            });
            _mapper = configuration.CreateMapper();

            // Initialize the service
            _service = new CustomerService(_context, _mapper);
        }

        [Fact]
        public async Task GetCustomersAsync_ReturnsAllCustomers()
        {
            // Arrange
            var customers = new List<Customer>
            {
                new() { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" },
                new() { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Doe" }
            };
            await _context.Customers.AddRangeAsync(customers);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetCustomersAsync();

            // Assert
            Assert.Equal(customers.Count, result.Count());
        }

        [Fact]
        public async Task GetCustomerAsync_ReturnsCustomer_WhenCustomerExists()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var customer = new Customer { Id = customerId, FirstName = "John", LastName = "Doe" };
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetCustomerAsync(customerId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(customerId, result.Id);
        }

        [Fact]
        public async Task GetCustomerAsync_ReturnsNull_WhenCustomerDoesNotExist()
        {
            // Act
            var result = await _service.GetCustomerAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddCustomerAsync_AddsCustomerSuccessfully()
        {
            // Arrange
            var createDto = new CreateCustomerDto { FirstName = "New", LastName = "Customer" };

            // Act
            var result = await _service.AddCustomerAsync(createDto);

            // Assert
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.Equal(createDto.FirstName, result.FirstName);
            Assert.Equal(createDto.LastName, result.LastName);
            Assert.Single(_context.Customers);
        }

        [Fact]
        public async Task UpdateCustomerAsync_UpdatesCustomerSuccessfully_WhenCustomerExists()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var customer = new Customer { Id = customerId, FirstName = "Old", LastName = "Name" };
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();

            var updateDto = new UpdateCustomerDto { FirstName = "Updated", LastName = "Name" };

            // Act
            var result = await _service.UpdateCustomerAsync(customerId, updateDto);

            // Assert
            Assert.True(result);
            var updatedCustomer = await _context.Customers.FindAsync(customerId);
            Assert.Equal(updateDto.FirstName, updatedCustomer?.FirstName);
            Assert.Equal(updateDto.LastName, updatedCustomer?.LastName);
        }

        [Fact]
        public async Task UpdateCustomerAsync_ReturnsFalse_WhenCustomerDoesNotExist()
        {
            // Act
            var result = await _service.UpdateCustomerAsync(Guid.NewGuid(), new UpdateCustomerDto());

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteCustomerAsync_DeletesCustomerSuccessfully_WhenCustomerExists()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var customer = new Customer { Id = customerId, FirstName = "John", LastName = "Doe" };
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.DeleteCustomerAsync(customerId);

            // Assert
            Assert.True(result);
            Assert.Empty(_context.Customers);
        }

        [Fact]
        public async Task DeleteCustomerAsync_ReturnsFalse_WhenCustomerDoesNotExist()
        {
            // Act
            var result = await _service.DeleteCustomerAsync(Guid.NewGuid());

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task CreateOrderForCustomerAsync_CreatesOrderSuccessfully()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var customer = new Customer { Id = customerId, FirstName = "John", LastName = "Doe" };
            var product = new Product { Id = productId, Name = "Product1", Price = 100 };

            await _context.Customers.AddAsync(customer);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            var createOrderDto = new CustomerCreateOrderDto
            {
                Items =
                [
                    new OrderProductDto { ProductId = productId, Quantity = 2 }
                ]
            };

            // Act
            var result = await _service.CreateOrderForCustomerAsync(customerId, createOrderDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(customerId, result.CustomerId);
            Assert.Single(result.OrderItems);
            Assert.Equal(productId, result.OrderItems.First().ProductId);
        }

        [Fact]
        public async Task CreateOrderForCustomerAsync_ReturnsNull_WhenCustomerDoesNotExist()
        {
            // Arrange
            var createOrderDto = new CustomerCreateOrderDto
            {
                Items = new List<OrderProductDto>
                {
                    new OrderProductDto { ProductId = Guid.NewGuid(), Quantity = 2 }
                }
            };

            // Act
            var result = await _service.CreateOrderForCustomerAsync(Guid.NewGuid(), createOrderDto);

            // Assert
            Assert.Null(result);
        }
    }
}
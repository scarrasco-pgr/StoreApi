using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using StoreApi.Controllers;
using StoreApi.Models.Domain;
using StoreApi.Models.DTOs.Customer;
using StoreApi.Services;

namespace StoreApiUnitTests.Controllers
{
    public class CustomersControllerTests
    {
        private readonly ICustomerService _customerService;
        private readonly CustomersController _controller;

        public CustomersControllerTests()
        {
            _customerService = Substitute.For<ICustomerService>();
            _controller = new CustomersController(_customerService);
        }

        [Fact]
        public async Task GetCustomers_ReturnsOkResult_WithListOfCustomers()
        {
            // Arrange
            var customers = new List<Customer>
            {
                new() { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" },
                new() { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Doe" }
            };
            _customerService.GetCustomersAsync().Returns(customers);

            // Act
            var result = await _controller.GetCustomers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCustomers = Assert.IsAssignableFrom<IEnumerable<Customer>>(okResult.Value);
            Assert.Equal(customers.Count, returnedCustomers.Count());
        }

        [Fact]
        public async Task GetCustomer_ReturnsCustomer_WhenCustomerExists()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var customer = new Customer { Id = customerId, FirstName = "John", LastName = "Doe" };
            _customerService.GetCustomerAsync(customerId).Returns(customer);

            // Act
            var result = await _controller.GetCustomer(customerId);

            // Assert
            var returnedCustomer = Assert.IsType<Customer>(result.Value);
            Assert.Equal(customerId, returnedCustomer.Id);
        }

        [Fact]
        public async Task GetCustomer_ReturnsNotFound_WhenCustomerDoesNotExist()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            _customerService.GetCustomerAsync(customerId).Returns((Customer?)null);

            // Act
            var result = await _controller.GetCustomer(customerId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetCustomerOrders_ReturnsOkResult_WithListOfOrders()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var orders = new List<Order>
            {
                new() { Id = Guid.NewGuid(), CustomerId = customerId },
                new() { Id = Guid.NewGuid(), CustomerId = customerId }
            };
            _customerService.GetCustomerAsync(customerId).Returns(new Customer { Id = customerId });
            _customerService.GetCustomerOrdersAsync(customerId).Returns(orders);

            // Act
            var result = await _controller.GetCustomerOrders(customerId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedOrders = Assert.IsAssignableFrom<IEnumerable<Order>>(okResult.Value);
            Assert.Equal(orders.Count, returnedOrders.Count());
        }

        [Fact]
        public async Task GetCustomerOrders_ReturnsNotFound_WhenCustomerDoesNotExist()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            _customerService.GetCustomerAsync(customerId).Returns((Customer?)null);

            // Act
            var result = await _controller.GetCustomerOrders(customerId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task PutCustomer_ReturnsNoContent_WhenUpdateIsSuccessful()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var updateDto = new UpdateCustomerDto { FirstName = "John", LastName = "Doe" };
            _customerService.UpdateCustomerAsync(customerId, updateDto).Returns(true);

            // Act
            var result = await _controller.PutCustomer(customerId, updateDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PutCustomer_ReturnsNotFound_WhenUpdateFails()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var updateDto = new UpdateCustomerDto { FirstName = "John", LastName = "Doe" };
            _customerService.UpdateCustomerAsync(customerId, updateDto).Returns(false);

            // Act
            var result = await _controller.PutCustomer(customerId, updateDto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task PostCustomer_ReturnsCreatedAtActionResult_WithCreatedCustomer()
        {
            // Arrange
            var createDto = new CreateCustomerDto { FirstName = "John", LastName = "Doe" };
            var createdCustomer = new Customer { Id = Guid.NewGuid(), FirstName = createDto.FirstName, LastName = createDto.LastName };
            _customerService.AddCustomerAsync(createDto).Returns(createdCustomer);

            // Act
            var result = await _controller.PostCustomer(createDto);

            // Assert
            var createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedCustomer = Assert.IsType<Customer>(createdAtResult.Value);
            Assert.Equal(createdCustomer.Id, returnedCustomer.Id);
        }

        [Fact]
        public async Task DeleteCustomer_ReturnsNoContent_WhenDeleteIsSuccessful()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            _customerService.DeleteCustomerAsync(customerId).Returns(true);

            // Act
            var result = await _controller.DeleteCustomer(customerId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteCustomer_ReturnsNotFound_WhenDeleteFails()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            _customerService.DeleteCustomerAsync(customerId).Returns(false);

            // Act
            var result = await _controller.DeleteCustomer(customerId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CustomerCreateOrder_ReturnsCreatedAtActionResult_WithCreatedOrder()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var createOrderDto = new CustomerCreateOrderDto
            {
                Items = [new() { ProductId = Guid.NewGuid(), Quantity = 1 }]
            };
            var createdOrder = new Order { Id = Guid.NewGuid(), CustomerId = customerId };
            _customerService.CreateOrderForCustomerAsync(customerId, createOrderDto).Returns(createdOrder);

            // Act
            var result = await _controller.CustomerCreateOrder(customerId, createOrderDto);

            // Assert
            var createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedOrder = Assert.IsType<Order>(createdAtResult.Value);
            Assert.Equal(createdOrder.Id, returnedOrder.Id);
        }

        [Fact]
        public async Task CustomerCreateOrder_ReturnsBadRequest_WhenOrderCreationFails()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var createOrderDto = new CustomerCreateOrderDto
            {
                Items = [new() { ProductId = Guid.NewGuid(), Quantity = 1 }]
            };
            _customerService.CreateOrderForCustomerAsync(customerId, createOrderDto).Returns((Order?)null);

            // Act
            var result = await _controller.CustomerCreateOrder(customerId, createOrderDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Invalid customer or product IDs.", badRequestResult.Value);
        }
    }
}
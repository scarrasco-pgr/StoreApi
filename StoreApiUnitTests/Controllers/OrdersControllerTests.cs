using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using StoreApi.Controllers;
using StoreApi.Models.Domain;
using StoreApi.Models.DTOs.Order;
using StoreApi.Services;

namespace StoreApiUnitTests.Controllers
{
    public class OrdersControllerTests
    {
        private readonly IOrderService _service;
        private readonly OrdersController _controller;

        public OrdersControllerTests()
        {
            _service = Substitute.For<IOrderService>();
            _controller = new OrdersController(_service);
        }

        [Fact]
        public async Task GetOrders_ReturnsOkResult_WithListOfOrders()
        {
            // Arrange
            var orders = new List<Order>
            {
                new() { Id = Guid.NewGuid() },
                new() { Id = Guid.NewGuid() }
            };
            _service.GetOrdersAsync().Returns(orders);

            // Act
            var result = await _controller.GetOrders();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedOrders = Assert.IsAssignableFrom<IEnumerable<Order>>(okResult.Value);
            Assert.Equal(orders.Count, returnedOrders.Count());
        }

        [Fact]
        public async Task GetOrder_ReturnsOkResult_WithOrder_WhenFound()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order { Id = orderId };
            _service.GetOrderAsync(orderId).Returns(order);

            // Act
            var result = await _controller.GetOrder(orderId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedOrder = Assert.IsType<Order>(okResult.Value);
            Assert.Equal(orderId, returnedOrder.Id);
        }

        [Fact]
        public async Task GetOrder_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            _service.GetOrderAsync(orderId).Returns((Order?)null);

            // Act
            var result = await _controller.GetOrder(orderId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task PutOrder_ReturnsNoContent_WhenUpdateIsSuccessful()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var updateDto = new UpdateOrderDto { };
            _service.UpdateOrderAsync(orderId, updateDto).Returns(true);

            // Act
            var result = await _controller.PutOrder(orderId, updateDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PutOrder_ReturnsNotFound_WhenUpdateFails()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var updateDto = new UpdateOrderDto { };
            _service.UpdateOrderAsync(orderId, updateDto).Returns(false);

            // Act
            var result = await _controller.PutOrder(orderId, updateDto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteOrder_ReturnsNoContent_WhenDeleteIsSuccessful()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            _service.DeleteOrderAsync(orderId).Returns(true);

            // Act
            var result = await _controller.DeleteOrder(orderId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteOrder_ReturnsNotFound_WhenDeleteFails()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            _service.DeleteOrderAsync(orderId).Returns(false);

            // Act
            var result = await _controller.DeleteOrder(orderId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
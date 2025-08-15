using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using StoreApi.Controllers;
using StoreApi.Models.Domain;
using StoreApi.Models.DTOs.OrderDetail;
using StoreApi.Services;

namespace StoreApiUnitTests.Controllers
{
    public class OrderDetailsControllerTests
    {
        private readonly IOrderDetailService _service;
        private readonly OrderDetailsController _controller;

        public OrderDetailsControllerTests()
        {
            _service = Substitute.For<IOrderDetailService>();
            _controller = new OrderDetailsController(_service);
        }

        [Fact]
        public async Task GetOrderDetails_ReturnsOkResult_WithListOfOrderDetails()
        {
            // Arrange
            var orderDetails = new List<OrderDetail>
            {
                new() { Id = Guid.NewGuid(), Quantity = 2 },
                new() { Id = Guid.NewGuid(), Quantity = 5 }
            };
            _service.GetOrderDetailsAsync().Returns(orderDetails);

            // Act
            var result = await _controller.GetOrderDetails();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedOrderDetails = Assert.IsAssignableFrom<IEnumerable<OrderDetail>>(okResult.Value);
            Assert.Equal(orderDetails.Count, returnedOrderDetails.Count());
        }

        [Fact]
        public async Task GetOrderDetail_ReturnsOkResult_WithOrderDetail_WhenFound()
        {
            // Arrange
            var orderDetailId = Guid.NewGuid();
            var orderDetail = new OrderDetail { Id = orderDetailId, Quantity = 3 };
            _service.GetOrderDetailAsync(orderDetailId).Returns(orderDetail);

            // Act
            var result = await _controller.GetOrderDetail(orderDetailId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedOrderDetail = Assert.IsType<OrderDetail>(okResult.Value);
            Assert.Equal(orderDetailId, returnedOrderDetail.Id);
        }

        [Fact]
        public async Task GetOrderDetail_ReturnsNotFound_WhenOrderDetailDoesNotExist()
        {
            // Arrange
            var orderDetailId = Guid.NewGuid();
            _service.GetOrderDetailAsync(orderDetailId).Returns((OrderDetail?)null);

            // Act
            var result = await _controller.GetOrderDetail(orderDetailId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task PutOrderDetail_ReturnsNoContent_WhenUpdateIsSuccessful()
        {
            // Arrange
            var orderDetailId = Guid.NewGuid();
            var updateDto = new UpdateOrderDetailDto { Quantity = 10 };
            _service.UpdateOrderDetailAsync(orderDetailId, updateDto).Returns(true);

            // Act
            var result = await _controller.PutOrderDetail(orderDetailId, updateDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PutOrderDetail_ReturnsNotFound_WhenUpdateFails()
        {
            // Arrange
            var orderDetailId = Guid.NewGuid();
            var updateDto = new UpdateOrderDetailDto { Quantity = 10 };
            _service.UpdateOrderDetailAsync(orderDetailId, updateDto).Returns(false);

            // Act
            var result = await _controller.PutOrderDetail(orderDetailId, updateDto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteOrderDetail_ReturnsNoContent_WhenDeleteIsSuccessful()
        {
            // Arrange
            var orderDetailId = Guid.NewGuid();
            _service.DeleteOrderDetailAsync(orderDetailId).Returns(true);

            // Act
            var result = await _controller.DeleteOrderDetail(orderDetailId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteOrderDetail_ReturnsNotFound_WhenDeleteFails()
        {
            // Arrange
            var orderDetailId = Guid.NewGuid();
            _service.DeleteOrderDetailAsync(orderDetailId).Returns(false);

            // Act
            var result = await _controller.DeleteOrderDetail(orderDetailId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
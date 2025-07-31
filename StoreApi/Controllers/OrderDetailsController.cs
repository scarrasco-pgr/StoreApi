using Microsoft.AspNetCore.Mvc;
using StoreApi.Models.Domain;
using StoreApi.Models.DTOs.OrderDetail;
using StoreApi.Services;

namespace StoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailsController(IOrderDetailService service) : ControllerBase
    {
        private readonly IOrderDetailService _service = service;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDetail>>> GetOrderDetails()
        {
            var orderDetails = await _service.GetOrderDetailsAsync();
            return Ok(orderDetails);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDetail>> GetOrderDetail(Guid id)
        {
            var orderDetail = await _service.GetOrderDetailAsync(id);
            if (orderDetail == null)
                return NotFound();
            return Ok(orderDetail);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderDetail(Guid id, UpdateOrderDetailDto dto)
        {
            var updated = await _service.UpdateOrderDetailAsync(id, dto);
            if (!updated)
                return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderDetail(Guid id)
        {
            var deleted = await _service.DeleteOrderDetailAsync(id);
            if (!deleted)
                return NotFound();
            return NoContent();
        }
    }
}

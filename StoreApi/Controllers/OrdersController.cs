using Microsoft.AspNetCore.Mvc;
using StoreApi.Models.Domain;
using StoreApi.Models.DTOs.Order;
using StoreApi.Services;

namespace StoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController(IOrderService service) : ControllerBase
    {
        private readonly IOrderService _service = service;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var orders = await _service.GetOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Order>> GetOrder(Guid id)
        {
            var order = await _service.GetOrderAsync(id);
            if (order == null)
                return NotFound();
            return Ok(order);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> PutOrder(Guid id, UpdateOrderDto dto)
        {
            var updated = await _service.UpdateOrderAsync(id, dto);
            if (!updated)
                return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            var deleted = await _service.DeleteOrderAsync(id);
            if (!deleted)
                return NotFound();
            return NoContent();
        }
    }
}

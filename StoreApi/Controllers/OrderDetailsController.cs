using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreApi.Data;
using StoreApi.Models.Domain;

namespace StoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailsController(StoreDbContext context) : ControllerBase
    {
        private readonly StoreDbContext _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDetail>>> GetOrderDetails()
        {
            return await _context.OrderDetails.Include(od => od.Product)
                .Include(od => od.Product)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDetail>> GetOrderDetail(Guid id)
        {
            var orderDetail = await _context.OrderDetails.FindAsync(id);

            if (orderDetail == null)
            {
                return NotFound();
            }

            return orderDetail;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderDetail(Guid id, OrderDetail orderDetail)
        {
            if (id != orderDetail.Id)
            {
                return BadRequest();
            }

            _context.Entry(orderDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderDetailExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<OrderDetail>> PostOrderDetail(OrderDetail orderDetail)
        {
            _context.OrderDetails.Add(orderDetail);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrderDetail", new { id = orderDetail.Id }, orderDetail);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderDetail(Guid id)
        {
            var orderDetail = await _context.OrderDetails.FindAsync(id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            _context.OrderDetails.Remove(orderDetail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderDetailExists(Guid id)
        {
            return _context.OrderDetails.Any(e => e.Id == id);
        }
    }
}

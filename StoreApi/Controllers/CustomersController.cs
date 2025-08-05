using Microsoft.AspNetCore.Mvc;
using StoreApi.Models.Domain;
using StoreApi.Models.DTOs.Customer;
using StoreApi.Services;

namespace StoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController(ICustomerService customerService) : ControllerBase
    {
        private readonly ICustomerService _customerService = customerService;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
            => Ok(await _customerService.GetCustomersAsync());

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Customer>> GetCustomer(Guid id)
        {
            var customer = await _customerService.GetCustomerAsync(id);
            if (customer == null) return NotFound();
            return customer;
        }

        [HttpGet("{id:guid}/Orders")]
        public async Task<ActionResult<IEnumerable<Order>>> GetCustomerOrders(Guid id)
        {
            var customer = await _customerService.GetCustomerAsync(id);
            if (customer == null) return NotFound();
            return Ok(await _customerService.GetCustomerOrdersAsync(id));
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> PutCustomer(Guid id, UpdateCustomerDto customerDto)
        {
            var success = await _customerService.UpdateCustomerAsync(id, customerDto);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(CreateCustomerDto customerDto)
        {
            var customer = await _customerService.AddCustomerAsync(customerDto);
            return CreatedAtAction("GetCustomer", new { id = customer.Id }, customer);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            var success = await _customerService.DeleteCustomerAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpPost("{id:guid}/Orders")]
        public async Task<ActionResult<Order>> CustomerCreateOrder(Guid id, [FromBody] CustomerCreateOrderDto dto)
        {
            var order = await _customerService.CreateOrderForCustomerAsync(id, dto);
            if (order == null) return BadRequest("Invalid customer or product IDs.");
            return CreatedAtAction(nameof(GetCustomerOrders), new { id = order.CustomerId }, order);
        }
    }
}

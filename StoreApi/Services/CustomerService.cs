using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using StoreApi.Data;
using StoreApi.Models.Domain;
using StoreApi.Models.DTOs.Customer;

namespace StoreApi.Services
{
    public class CustomerService(StoreDbContext context, IMapper mapper, ICustomerValidationService validator) : ICustomerService
    {
        private readonly StoreDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly ICustomerValidationService _validator = validator;

        public async Task<IEnumerable<Customer>> GetCustomersAsync()
            => await _context.Customers.ToListAsync();

        public async Task<Customer?> GetCustomerAsync(Guid id)
            => await _context.Customers.FindAsync(id);

        public async Task<IEnumerable<Order>> GetCustomerOrdersAsync(Guid customerId)
            => await _context.Orders.Where(o => o.CustomerId == customerId).ToListAsync();

        public async Task<Customer> AddCustomerAsync(CreateCustomerDto dto)
        {
            await _validator.ValidateCreateCustomer(dto);
            var customer = _mapper.Map<Customer>(dto);
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task<bool> UpdateCustomerAsync(Guid id, UpdateCustomerDto dto)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return false;
            _mapper.Map(dto, customer);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCustomerAsync(Guid id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return false;
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Order?> CreateOrderForCustomerAsync(Guid customerId, CustomerCreateOrderDto dto)
        {
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null) return null;

            var productIds = dto.Items.Select(i => i.ProductId).ToList();
            var products = await _context.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();
            if (products.Count != productIds.Count) return null;

            var order = new Order
            {
                CustomerId = customerId,
                OrderPlaced = DateTime.UtcNow,
                OrderItems = [.. dto.Items.Select(i => new OrderDetail
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                })]
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == order.Id);
        }
    }
}
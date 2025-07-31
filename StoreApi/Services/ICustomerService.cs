using StoreApi.Models.Domain;
using StoreApi.Models.DTOs.Customer;

namespace StoreApi.Services
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> GetCustomersAsync();
        Task<Customer?> GetCustomerAsync(Guid id);
        Task<IEnumerable<Order>> GetCustomerOrdersAsync(Guid customerId);
        Task<Customer> AddCustomerAsync(CreateCustomerDto dto);
        Task<bool> UpdateCustomerAsync(Guid id, UpdateCustomerDto dto);
        Task<bool> DeleteCustomerAsync(Guid id);
        Task<Order?> CreateOrderForCustomerAsync(Guid customerId, CustomerCreateOrderDto dto);
    }
}
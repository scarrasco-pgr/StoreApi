using StoreApi.Models.Domain;
using StoreApi.Models.DTOs.Order;

namespace StoreApi.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetOrdersAsync();
        Task<Order?> GetOrderAsync(Guid id);
        Task<bool> UpdateOrderAsync(Guid id, UpdateOrderDto dto);
        Task<bool> DeleteOrderAsync(Guid id);
    }
}
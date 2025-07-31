using StoreApi.Models.Domain;
using StoreApi.Models.DTOs.OrderDetail;

namespace StoreApi.Services
{
    public interface IOrderDetailService
    {
        Task<IEnumerable<OrderDetail>> GetOrderDetailsAsync();
        Task<OrderDetail?> GetOrderDetailAsync(Guid id);
        Task<bool> UpdateOrderDetailAsync(Guid id, UpdateOrderDetailDto dto);
        Task<bool> DeleteOrderDetailAsync(Guid id);
    }
}
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StoreApi.Data;
using StoreApi.Models.Domain;
using StoreApi.Models.DTOs.OrderDetail;

namespace StoreApi.Services
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly StoreDbContext _context;
        private readonly IMapper _mapper;

        public OrderDetailService(StoreDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrderDetail>> GetOrderDetailsAsync()
        {
            return await _context.OrderDetails
                .Include(od => od.Product)
                .ToListAsync();
        }

        public async Task<OrderDetail?> GetOrderDetailAsync(Guid id)
        {
            return await _context.OrderDetails.FindAsync(id);
        }

        public async Task<bool> UpdateOrderDetailAsync(Guid id, UpdateOrderDetailDto dto)
        {
            var orderDetail = await _context.OrderDetails.FindAsync(id);
            if (orderDetail == null) return false;

            _mapper.Map(dto, orderDetail);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteOrderDetailAsync(Guid id)
        {
            var orderDetail = await _context.OrderDetails.FindAsync(id);
            if (orderDetail == null) return false;

            _context.OrderDetails.Remove(orderDetail);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
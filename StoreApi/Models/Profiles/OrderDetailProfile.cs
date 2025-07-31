using AutoMapper;
using StoreApi.Models.Domain;
using StoreApi.Models.DTOs.OrderDetail;

namespace StoreApi.Models.Profiles
{
    public class OrderDetailProfile : Profile
    {
        public OrderDetailProfile()
        {
            CreateMap<UpdateOrderDetailDto, OrderDetail>();
        }
    }
}

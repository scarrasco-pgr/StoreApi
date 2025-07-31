using AutoMapper;
using StoreApi.Models.Domain;
using StoreApi.Models.DTOs.Order;

namespace StoreApi.Models.Profiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<UpdateOrderDto, Order>();
        }
    }
}

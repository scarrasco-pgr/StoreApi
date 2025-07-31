using AutoMapper;
using StoreApi.Models.Domain;
using StoreApi.Models.DTOs.Customer;

namespace StoreApi.Models.Profiles
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<CreateCustomerDto, Customer>();
            CreateMap<UpdateCustomerDto, Customer>();
        }
    }
}
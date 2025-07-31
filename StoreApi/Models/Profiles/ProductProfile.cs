using AutoMapper;
using StoreApi.Models.Domain;
using StoreApi.Models.DTOs.Product;

namespace StoreApi.Models.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<CreateProductDto, Product>();
            CreateMap<UpdateProductDto, Product>();
        }
    }
}

using StoreApi.Models.Domain;
using StoreApi.Models.DTOs.Product;

namespace StoreApi.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetProductsAsync();
        Task<Product?> GetProductAsync(Guid id);
        Task<Product> CreateProductAsync(CreateProductDto dto);
        Task<bool> UpdateProductAsync(Guid id, UpdateProductDto dto);
        Task<bool> DeleteProductAsync(Guid id);
    }
}
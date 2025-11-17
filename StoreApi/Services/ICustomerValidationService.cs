using StoreApi.Models.DTOs.Customer;

namespace StoreApi.Services
{
    public interface ICustomerValidationService
    {
        Task ValidateCreateCustomer(CreateCustomerDto dto);
    }
}
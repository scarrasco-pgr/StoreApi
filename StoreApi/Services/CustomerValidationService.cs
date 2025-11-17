using FluentValidation;
using StoreApi.Models.DTOs.Customer;

namespace StoreApi.Services
{
    public class CustomerValidationService(IValidator<CreateCustomerDto> validator) : ICustomerValidationService
    {
        private readonly IValidator<CreateCustomerDto> _validator = validator;

        public async Task ValidateCreateCustomer(CreateCustomerDto dto)
        {
            var result = await _validator.ValidateAsync(dto);
            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }
        }
    }
}

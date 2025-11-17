using FluentValidation;
using StoreApi.Models.DTOs.Customer;

namespace StoreApi.Validators.Customer
{
    public class CreateCustomerValidator : AbstractValidator<CreateCustomerDto>
    {
        public CreateCustomerValidator()
        {
            RuleFor(customer => customer.FirstName).NotEmpty();
            RuleFor(customer => customer.LastName).NotEmpty();

        }
    }
}

using FluentValidation;
using StoreApi.Models.DTOs.Customer;
using StoreApi.Services;
using StoreApi.Validators.Customer;

namespace StoreApiUnitTests.Services
{
    public class CustomerValidationServiceTests
    {
        private readonly IValidator<CreateCustomerDto> _validator;
        private readonly CustomerValidationService _service;

        public CustomerValidationServiceTests()
        {
            _validator = new CreateCustomerValidator();
            _service = new CustomerValidationService(_validator);
        }

        [Fact]
        public async Task ValidateCreateCustomer_ValidDto_DoesNotThrow()
        {
            // Arrange
            var dto = new CreateCustomerDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "1234567890",
                Address = "123 Main St"
            };

            // Act & Assert
            await _service.ValidateCreateCustomer(dto);
        }

        [Fact]
        public async Task ValidateCreateCustomer_InvalidDto_ThrowsValidationException()
        {
            // Arrange
            var dto = new CreateCustomerDto
            {
                FirstName = "",
                LastName = "",
                Email = "not-an-email",
                PhoneNumber = "",
                Address = ""
            };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _service.ValidateCreateCustomer(dto));
        }
    }
}
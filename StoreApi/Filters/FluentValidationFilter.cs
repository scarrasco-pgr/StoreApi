using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace StoreApi.Filters
{
    public class FluentValidationFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is ValidationException validationException)
            {
                var errors = validationException.Errors.Select(err => new
                {
                    Property = err.PropertyName,
                    Error = err.ErrorMessage
                });

                context.Result = new BadRequestObjectResult(new
                {
                    Message = "Validation failed",
                    Errors = errors
                });

                context.ExceptionHandled = true;
            }
        }
    }
}

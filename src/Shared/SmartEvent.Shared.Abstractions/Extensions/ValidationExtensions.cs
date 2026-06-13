using FluentValidation;
using SmartEvent.Shared.Abstractions.Models;

namespace SmartEvent.Shared.Abstractions.Extensions
{
    public static class ValidationExtensions
    {
        public static async Task<ApiResponse<T>?> ValidateAndRespond<T>(
            this IValidator<T> validator,
            T request)
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return ApiResponse<T>.Fail("Validation failed", errors);
            }

            return null; // All is ok, continue
        }
    }
}

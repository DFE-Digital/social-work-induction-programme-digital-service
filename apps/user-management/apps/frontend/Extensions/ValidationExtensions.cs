using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Dfe.Sww.Ecf.Frontend.Extensions;

/// <summary>
/// Extension methods for Fluent Validation
/// </summary>
public static class ValidationExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="result"></param>
    /// <param name="modelState"></param>
    /// <param name="prefix"></param>
    public static void AddToModelState(
        this ValidationResult result,
        ModelStateDictionary modelState,
        string? prefix = null
    )
    {
        foreach (var error in result.Errors)
        {
            var key = string.IsNullOrEmpty(prefix)
                ? error.PropertyName
                : $"{prefix}.{error.PropertyName}";

            modelState.AddModelError(key, error.ErrorMessage);
        }
    }
}

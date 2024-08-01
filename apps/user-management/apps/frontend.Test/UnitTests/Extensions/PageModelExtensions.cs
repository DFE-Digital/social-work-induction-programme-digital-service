using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Extensions;

public static class PageModelExtensions
{
    /// <summary>
    /// Allows the running of AspNetCore validation rules in unit tests.
    /// Validation typically happens before a page model's handler methods are called, so we have to manually
    /// invoke the validation when calling handler methods directly.
    /// </summary>
    /// <param name="model"></param>
    /// <typeparam name="T"></typeparam>
    public static void ValidateModel<T>(this T model)
        where T : PageModel
    {
        model.ModelState.Clear();

        var validationContext = new ValidationContext(model, null, null);
        var validationResults = new List<ValidationResult>();
        if (Validator.TryValidateObject(model, validationContext, validationResults, true))
            return;
        foreach (var validationResult in validationResults)
        {
            foreach (var memberName in validationResult.MemberNames)
            {
                model.ModelState.AddModelError(memberName, validationResult.ErrorMessage ?? "");
            }
        }
    }
}

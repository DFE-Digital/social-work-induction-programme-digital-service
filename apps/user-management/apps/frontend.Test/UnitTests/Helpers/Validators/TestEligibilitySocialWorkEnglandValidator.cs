using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;
using Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;
using Dfe.Sww.Ecf.Frontend.Validation;
using FluentValidation;
using FluentValidation.Results;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Validators;

internal sealed class TestEligibilitySocialWorkEnglandValidator : EligibilitySocialWorkEnglandValidator
{
    public TestEligibilitySocialWorkEnglandValidator(IAuthServiceClient auth)
        : base(auth) { }

    public override Task<ValidationResult> ValidateAsync(
        ValidationContext<EligibilitySocialWorkEngland> context,
        CancellationToken cancellation = default)
    {
        context.RootContextData["SkipSweIdUnique"] = false;
        return base.ValidateAsync(context, cancellation);
    }
}

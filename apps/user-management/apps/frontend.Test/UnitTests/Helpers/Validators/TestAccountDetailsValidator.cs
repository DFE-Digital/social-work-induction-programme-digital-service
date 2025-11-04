using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Validation;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Validators;

internal sealed class TestAccountDetailsValidator : AccountDetailsValidator
{
    public TestAccountDetailsValidator(IAccountService accountService, IAuthServiceClient auth)
        : base(accountService, auth) { }

    public override Task<FluentValidation.Results.ValidationResult> ValidateAsync(
        FluentValidation.ValidationContext<AccountDetails> context,
        CancellationToken cancellation = default)
    {
        context.RootContextData["SkipSweIdUnique"] = false;
        return base.ValidateAsync(context, cancellation);
    }
}

using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Validation.Common;
using FluentValidation;

namespace Dfe.Sww.Ecf.Frontend.Validation;

/// <summary>
///     Validation for the account model
/// </summary>
public class AccountDetailsValidator : AbstractValidator<AccountDetails>
{
    public AccountDetailsValidator(IAccountService accountService, IAuthServiceClient authServiceClient)
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(y => y.FirstName).FirstNameValidation();
        RuleFor(y => y.LastName).LastNameValidation();
        RuleFor(y => y.Email).EmailValidation((email, _) => accountService.CheckEmailExistsAsync(email));
        When(
            x => x.Types?.Contains(AccountType.Assessor) == true || x.Types?.Contains(AccountType.EarlyCareerSocialWorker) == true,
            () => { RuleFor(y => y.SocialWorkEnglandNumber).SocialWorkEnglandNumberValidation(authServiceClient); }
        );
    }
}

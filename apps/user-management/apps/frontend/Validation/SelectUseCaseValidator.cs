using Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;
using FluentValidation;

namespace Dfe.Sww.Ecf.Frontend.Validation;

public class SelectUseCaseValidator : AbstractValidator<SelectUseCase>
{
    public SelectUseCaseValidator()
    {
        RuleFor(model => model.SelectedAccountTypes)
            .NotEmpty()
            .WithMessage("Select what they need to do");
    }
}

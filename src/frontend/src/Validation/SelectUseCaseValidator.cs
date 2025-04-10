using FluentValidation;
using SocialWorkInductionProgramme.Frontend.Pages.ManageAccounts;

namespace SocialWorkInductionProgramme.Frontend.Validation;

public class SelectUseCaseValidator : AbstractValidator<SelectUseCase>
{
    public SelectUseCaseValidator()
    {
        RuleFor(model => model.SelectedAccountTypes)
            .NotEmpty()
            .WithMessage("Select what they need to do");
    }
}

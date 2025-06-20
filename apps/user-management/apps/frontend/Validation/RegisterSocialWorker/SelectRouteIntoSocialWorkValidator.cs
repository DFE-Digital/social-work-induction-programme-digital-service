using Dfe.Sww.Ecf.Frontend.Models.RegisterSocialWorker;
using Dfe.Sww.Ecf.Frontend.Pages.SocialWorkerRegistration;
using FluentValidation;

namespace Dfe.Sww.Ecf.Frontend.Validation.RegisterSocialWorker;

public class SelectRouteIntoSocialWorkValidator : AbstractValidator<SelectRouteIntoSocialWork>
{
    public SelectRouteIntoSocialWorkValidator()
    {
        RuleFor(model => model.SelectedRouteIntoSocialWork)
            .NotEmpty()
            .WithMessage("Select your entry route into social work");

        When(
            x => x.SelectedRouteIntoSocialWork == RouteIntoSocialWork.Other,
            () => { RuleFor(y => y.OtherRouteIntoSocialWork)
                .NotEmpty()
                .WithMessage("Enter your entry route"); }
        );
    }
}

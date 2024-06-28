using DfeSwwEcf.NotificationService.Models;
using FluentValidation;

namespace DfeSwwEcf.NotificationService.Validation;

/// <summary>
/// Validation for the notifcation request body
/// </summary>
public class NotificationRequestValidator : AbstractValidator<NotificationRequest>
{
    public NotificationRequestValidator()
    {
        RuleFor(x => x.TemplateId).NotEmpty();

        RuleFor(y => y.EmailAddress).NotEmpty();
        RuleFor(y => y.EmailAddress).EmailAddress();
    }
}

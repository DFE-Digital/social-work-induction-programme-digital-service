using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Dfe.Sww.Ecf.Frontend.TagHelpers;

public class UserDetailsTagHelper : TagHelper
{
    public User? User { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "";
        output.TagMode = TagMode.StartTagAndEndTag;

        if (User is null)
            return;

        output.Content.SetHtmlContent($"<p>{User.FullName}</p><p>{User.Email}</p>");

        if (User.Types is null || !User.Types.Contains(UserType.EarlyCareerSocialWorker))
        {
            return;
        }

        if (string.IsNullOrEmpty(User.SocialWorkEnglandNumber))
        {
            output.Content.AppendHtml(
                "<p>"
                    + "<govuk-tag class=\"govuk-tag govuk-tag--orange\">" + UserStatus.PendingRegistration.GetDisplayName() + "</govuk-tag>"
                    + "<span class=\"govuk-!-display-block govuk-hint govuk-!-margin-bottom-0\">"
                    + "You have not provided a Social Work England registration number for this account"
                    + "</span>"
                    + "</p>"
            );
            return;
        }

        output.Content.AppendHtml(
            $"<p>SWE registration number {User.SocialWorkEnglandNumber}</p>"
        );
    }
}

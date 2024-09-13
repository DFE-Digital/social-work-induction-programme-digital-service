using Dfe.Sww.Ecf.Frontend.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Dfe.Sww.Ecf.Frontend.TagHelpers;

public class AccountDetailsTagHelper : TagHelper
{
    public Account? Account { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "";
        output.TagMode = TagMode.StartTagAndEndTag;

        if (Account is null)
            return;

        output.Content.SetHtmlContent($"<p>{Account.FullName}</p><p>{Account.Email}</p>");

        if (Account.Types is null || !Account.Types.Contains(AccountType.EarlyCareerSocialWorker))
        {
            return;
        }

        if (string.IsNullOrEmpty(Account.SocialWorkEnglandNumber))
        {
            output.Content.AppendHtml(
                "<p>"
                    + "<govuk-tag class=\"govuk-tag govuk-tag--orange\">" + AccountStatus.PendingRegistration.GetDisplayName() + "</govuk-tag>"
                    + "<span class=\"govuk-!-display-block govuk-hint govuk-!-margin-bottom-0\">"
                    + "You have not provided a Social Work England registration number for this account"
                    + "</span>"
                    + "</p>"
            );
            return;
        }

        output.Content.AppendHtml(
            $"<p>SWE registration number {Account.SocialWorkEnglandNumber}</p>"
        );
    }
}

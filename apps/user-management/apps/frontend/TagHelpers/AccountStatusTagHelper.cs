using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Dfe.Sww.Ecf.Frontend.TagHelpers;

public class AccountStatusTagHelper : TagHelper
{
    public AccountStatus? Status { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = null;

        if (Status == null) return;

        var tagClass = Status switch
        {
            AccountStatus.Active => "govuk-tag--green",
            AccountStatus.PendingRegistration => "govuk-tag--grey",
            AccountStatus.Inactive => "govuk-tag--yellow",
            _ => "govuk-tag--grey"
        };

        output.Content.SetHtmlContent(
            $"<strong class=\"govuk-tag {tagClass}\" data-test-class=\"account-status\">{Status.GetDisplayName()}</strong>");
    }
}

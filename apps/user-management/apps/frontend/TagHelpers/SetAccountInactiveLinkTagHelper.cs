using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Dfe.Sww.Ecf.Frontend.TagHelpers;

public class SetAccountInactiveLinkTagHelper : TagHelper
{
    public string? AccountName { get; set; }

    public string? Href { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "p";
        output.TagMode = TagMode.StartTagAndEndTag;
        output.AddClass("govuk-body", HtmlEncoder.Default);

        output.Content.SetHtmlContent(
            $"<a class=\"govuk-link govuk-link--no-visited-state\" data-test-id=\"unlink\" href=\"{Href}\">{AccountName} is leaving this organisation</a>"
                + "<span class=\"govuk-hint govuk-!-display-block\">"
                + "This will unlink this account from this organisation. It will not delete the account or any learner records. "
                + "Coordinators from other organisations will be able to link this account to their own PQP service."
                + "</span>"
        );
    }
}

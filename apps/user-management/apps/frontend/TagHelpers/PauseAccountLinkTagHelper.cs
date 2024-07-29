using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Dfe.Sww.Ecf.Frontend.TagHelpers;

public class PauseAccountLinkTagHelper : TagHelper
{
    public string? AccountName { get; set; }

    public string? Href { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "p";
        output.TagMode = TagMode.StartTagAndEndTag;
        output.AddClass("govuk-body", HtmlEncoder.Default);

        output.Content.SetHtmlContent(
            $"<a class=\"govuk-link govuk-link--no-visited-state\" href=\"{Href}\">{AccountName} is taking a break from the PQP programme</a>"
                + "<span class=\"govuk-hint govuk-!-display-block\">"
                + "This will temporarily pause this account while the staff member is taking a break from the programme (for example, parental leave or other leave of absence). "
                + "It will not delete the account or any learner records."
                + "</span>"
        );
    }
}

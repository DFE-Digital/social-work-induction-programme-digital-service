using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Dfe.Sww.Ecf.Frontend.TagHelpers;

public class LinkAccountLinkTagHelper : TagHelper
{
    public string? AccountName { get; set; }

    public string? Href { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "p";
        output.TagMode = TagMode.StartTagAndEndTag;
        output.AddClass("govuk-body", HtmlEncoder.Default);

        output.Content.SetHtmlContent(
            $"<a class=\"govuk-link govuk-link--no-visited-state\" data-test-id=\"link\" href=\"{Href}\">Link {AccountName} to this organisation</a>"
        );
    }
}

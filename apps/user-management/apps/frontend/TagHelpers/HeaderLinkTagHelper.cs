using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Dfe.Sww.Ecf.Frontend.TagHelpers;

[HtmlTargetElement("header-link")]
public class HeaderLinkTagHelper : TagHelper
{
    public required string IsActiveClass { get; set; }

    public required string Href { get; set; }

    public required string Text { get; set; }

    public bool? IsVisible { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (IsVisible is false) return;

        output.TagName = null;
        output.Content.SetHtmlContent(
            $"<li class=\"govuk-service-navigation__item {IsActiveClass}\">"
            + $"<a class=\"govuk-service-navigation__link\" href=\"{Href}\">"
            + Text
            + "</a>"
            + "</li>"
        );
    }
}

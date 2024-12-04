using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Dfe.Sww.Ecf.Frontend.TagHelpers;

public class HeaderLinkTagHelper : TagHelper
{
    public required string IsActiveClass { get; set; }

    public required string Href { get; set; }

    public required string Text { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.Content.SetHtmlContent(
            $"<li class=\"dfe-header__navigation-item {IsActiveClass}\")\">"
                + $"<a class=\"dfe-header__navigation-link\" href=\"{Href}\">"
                + Text
                + "<svg class=\"dfe-icon dfe-icon__chevron-right\" xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" aria-hidden=\"true\" width=\"34\" height=\"34\">"
                + " <path d=\"M15.5 12a1 1 0 0 1-.29.71l-5 5a1 1 0 0 1-1.42-1.42l4.3-4.29-4.3-4.29a1 1 0 0 1 1.42-1.42l5 5a1 1 0 0 1 .29.71z\"></path>"
                + "</svg>"
                + "</a>"
                + "</li>"
        );
    }
}

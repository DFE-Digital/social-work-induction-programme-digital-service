using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Dfe.Sww.Ecf.Frontend.TagHelpers;

[HtmlTargetElement("govuk-cards")]
public class GovUkCardsTagHelper : TagHelper
{
    /// <summary>Optional heading above the cards</summary>
    public string? Heading { get; set; }

    /// <summary>List of cards to render</summary>
    public IEnumerable<CardItem>? Items { get; set; }

    /// <summary>Which heading level to use for each card’s title (default is h3)</summary>
    public int SubHeadingLevel { get; set; } = 3;

    /// <summary>Number of columns (single, 2, or 3); omit or set to 0 for single</summary>
    public int Columns { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (Items == null)
            throw new ArgumentNullException(nameof(Items), "The cards component requires an Items collection.");

        output.TagName = "div";
        output.Attributes.SetAttribute("class", "gem-c-cards");

        var content = new DefaultTagHelperContent();

        if (!string.IsNullOrEmpty(Heading))
        {
            var headingTag = new TagBuilder($"h1");
            headingTag.AddCssClass("gem-c-cards__heading govuk-heading-l");
            if (Columns == 0)
                headingTag.AddCssClass("gem-c-cards__heading--underline");
            headingTag.InnerHtml.Append(Heading);
            content.AppendHtml(headingTag);
        }

        var list = new TagBuilder("ul");
        list.AddCssClass("gem-c-cards__list");
        if (Columns == 0) list.AddCssClass("gem-c-cards__list--one-column");
        else if (Columns == 2) list.AddCssClass("gem-c-cards__list--two-column-desktop");
        else if (Columns == 3) list.AddCssClass("gem-c-cards__list--three-column-desktop");

        foreach (var item in Items)
        {
            if (item.Link == null || string.IsNullOrEmpty(item.Link.Path))
                throw new ArgumentException("Each CardItem must have a link with a non-empty path.");

            var listItem = new TagBuilder("li");
            listItem.AddCssClass("gem-c-cards__list-item");

            var divWrapper = new TagBuilder("div");
            divWrapper.AddCssClass("gem-c-cards__list-item-wrapper");

            var subHeading = new TagBuilder($"h{SubHeadingLevel}");
            subHeading.AddCssClass("gem-c-cards__sub-heading govuk-heading-s");

            var anchor = new TagBuilder("a");
            anchor.AddCssClass("govuk-link gem-c-cards__link gem-c-force-print-link-styles");
            anchor.Attributes["href"] = item.Link.Path;
            if (item.Link.Attributes != null)
            {
                foreach (var keyValuePair in item.Link.Attributes)
                    anchor.Attributes[$"{keyValuePair.Key}"] = keyValuePair.Value;
            }

            if (item.Link.Text != null) anchor.InnerHtml.Append(item.Link.Text);

            subHeading.InnerHtml.AppendHtml(anchor);
            if (!string.IsNullOrEmpty(item.Description))
            {
                var description = new TagBuilder("p");
                description.AddCssClass("govuk-body gem-c-cards__description");
                description.InnerHtml.Append(item.Description);
                subHeading.InnerHtml.AppendHtml(description);
            }

            divWrapper.InnerHtml.AppendHtml(subHeading);
            listItem.InnerHtml.AppendHtml(divWrapper);
            list.InnerHtml.AppendHtml(listItem);
        }

        content.AppendHtml(list);
        output.Content.SetHtmlContent(content);
    }
}

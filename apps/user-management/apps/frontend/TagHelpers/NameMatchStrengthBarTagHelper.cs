using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models.NameMatch;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Dfe.Sww.Ecf.Frontend.TagHelpers;

public class NameMatchStrengthBarTagHelper : TagHelper
{
    public MatchResult? NameMatchResult { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (NameMatchResult == null)
        {
            return;
        }

        const int maxScore = 100;
        const int sections = 5;
        const int scorePerSection = maxScore / sections;

        var scoreInt = (int)NameMatchResult;
        var populatedSections = scoreInt / scorePerSection;
        var emptySections = sections - populatedSections;

        output.Content.AppendHtml("<div class=\"strength-bar-container\">");

        for (var i = 0; i < populatedSections; i++)
        {
            output.Content.AppendHtml($"<div class=\"strength-bar match-{scoreInt}\"></div>");
        }

        for (var j = 0; j < emptySections; j++)
        {
            output.Content.AppendHtml("<div class=\"strength-bar\"></div>");
        }

        output.Content.AppendHtml("</div>");

        output.Content.AppendHtml(
            $"<div class =\"strength-label label-{scoreInt}\">{NameMatchResult.GetDisplayName()} Match</div>"
        );
    }
}

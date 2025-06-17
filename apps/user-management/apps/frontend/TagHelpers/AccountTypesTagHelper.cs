using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Dfe.Sww.Ecf.Frontend.TagHelpers;

public class AccountTypesTagHelper : TagHelper
{
    public IList<AccountType>? Types { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = null;

        if (Types == null || Types.Count == 0) return;

        // Singular account types are displayed without a wrapping tag
        if (Types.Count == 1)
        {
            output.Content.SetContent(Types[0].GetDisplayName());
            return;
        }

        // Multiple account types are displayed a separate `p` tags
        var content = Types.Aggregate(
            "",
            (current, accountType) => current + $"<p>{accountType.GetDisplayName()}</p>"
        );
        output.Content.SetHtmlContent(content);
    }
}

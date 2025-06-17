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

        if (Types == null) return;

        var content = Types.Aggregate(
            "",
            (current, accountType) => current + $"<p>{accountType.GetDisplayName()}</p>"
        );

        output.Content.SetHtmlContent(content);
    }
}

using Dfe.Sww.Ecf.Frontend.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Dfe.Sww.Ecf.Frontend.TagHelpers;

public class AccountTypesTagHelper : TagHelper
{
    public IList<AccountType>? Types { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "";

        if (Types == null)
        {
            return;
        }

        output.Content.SetContent(string.Join(", ", Types.Select(type => type.GetDisplayName())));
    }
}

using SocialWorkInductionProgramme.Frontend.Extensions;
using Microsoft.AspNetCore.Razor.TagHelpers;
using SocialWorkInductionProgramme.Frontend.Models;

namespace SocialWorkInductionProgramme.Frontend.TagHelpers;

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

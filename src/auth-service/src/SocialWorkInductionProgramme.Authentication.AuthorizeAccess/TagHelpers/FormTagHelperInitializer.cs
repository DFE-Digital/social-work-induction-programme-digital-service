using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;

namespace SocialWorkInductionProgramme.Authentication.AuthorizeAccess.TagHelpers;

public class FormTagHelperInitializer : ITagHelperInitializer<FormTagHelper>
{
    public void Initialize(FormTagHelper helper, ViewContext context)
    {
        helper.Antiforgery ??= true;
    }
}

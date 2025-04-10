using SocialWorkInductionProgramme.Authentication.AuthorizeAccess.Infrastructure.Security;
using SocialWorkInductionProgramme.Authentication.UiCommon.FormFlow;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SocialWorkInductionProgramme.Authentication.AuthorizeAccess.Pages;

[Journey(SignInJourneyState.JourneyName)]
public class TestModel : PageModel
{
    [FromQuery(Name = "scheme")]
    public string? AuthenticationScheme { get; set; }

    [FromQuery(Name = "trn_token")]
    public string? TrnToken { get; set; }

    public IActionResult OnGet()
    {
        if (string.IsNullOrEmpty(AuthenticationScheme))
        {
            return BadRequest();
        }

        if (User.Identity?.IsAuthenticated != true)
        {
            return Challenge(
                new AuthenticationProperties()
                {
                    Items =
                    {
                        {
                            MatchToEcfAccountAuthenticationHandler
                                .AuthenticationPropertiesItemKeys
                                .OneLoginAuthenticationScheme,
                            AuthenticationScheme
                        },
                        {
                            MatchToEcfAccountAuthenticationHandler
                                .AuthenticationPropertiesItemKeys
                                .ServiceName,
                            "Test service"
                        },
                        {
                            MatchToEcfAccountAuthenticationHandler
                                .AuthenticationPropertiesItemKeys
                                .ServiceUrl,
                            Request.GetEncodedUrl()
                        },
                        {
                            MatchToEcfAccountAuthenticationHandler
                                .AuthenticationPropertiesItemKeys
                                .LinkingToken,
                            TrnToken
                        },
                    },
                    RedirectUri = Request.GetEncodedUrl(),
                },
                AuthenticationSchemes.MatchToEcfAccount
            );
        }

        return Page();
    }
}

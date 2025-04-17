using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Dfe.Sww.Ecf.Frontend.Configuration;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Repositories.Interfaces;
using Dfe.Sww.Ecf.Frontend.Routing;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Dfe.Sww.Ecf.Frontend.Pages.Debug;

public class Backdoor(
    IWebHostEnvironment environment,
    IOptions<OidcConfiguration> oidcConfiguration,
    IAccountRepository accountRepository,
    EcfLinkGenerator linkGenerator
) : DebugBasePageModel(environment, oidcConfiguration)
{
    public IList<Account> Accounts { get; set; } =
        accountRepository.GetAll().OrderBy(account => account.CreatedAt).ToList();

    [BindProperty]
    [Required(ErrorMessage = "Select who you want to log in as")]
    public Guid? SelectedAccountId { get; set; }

    public PageResult OnGet() => Page();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        if (SelectedAccountId is null || SelectedAccountId == Guid.Empty)
            return NotFound();
        var selectedAccount = accountRepository.GetById(SelectedAccountId.Value);
        if (selectedAccount is null)
            return NotFound();

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(
                new ClaimsIdentity(
                    [
                        new Claim(ClaimTypes.Email, selectedAccount.Email ?? string.Empty),
                        new Claim(ClaimTypes.Name, selectedAccount.FullName)
                    ],
                    CookieAuthenticationDefaults.AuthenticationScheme
                )
            ),
            new AuthenticationProperties()
        );

        return Redirect(linkGenerator.Home());
    }
}

using Dfe.Sww.Ecf.Frontend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Sww.Ecf.Frontend.Controllers;

/// <summary>
/// Controller for user account related functionality
/// </summary>
public class AccountsController : Controller
{
    /// <summary>
    /// Action to get all accounts
    /// </summary>
    /// <returns>A list of user accounts</returns>
    public async Task<IActionResult> Index()
    {
        List<Account> allAccounts = new List<Account>
        {
            new Account
            {
                FullName = "Ellen Lastname",
                Status = "Paused",
                AdditionalStatusInfo = "Taking a break from the PQP programme",
                Type = new List<string> { "Early career social worker" },
                Id = 0
            },
            new Account
            {
                FullName = "Joe Lastname",
                Status = "Active",
                Type = new List<string> { "Coordinator" },
                Id = 1
            },
            new Account
            {
                FullName = "Laura Lastname",
                Status = "Pending registration",
                AdditionalStatusInfo =
                    "You have not provided a Social Work England registration number for this account",
                Type = new List<string> { "Early career social worker" },
                Id = 2
            },
            new Account
            {
                FullName = "Ricardo LongLastname",
                Status = "Active",
                Type = new List<string> { "Assessor", "Coordinator" },
                Id = 3
            },
            new Account
            {
                FullName = "Sheena Lastname",
                Status = "Active",
                Type = new List<string> { "Early career social worker" },
                Id = 4
            },
            new Account
            {
                FullName = "Yavuz Lastname",
                Status = "Active",
                Type = new List<string> { "Early career social worker" },
                Id = 5
            }
        };
        return View(allAccounts);
    }
}

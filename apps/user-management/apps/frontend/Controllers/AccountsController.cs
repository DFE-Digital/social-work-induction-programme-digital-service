using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.DAL;
using Dfe.Sww.Ecf.Frontend.Validation;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Sww.Ecf.Frontend.Controllers;

/// <summary>
/// Controller for user account related functionality
/// </summary>
public class AccountsController : Controller
{
    private readonly IValidator<Account> _validator;

    private readonly AccountsRepository _accountsRepository;

    private static Account? AddedAccount { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AccountsController"/> class
    /// </summary>
    /// <param name="validator"></param>
    /// <param name="accountsRepository"></param>
    public AccountsController(IValidator<Account> validator, AccountsRepository accountsRepository)
    {
        _validator = validator;
        _accountsRepository = accountsRepository;
    }

    /// <summary>
    /// Action to get all accounts
    /// </summary>
    /// <returns>A list of user accounts</returns>
    public IActionResult Index()
    {
        var allAccounts = _accountsRepository.GetAll();
        return View(allAccounts);
    }

    /// <summary>
    /// Action to select what type of user account to add
    /// </summary>
    /// <returns>A list of account types</returns>
    [HttpGet]
    public IActionResult SelectUserType()
    {
        ViewData["Referer"] = Request.Headers.Referer;
        return View();
    }

    /// <summary>
    /// Action to store selected user account type
    /// </summary>
    /// <returns>A form for adding user details</returns>
    [HttpPost]
    public IActionResult SelectUserType(AccountType accountType)
    {
        // TODO Add/store session data here
        AddedAccount = new Account
        {
            Id = _accountsRepository.Count(),
            Status = AccountStatus.Active,
            Types = [accountType]
        };

        return RedirectToAction(nameof(AddUserDetails));
    }

    /// <summary>
    /// Action for adding user details
    /// </summary>
    /// <returns>A form for adding user details</returns>
    [HttpGet]
    public IActionResult AddUserDetails()
    {
        ViewData["Referer"] = Request.Headers["Referer"];
        return View();
    }

    /// <summary>
    /// Action for adding user details
    /// </summary>
    /// <returns>A form for adding user details</returns>
    [HttpPost]
    public async Task<IActionResult> AddUserDetails(Account account)
    {
        var result = await _validator.ValidateAsync(account);
        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
            return View(nameof(AddUserDetails), account);
        }

        if (AddedAccount is null)
        {
            throw new NullReferenceException();
        }

        AddedAccount.FirstName = account.FirstName;
        AddedAccount.LastName = account.LastName;
        AddedAccount.Email = account.Email;

        return RedirectToAction(nameof(ConfirmUserDetails));
    }

    /// <summary>
    /// Action for confirming user details
    /// </summary>
    /// <returns>A confirmation screen displaying user details</returns>
    [HttpGet]
    public IActionResult ConfirmUserDetails()
    {
        ViewData["Referer"] = Request.Headers["Referer"];

        return View(AddedAccount);
    }

    /// <summary>
    /// Action for confirming user details
    /// </summary>
    /// <returns>A confirmation screen displaying user details</returns>
    [HttpPost]
    [ActionName("ConfirmUserDetails")]
    public IActionResult ConfirmUserDetails_Post()
    {
        if (AddedAccount is null)
        {
            throw new NullReferenceException();
        }

        // TODO Store in DB user account record

        // TODO replace with chosen state management tool
        TempData["AccountAddedMessage"] = AddedAccount.Email;
        _accountsRepository.Add(AddedAccount);

        return RedirectToAction(nameof(Index));
    }
}

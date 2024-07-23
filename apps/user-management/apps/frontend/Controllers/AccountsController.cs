using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Repositories.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Views.Accounts;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Sww.Ecf.Frontend.Controllers;

/// <summary>
/// Controller for user account related functionality
/// </summary>
public class AccountsController(IValidator<AddAccountDetailsModel> validator, IAccountRepository accountRepository, ICreateAccountJourneyService createAccountJourneyService) : Controller
{
    private readonly IValidator<AddAccountDetailsModel> _validator = validator;
    private readonly IAccountRepository _accountRepository = accountRepository;
    private readonly ICreateAccountJourneyService _createAccountJourneyService = createAccountJourneyService;

    /// <summary>
    /// Action to get all accounts
    /// </summary>
    /// <returns>A list of user accounts</returns>
    public IActionResult Index()
    {
        var response = _accountRepository.GetAll();

        return View(response);
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
    public IActionResult SelectUserType(SelectUserTypeModel selectUserTypeModel)
    {
        if (selectUserTypeModel?.AccountType is null)
        {
            ViewData["Referer"] = Request.Headers.Referer;
            return View();
        }

        if (selectUserTypeModel.AccountType == AccountType.AssessorCoordinator)
        {
            return RedirectToAction(nameof(SelectUseCase));
        }

        var accountTypes = new List<AccountType> { selectUserTypeModel.AccountType.Value };
        _createAccountJourneyService.SetAccountTypes(accountTypes);

        return RedirectToAction(nameof(AddUserDetails));
    }

    /// <summary>
    /// Action to select what type of user account to add
    /// </summary>
    /// <returns>A list of account types</returns>
    [HttpGet]
    public IActionResult SelectUseCase()
    {
        ViewData["Referer"] = Request.Headers.Referer;
        return View();
    }

    /// <summary>
    /// Action to store selected user account type
    /// </summary>
    /// <returns>A form for adding user details</returns>
    [HttpPost]
    public IActionResult SelectUseCase(SelectUseCaseModel selectUseCaseModel)
    {
        if (selectUseCaseModel?.AccountTypes is null)
        {
            ViewData["Referer"] = Request.Headers.Referer;
            return View();
        }

        _createAccountJourneyService.SetAccountTypes(selectUseCaseModel.AccountTypes);

        return RedirectToAction(nameof(AddUserDetails));
    }

    /// <summary>
    /// Action for adding user details
    /// </summary>
    /// <returns>A form for adding user details</returns>
    [HttpGet]
    public IActionResult AddUserDetails()
    {
        ViewData["Referer"] = Request.Headers.Referer;

        var userDetails = _createAccountJourneyService.GetAccountDetails();
        return View(userDetails);
    }

    /// <summary>
    /// Action for adding user details
    /// </summary>
    /// <returns>A form for adding user details</returns>
    [HttpPost]
    public async Task<IActionResult> AddUserDetails(AddAccountDetailsModel userDetails)
    {
        var result = await _validator.ValidateAsync(userDetails);
        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
            return View(nameof(AddUserDetails), userDetails);
        }

        _createAccountJourneyService.SetAccountDetails(userDetails);

        return RedirectToAction(nameof(ConfirmUserDetails));
    }

    /// <summary>
    /// Action for confirming user details
    /// </summary>
    /// <returns>A confirmation screen displaying user details</returns>
    [HttpGet]
    public IActionResult ConfirmUserDetails()
    {
        ViewData["Referer"] = Request.Headers.Referer;

        var userDetailsModel = _createAccountJourneyService.GetAccountDetails();

        return View(userDetailsModel);
    }

    /// <summary>
    /// Action for confirming user details
    /// </summary>
    /// <returns>A confirmation screen displaying user details</returns>
    [HttpPost]
    [ActionName("ConfirmUserDetails")]
    public IActionResult ConfirmUserDetails_Post()
    {
        var userDetails = _createAccountJourneyService.GetAccountDetails();
        if (userDetails is null)
        {
            throw new NullReferenceException();
        }

        _createAccountJourneyService.CompleteJourney();

        TempData["CreatedAccountEmail"] = userDetails.Email;

        return RedirectToAction(nameof(Index));
    }
}

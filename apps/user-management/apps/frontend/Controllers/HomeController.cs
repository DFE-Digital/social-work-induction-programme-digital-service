using System.Diagnostics;
using Dfe.Sww.Ecf.Frontend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Sww.Ecf.Frontend.Controllers;

/// <summary>
/// Home controller
/// </summary>
public class HomeController : Controller
{
    /// <summary>
    /// Home page action
    /// </summary>
    /// <returns>Index view</returns>
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// Privacy action
    /// </summary>
    /// <returns>Privacy view</returns>
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(
            new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }
        );
    }
}

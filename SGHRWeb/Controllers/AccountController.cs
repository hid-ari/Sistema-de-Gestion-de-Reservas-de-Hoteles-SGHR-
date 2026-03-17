using Microsoft.AspNetCore.Mvc;
using SGHRWeb.Models;

namespace SGHRWeb.Controllers;

public class AccountController : Controller
{
    private static readonly List<UserRegistration> Registrations = new();

    [HttpGet]
    public IActionResult Register()
    {
        return View(new UserRegistration());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Register(UserRegistration model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        Registrations.Add(model);
        ViewData["Saved"] = true;
        ModelState.Clear();
        return View(new UserRegistration());
    }
}

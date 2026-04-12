using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SGHRWeb.Auth;
using SGHRWeb.Models;
using System.Security.Claims;

namespace SGHRWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserStore _userStore;

        public AccountController(UserStore userStore)
        {
            _userStore = userStore;
        }

        // ── LOGIN ────────────────────────────────────────────────────────────

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            var user = _userStore.ValidateCredentials(model.Email, model.Password);
            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Correo electrónico o contraseña incorrectos.");
                return View(model);
            }

            // Construir claims del usuario autenticado
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name,            $"{user.Nombre} {user.Apellido}"),
                new Claim(ClaimTypes.Email,            user.Email),
                new Claim(ClaimTypes.Role,             user.Role),
                new Claim("Telefono",                  user.Telefono)
            };

            var identity   = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal  = new ClaimsPrincipal(identity);
            var properties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc   = model.RememberMe
                    ? DateTimeOffset.UtcNow.AddDays(30)
                    : DateTimeOffset.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal, properties);

            TempData["Success"] = $"Bienvenido/a, {user.Nombre}!";

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        // ── REGISTRO ─────────────────────────────────────────────────────────

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            return View(new RegisterViewModel());
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // SRS Val.3 – Verificar que el correo no esté ya registrado
            if (_userStore.EmailExists(model.Email))
            {
                ModelState.AddModelError("Email", "Este correo electrónico ya está registrado.");
                return View(model);
            }

            var user = _userStore.Register(
                model.Nombre, model.Apellido,
                model.Email, model.Telefono,
                model.Password);

            // Auto-login tras registro
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name,            $"{user.Nombre} {user.Apellido}"),
                new Claim(ClaimTypes.Email,            user.Email),
                new Claim(ClaimTypes.Role,             user.Role),
                new Claim("Telefono",                  user.Telefono)
            };

            var identity  = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties { IsPersistent = false });

            TempData["Success"] = $"Cuenta creada exitosamente. Bienvenido/a, {user.Nombre}!";
            return RedirectToAction("Index", "Home");
        }

        // ── LOGOUT ───────────────────────────────────────────────────────────

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Success"] = "Sesión cerrada correctamente.";
            return RedirectToAction("Login");
        }

        // GET: /Account/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied() => View();
    }
}

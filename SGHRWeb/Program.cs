using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SGHR.Data.Abstraction;
using SGHR.Data.Context;
using SGHR.Data.Services;
using SGHRWeb.Auth;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// ── Base de datos en memoria ────────────────────────────────────────────────
builder.Services.AddDbContext<SGHRContext>(options =>
    options.UseInMemoryDatabase("SGHR_Web"));

// ── Servicios de dominio ────────────────────────────────────────────────────
builder.Services.AddScoped<ICategoriaHabitacionService, CategoriaHabitacionService>();
builder.Services.AddScoped<IReservaService, ReservaService>();

// ── Autenticación por Cookie ────────────────────────────────────────────────
builder.Services.AddSingleton<UserStore>(); // singleton: almacén en memoria compartido

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath        = "/Account/Login";
        options.LogoutPath       = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.Name      = "SGHR.Auth";
        options.Cookie.HttpOnly  = true;
        options.Cookie.SameSite  = SameSiteMode.Strict;
        options.ExpireTimeSpan   = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

// ── Build ───────────────────────────────────────────────────────────────────
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication(); // ← Primero autenticación
app.UseAuthorization();  // ← Luego autorización

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();

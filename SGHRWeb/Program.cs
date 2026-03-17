<<<<<<< HEAD
=======
using SGHRWeb.Auth;

>>>>>>> origin/Dev
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
<<<<<<< HEAD
=======
builder.Services.AddAuthentication("Placeholder")
    .AddScheme<AuthenticationSchemeOptions, PlaceholderAuthenticationHandler>(
        "Placeholder",
        _ => { });
builder.Services.AddAuthorization();
>>>>>>> origin/Dev

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

<<<<<<< HEAD
app.UseAuthorization();



  
=======
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

>>>>>>> origin/Dev

app.Run();

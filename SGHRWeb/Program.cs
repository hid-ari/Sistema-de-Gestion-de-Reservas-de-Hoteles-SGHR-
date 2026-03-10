using Microsoft.EntityFrameworkCore;
using SGHR.Data.Context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configurar DbContext con SQLite
builder.Services.AddDbContext<SGHRDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Asegurar que la base de datos esté creada
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SGHRDbContext>();
    context.Database.EnsureCreated(); // Para desarrollo
    // O context.Database.Migrate(); // Para producción con migraciones
}

app.Run();

# En la terminal/consola de paquetes
dotnet ef migrations add InitialCreate --project SGHR.Data --startup-project SGHRWeb
dotnet ef database update --project SGHR.Data --startup-project SGHRWeb

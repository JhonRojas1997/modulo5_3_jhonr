using Microsoft.EntityFrameworkCore;

using Modulo5_3_JhonR.Data;

var builder = WebApplication.CreateBuilder(args);

// Obtener la cadena de conexión desde el archivo appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Agregar los servicios de EF Core con PostgreSQL
builder.Services.AddDbContext<PostgresDbContext>(options =>
    options.UseNpgsql(connectionString));


// Agregar los servicios MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
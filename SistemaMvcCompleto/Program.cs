using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using SistemaCompleto.Data;
using SistemaCompleto.Filters;
using SistemaCompleto.Models;
using SistemaCompleto.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SistamaProdutocontext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter(policyName: "LoginLimit", opt =>
    {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromMinutes(60);
        opt.QueueLimit = 0;
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequiredLength = 4;
})
    .AddEntityFrameworkStores<SistamaProdutocontext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/Login";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.SlidingExpiration = true;

    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Apenas HTTPS
});

builder.Services.AddScoped<IProdutoServicecs, ProdutoService>();
builder.Services.AddScoped<IExcelService, ExcelService>();
builder.Services.AddScoped<PrimeiroAcessoFilter>();

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.AddService<PrimeiroAcessoFilter>();
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await DbInitializer.InitializeAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro ao popular/inicializar o banco de dados.");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseRateLimiter(); 

app.UseAuthentication(); 
app.UseAuthorization();  

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using SistemaCompleto.Data;
using SistemaCompleto.Filters;
using SistemaCompleto.Models;
using SistemaCompleto.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar DbContext
builder.Services.AddDbContext<SistamaProdutocontext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//2.Configurar Rate Limiter(Unificado)
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

// 3. Configurar Identity
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

// 🍪 Configuração dos Cookies
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

// 4. Registrar Injeções de Dependência
builder.Services.AddScoped<IProdutoServicecs, ProdutoService>();
builder.Services.AddScoped<IExcelService, ExcelService>();
builder.Services.AddScoped<PrimeiroAcessoFilter>();

// 🎯 Registrar Controllers e Views 
// Nota: Se o PrimeiroAcessoFilter continuar dando 404, remova-o temporariamente daqui para testar
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.AddService<PrimeiroAcessoFilter>();
});

var app = builder.Build();

// 5. Executar DbInitializer (Roda ANTES do app aceitar requisições)
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

// 6. Configurar Pipeline de Middlewares HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseRateLimiter(); // 👈 Ativa o RateLimiter configurado

app.UseAuthentication(); // 👈 1º Identifica o usuário
app.UseAuthorization();  // 👈 2º Checa as permissões do usuário

// 7. Mapeamento de Rotas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
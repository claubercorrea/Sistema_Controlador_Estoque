using CadastroProdutos.Data;

using CadastroProdutos.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CadastroProdutos.Models; // <-- Ele está aqui!
// CONFIGURAÇÃO DA LICENÇA DO PDF:
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
var builder = WebApplication.CreateBuilder(args);

// 1. Configuração do Banco de Dados
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Configuração do Identity (Nativo para Cookies e Roles)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// 3. Configuração do Cookie de Login
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Auth/Login";
    options.AccessDeniedPath = "/Auth/Login";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(2);
});

// 4. Injeção de Dependência
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEstoqueService, EstoqueService>();
builder.Services.AddScoped<IAdminService, AdminService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// --- INICIALIZAÇÃO DE ROLES E ADMIN ---
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    //var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // Cria Roles
    string[] roles = { "Admin", "Funcionario" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Cria Admin padrão
    var emailAdmin = "admin@sistema.com";
    var user = await userManager.FindByEmailAsync(emailAdmin);

    if (user == null)
    {
        var admin = new ApplicationUser { UserName = emailAdmin, Email = emailAdmin, PrimeiroAcesso = false, EmailConfirmed = true };
        var result = await userManager.CreateAsync(admin, "Senha123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
    else
    {
        // GARANTIA: Se o usuário já existir, certifica-se de que ele tem a permissão de Admin
        if (!await userManager.IsInRoleAsync(user, "Admin"))
        {
            await userManager.AddToRoleAsync(user, "Admin");
        }
    
}
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();
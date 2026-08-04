


using Microsoft.AspNetCore.Identity;
using SistemaCompleto.Models;

namespace SistemaCompleto.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            string[] roleNames = { "Admin", "Usuario" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var adminEmail = "admin@sistema.com"; 
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    NomeCompleto = "Administrador Master",
                    EmailConfirmed = true,
                    PrimeiroAcesso = true 
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@1234");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}
using Microsoft.AspNetCore.Identity;

namespace BibliotecaMVC.Data
{
    public static class GestionRoles
    {
        public static async Task CargarAdmin(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            if (!await roleManager.RoleExistsAsync("Administrador"))
            {
                await roleManager.CreateAsync(new IdentityRole("Administrador"));
            }

            var usuario = await userManager.FindByNameAsync("xiomara");
            if (usuario != null && !await userManager.IsInRoleAsync(usuario, "Administrador"))
            {
                await userManager.AddToRoleAsync(usuario, "Administrador");
            }
        }
    }
}
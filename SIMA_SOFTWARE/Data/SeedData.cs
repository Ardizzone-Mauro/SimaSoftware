using Microsoft.AspNetCore.Identity;

namespace SIMA_SOFTWARE.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Listado de roles SIMA Software
            string[] ListadoRoles = {
            "Cliente",
            "Vendedor Online",
            "Administrador de Stock",
            "Gerente de Ventas Online",
            "Administrador del Sistema"
        };

            foreach (var rol in ListadoRoles)
            {
                var roleExist = await roleManager.RoleExistsAsync(rol);
                if (!roleExist)
                {
                    // Crea los roles en la tabla AspNetRoles de SQL Server
                    await roleManager.CreateAsync(new IdentityRole(rol));
                }
            }
        }

    }
}

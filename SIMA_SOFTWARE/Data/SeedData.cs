using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SIMA_SOFTWARE.Models;

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
                bool roleExist = await roleManager.RoleExistsAsync(rol);
                if (!roleExist)
                {
                    // Crea los roles en la tabla AspNetRoles de SQL Server
                    await roleManager.CreateAsync(new IdentityRole(rol));
                }
            }

            // 2. Gestión de Productos (Lógica de Negocio)
            using (var context = new SimaDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<SimaDbContext>>()))
            {
                // Solo cargamos si la tabla de productos está vacía para evitar duplicados
                if (context.Productos.Any())
                {
                    return;
                }

                context.Productos.AddRange(
                    new Producto { Nombre = "Llama Peinada - Marrón", Precio = 2800.00m, UrlImagen = "/images/llama-marron.jpg" },
                    new Producto { Nombre = "Merino Rosa", Precio = 1990.00m, UrlImagen = "/images/merino-rosa.jpg" },
                    new Producto { Nombre = "Oveja - Gris", Precio = 3000.00m, UrlImagen = "/images/oveja-gris.jpg" },
                    new Producto { Nombre = "Llama - Blanco", Precio = 5000.00m, UrlImagen = "/images/llama-blanco.jpg" },
                    new Producto { Nombre = "Conejo - Marrón Oscuro", Precio = 4350.00m, UrlImagen = "/images/conejo-oscuro.jpg" },
                    new Producto { Nombre = "Llama / Merino - Blanco", Precio = 3510.00m, UrlImagen = "/images/llama-merino.jpg" },
                    new Producto { Nombre = "Llama dos cabos - Beige", Precio = 2795.00m, UrlImagen = "/images/llama-2-cabos.jpg" },
                    new Producto { Nombre = "Llama Beige", Precio = 1500.00m, UrlImagen = "/images/llama-beige.jpg" },
                    new Producto { Nombre = "Llama Crudo", Precio = 1500.00m, UrlImagen = "/images/llama-crudo.jpg" },
                    new Producto { Nombre = "Llama Gris", Precio = 2900.00m, UrlImagen = "/images/llama-gris.jpg" },
                    new Producto { Nombre = "Merino - Azul", Precio = 3150.00m, UrlImagen = "/images/merino-azul.jpg" },
                    new Producto { Nombre = "Merino Crudo", Precio = 2275.00m, UrlImagen = "/images/merino-crudo.jpg" },
                    new Producto { Nombre = "Llama Rustica - Beige", Precio = 4200.00m, UrlImagen = "/images/llama-rustica.jpg" },
                    new Producto { Nombre = "Llama Peinada - Natural", Precio = 2600.00m, UrlImagen = "/images/llama-natural.jpg" },
                    new Producto { Nombre = "Oveja - Negro", Precio = 3000.00m, UrlImagen = "/images/oveja-negro.jpg" }
                );

                await context.SaveChangesAsync();
            }
        }
    }
}
        

    


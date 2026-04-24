using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SIMA_SOFTWARE.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

            // 2. Gestión de Productos + Inventario + Stock
            using (var context = new SimaDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<SimaDbContext>>()))
            {
                if (context.Productos.Any())
                {
                    return;
                }

                // 🟣 PASO 1: CREAR DEPÓSITOS
                var depositoCentral = new Deposito { Nombre = "Depósito Central" };
                var depositoLocal = new Deposito { Nombre = "Local" };

                context.Depositos.AddRange(depositoCentral, depositoLocal);
                await context.SaveChangesAsync();

                // 🟣 PASO 2: CREAR PRODUCTOS
                var productos = new List<Producto>
    {
        new Producto { Nombre = "Oveja - Algodón Arena", Precio = 3000m, UrlImagen = "/images/Productos/oveja-algodon-arena.jpg", Descripcion = "Algodón suave color arena." },
        new Producto { Nombre = "Oveja - Algodón Coral", Precio = 3000m, UrlImagen = "/images/Productos/oveja-algodon-coral.jpg", Descripcion = "Algodón color coral." },
        new Producto { Nombre = "Oveja - Algodón Mostaza", Precio = 3000m, UrlImagen = "/images/Productos/oveja-algodon-mostaza.jpg", Descripcion = "Tono mostaza vibrante." },
        new Producto { Nombre = "Oveja - Algodón Turquesa", Precio = 3000m, UrlImagen = "/images/Productos/oveja-algodon-turquesa.jpg", Descripcion = "Color turquesa intenso." },
        new Producto { Nombre = "Oveja - Algodón Verde", Precio = 3000m, UrlImagen = "/images/Productos/oveja-algodon-verde.jpg", Descripcion = "Algodón verde natural." },
        new Producto { Nombre = "Oveja - Gris Oscuro", Precio = 3000m, UrlImagen = "/images/Productos/oveja-gris-oscuro.jpeg", Descripcion = "Lana gris oscuro." },
        new Producto { Nombre = "Llama Peinada", Precio = 2800m, UrlImagen = "/images/Productos/llama-peinada.jpg", Descripcion = "Fibra de llama peinada." },
        new Producto { Nombre = "Llama Crudo", Precio = 2800m, UrlImagen = "/images/Productos/llama-crudo.jpeg", Descripcion = "Fibra natural sin teñir." },
        new Producto { Nombre = "Llama Merino Rosa", Precio = 1990m, UrlImagen = "/images/Productos/merino-rosa.jpeg", Descripcion = "Mezcla con merino rosa." },
        new Producto { Nombre = "Llama Merino Marrón", Precio = 3510m, UrlImagen = "/images/Productos/merino-llama-marron.jpeg", Descripcion = "Mezcla premium marrón." },
        new Producto { Nombre = "Llama Merino Caramelo", Precio = 3510m, UrlImagen = "/images/Productos/merino-llama-caramelo.jpeg", Descripcion = "Color caramelo cálido." },
        new Producto { Nombre = "Llama Merino Beige", Precio = 3510m, UrlImagen = "/images/Productos/merino-llama-beige.jpeg", Descripcion = "Tono neutro elegante." },
        new Producto { Nombre = "Llama Top Crudo", Precio = 1500m, UrlImagen = "/images/Productos/llama-top-crudo.jpeg", Descripcion = "Fibra lista para hilado." },
        new Producto { Nombre = "Llama Rústica Marrón", Precio = 4200m, UrlImagen = "/images/Productos/llama-rustico-marron.jpeg", Descripcion = "Fibra rústica resistente." },
        new Producto { Nombre = "Cabra Mohair", Precio = 1500m, UrlImagen = "/images/Productos/mohair.jpeg", Descripcion = "Mohair liviano." },
        new Producto { Nombre = "Cardada 3-7 Verde", Precio = 3100m, UrlImagen = "/images/Productos/cardada-3-7-verde.png", Descripcion = "Fibra cardada verde." },
        new Producto { Nombre = "Cardada 3-7 Negro", Precio = 3400m, UrlImagen = "/images/Productos/cardada-3-7-negro.png", Descripcion = "Fibra cardada negra." },
        new Producto { Nombre = "Conejo Blanco", Precio = 4350m, UrlImagen = "/images/Productos/conejo-blanco.jpeg", Descripcion = "Fibra tipo angora." },
        new Producto { Nombre = "Algodón", Precio = 3400m, UrlImagen = "/images/Productos/algodon.jpeg", Descripcion = "Algodón natural versátil." }
    };

                context.Productos.AddRange(productos);
                await context.SaveChangesAsync();

                // 🟣 PASO 3: INVENTARIO + STOCK
                var random = new Random();

                foreach (var producto in productos)
                {
                    var stockInicial = random.Next(5, 20);



                    var inventario = new Inventario
                    {
                        IdProducto = producto.IdProducto,
                        IdDeposito = depositoCentral.IdDeposito,
                        Stock = stockInicial
                    };

                    context.Inventarios.Add(inventario);
                    await context.SaveChangesAsync();

                    var movimiento = new Stock
                    {
                        IdInventario = inventario.IdInventario,
                        Cantidad = stockInicial,
                        PrecioUnitario = producto.Precio,
                        FechaActualizacion = DateTime.Now
                    };

                    context.Stocks.Add(movimiento);
                }

                await context.SaveChangesAsync();
            }
        }
    }
}






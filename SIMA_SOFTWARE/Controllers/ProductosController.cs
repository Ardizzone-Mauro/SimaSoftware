using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIMA_SOFTWARE.Data;
using SIMA_SOFTWARE.Models;
using SIMA_SOFTWARE.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIMA_SOFTWARE.Controllers
{
    public class ProductosController : Controller
    {
        private readonly SimaDbContext _context;

        public ProductosController(SimaDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            var productos = await _context.Productos
                .Include(p => p.Inventarios) // 👈 CLAVE
                .Select(p => new ProductoViewModel
                {
                    IdProducto = p.IdProducto,
                    Nombre = p.Nombre,
                    Precio = p.Precio,
                    UrlImagenExistente = p.UrlImagen, // 👈 importante
                    Descripcion = p.Descripcion,

                    //  CALCULAR STOCK TOTAL ACÁ
                    StockTotal = p.Inventarios.Sum(i => i.Stock)
                })
                .ToListAsync();

            return View(productos);
        }

        // GET: Productos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var producto = await _context.Productos
                .Include(p => p.Inventarios)
                .FirstOrDefaultAsync(p => p.IdProducto == id);

            if (producto == null) return NotFound();

            var inventario = producto.Inventarios?.FirstOrDefault();

            var model = new ProductoViewModel
            {
                IdProducto = producto.IdProducto,
                Nombre = producto.Nombre,
                Precio = producto.Precio,
                Descripcion = producto.Descripcion,
                UrlImagenExistente = producto.UrlImagen,
                StockInicial = inventario?.Stock ?? 0
            };

            return View(model);
        }

        // GET: Productos/Create
        public IActionResult Create()
        {
            ViewBag.Depositos = new SelectList(_context.Depositos, "IdDeposito", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errores = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var e in errores)
                {
                    Console.WriteLine(e.ErrorMessage);
                }

                ViewBag.Depositos = new SelectList(_context.Depositos, "IdDeposito", "Nombre");
                return View(model);
            }

            string? rutaImagen = null;

            if (model.Imagen != null)
            {
                var carpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/productos");

                //  aseguramos que exista
                if (!Directory.Exists(carpeta))
                    Directory.CreateDirectory(carpeta);

                var nombreArchivo = Guid.NewGuid() + Path.GetExtension(model.Imagen.FileName);
                var rutaCompleta = Path.Combine(carpeta, nombreArchivo);

                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    await model.Imagen.CopyToAsync(stream);
                }

                rutaImagen = "/images/productos/" + nombreArchivo;
            }

            // 🟣 1. Crear producto
            var producto = new Producto
            {
                Nombre = model.Nombre,
                Precio = model.Precio,
                Descripcion = model.Descripcion,
                UrlImagen = rutaImagen
            };

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync(); // 👉 NECESARIO para obtener ID

            // 🟣 2. Crear inventario
            var inventario = new Inventario
            {
                IdProducto = producto.IdProducto,
                IdDeposito = model.IdDeposito,
                Stock = model.StockInicial
            };

            _context.Inventarios.Add(inventario);
            await _context.SaveChangesAsync();

            // 🟣 3. Crear movimiento de stock
            var stock = new Stock
            {
                IdInventario = inventario.IdInventario,
                Cantidad = model.StockInicial,
                PrecioUnitario = model.Precio,
                FechaActualizacion = DateTime.Now
            };

            _context.Stocks.Add(stock);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }




        // GET: Productos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var producto = await _context.Productos
                .Include(p => p.Inventarios)
                .FirstOrDefaultAsync(p => p.IdProducto == id);

            if (producto == null) return NotFound();

            var inventario = producto.Inventarios?.FirstOrDefault();

            var model = new ProductoViewModel
            {
                IdProducto = producto.IdProducto,
                Nombre = producto.Nombre,
                Precio = producto.Precio,
                Descripcion = producto.Descripcion,
                UrlImagenExistente = producto.UrlImagen,

                IdDeposito = inventario?.IdDeposito ?? 0,
                StockInicial = inventario?.Stock ?? 0
            };

            ViewBag.Depositos = new SelectList(_context.Depositos, "IdDeposito", "Nombre", model.IdDeposito);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductoViewModel model)
        {
            if (id != model.IdProducto)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound();
            }

            // 🔥 Actualizar datos básicos
            producto.Nombre = model.Nombre;
            producto.Precio = model.Precio;
            producto.Descripcion = model.Descripcion;

            // 🔥 Manejo de imagen (si se sube una nueva)
            if (model.Imagen != null)
            {
                var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(model.Imagen.FileName);

                var ruta = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/images/productos",
                    nombreArchivo
                );

                using (var stream = new FileStream(ruta, FileMode.Create))
                {
                    await model.Imagen.CopyToAsync(stream);
                }

                producto.UrlImagen = "/images/productos/" + nombreArchivo;
            }

            _context.Update(producto);

            var inventario = await _context.Inventarios
    .FirstOrDefaultAsync(i => i.IdProducto == id && i.IdDeposito == model.IdDeposito);

            if (inventario != null)
            {
                // Actualizar stock existente
                inventario.Stock = model.StockInicial;
            }
            else
            {
                // Crear nuevo inventario si no existe para ese depósito
                var nuevoInventario = new Inventario
                {
                    IdProducto = id,
                    IdDeposito = model.IdDeposito,
                    Stock = model.StockInicial
                };

                _context.Inventarios.Add(nuevoInventario);
            }
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Productos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .FirstOrDefaultAsync(m => m.IdProducto == id);

            if (producto == null)
            {
                return NotFound();
            }

            var model = new ProductoViewModel
            {
                IdProducto = producto.IdProducto,
                Nombre = producto.Nombre ?? "",
                Precio = producto.Precio,
                UrlImagenExistente = producto.UrlImagen,
                Descripcion = producto.Descripcion
            };

            return View(model);
        }

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound();
            }

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.IdProducto == id);
        }
    }
}

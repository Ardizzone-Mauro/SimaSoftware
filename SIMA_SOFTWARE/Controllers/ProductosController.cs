using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIMA_SOFTWARE.Data;
using SIMA_SOFTWARE.Models;
using SIMA_SOFTWARE.Models.ViewModels;

namespace SIMA_SOFTWARE.Controllers
{
    public class ProductosController : Controller
    {
        private readonly SimaDbContext _context;

        public ProductosController(SimaDbContext context)
        {
            _context = context;
        }

        // =========================
        // INDEX
        // =========================
        public async Task<IActionResult> Index()
        {
            var productos = await _context.Productos
                .Where(p => !p.Eliminado)
                .Include(p => p.Inventarios)
                .Select(p => new ProductoViewModel
                {
                    IdProducto = p.IdProducto,
                    Nombre = p.Nombre,
                    Precio = p.Precio,
                    UrlImagenExistente = p.UrlImagen,
                    Descripcion = p.Descripcion,

                    Categoria = p.Categoria,
                    Color = p.Color,
                    CantidadHebras = p.CantidadHebras,

                    StockTotal = p.Inventarios.Sum(i => i.Stock)
                })
                .ToListAsync();

            return View(productos);
        }

        // =========================
        // DETAILS
        // =========================
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
                StockInicial = inventario?.Stock ?? 0,

                Categoria = producto.Categoria,
                Color = producto.Color,
                CantidadHebras = producto.CantidadHebras
            };

            return View(model);
        }

        // =========================
        // CREATE GET
        // =========================
        public IActionResult Create()
        {
            ViewBag.Depositos = new SelectList(_context.Depositos, "IdDeposito", "Nombre");
            return View();
        }

        // =========================
        // CREATE POST
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Depositos = new SelectList(_context.Depositos, "IdDeposito", "Nombre");
                return View(model);
            }

            string? rutaImagen = null;

            if (model.Imagen != null)
            {
                var carpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/productos");

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

            var producto = new Producto
            {
                Nombre = model.Nombre,
                Precio = model.Precio,
                Descripcion = model.Descripcion,
                UrlImagen = rutaImagen,

                Categoria = model.Categoria,
                Color = model.Color,
                CantidadHebras = model.CantidadHebras
            };

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            var inventario = new Inventario
            {
                IdProducto = producto.IdProducto,
                IdDeposito = model.IdDeposito,
                Stock = model.StockInicial
            };

            _context.Inventarios.Add(inventario);
            await _context.SaveChangesAsync();

            var stock = new Stock
            {
                IdInventario = inventario.IdInventario,
                Cantidad = model.StockInicial,
                PrecioUnitario = model.Precio,
                FechaActualizacion = DateTime.Now
            };

            _context.Stocks.Add(stock);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // EDIT GET
        // =========================
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
                StockInicial = inventario?.Stock ?? 0,

                Categoria = producto.Categoria,
                Color = producto.Color,
                CantidadHebras = producto.CantidadHebras
            };

            ViewBag.Depositos = new SelectList(_context.Depositos, "IdDeposito", "Nombre", model.IdDeposito);

            return View(model);
        }

        // =========================
        // EDIT POST
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductoViewModel model)
        {
            if (id != model.IdProducto) return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            var producto = await _context.Productos.FindAsync(id);

            if (producto == null) return NotFound();

            producto.Nombre = model.Nombre;
            producto.Precio = model.Precio;
            producto.Descripcion = model.Descripcion;

            producto.Categoria = model.Categoria;
            producto.Color = model.Color;
            producto.CantidadHebras = model.CantidadHebras;

            if (model.Imagen != null)
            {
                var nombreArchivo = Guid.NewGuid() + Path.GetExtension(model.Imagen.FileName);

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
                inventario.Stock = model.StockInicial;
            }
            else
            {
                _context.Inventarios.Add(new Inventario
                {
                    IdProducto = id,
                    IdDeposito = model.IdDeposito,
                    Stock = model.StockInicial
                });
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DELETE (soft delete)
        // =========================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _context.Productos
                .FirstOrDefaultAsync(p => p.IdProducto == id);

            if (producto == null) return NotFound();

            producto.Eliminado = true;
            producto.FechaEliminacion = DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // ELIMINADOS
        // =========================
        public async Task<IActionResult> Eliminados()
        {
            var productosEliminados = await _context.Productos
                .IgnoreQueryFilters()
                .Where(p => p.Eliminado)
                .ToListAsync();

            return View(productosEliminados);
        }

        // =========================
        // RESTAURAR
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restaurar(int id)
        {
            var producto = await _context.Productos
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.IdProducto == id);

            if (producto == null) return NotFound();

            producto.Eliminado = false;
            producto.FechaEliminacion = null;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Eliminados));
        }
    }
}
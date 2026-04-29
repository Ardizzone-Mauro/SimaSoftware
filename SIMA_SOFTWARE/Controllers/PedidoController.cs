using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIMA_SOFTWARE.Data;
using SIMA_SOFTWARE.Models;
using SIMA_SOFTWARE.Models.Requests;
using SIMA_SOFTWARE.Models.ViewModels;
using System.Linq;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Font;

namespace SIMA_SOFTWARE.Controllers
{
    public class PedidoController : Controller
    {
        private readonly SimaDbContext _context;

        public PedidoController(SimaDbContext context)
        {
            _context = context;
        }


        // ===============================
        // 🔹 LISTADO DE PEDIDOS (HU03)
        // ===============================
        public IActionResult Index(string estado, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var query = _context.Pedidos
                .Where(p => p.Activo)
                .AsQueryable();

            // 🔹 FILTRO POR ESTADO
            if (!string.IsNullOrEmpty(estado))
            {
                query = query.Where(p => p.Estado == estado);
            }

            // 🔹 FILTRO POR FECHA DESDE
            if (fechaDesde.HasValue)
            {
                query = query.Where(p => p.Fecha >= fechaDesde.Value);
            }

            // 🔹 FILTRO POR FECHA HASTA
            if (fechaHasta.HasValue)
            {
                query = query.Where(p => p.Fecha <= fechaHasta.Value);
            }

            var pedidos = query
     .Select(p => new PedidoViewModel
     {
         IdPedido = p.IdPedido,
         Fecha = p.Fecha,
         Estado = p.Estado,

         ClienteNombre = _context.Clientes
             .Where(c => c.IdCliente == p.IdCliente)
             .Select(c => c.Nombre)
             .FirstOrDefault(),

         TipoCliente = _context.Clientes
             .Where(c => c.IdCliente == p.IdCliente)
             .Select(c => c.TipoCliente.Categoria)
             .FirstOrDefault(),

         Total = _context.PedidoProductos
             .Where(pp => pp.IdPedido == p.IdPedido)
             .Sum(pp => pp.Cantidad * pp.PrecioUnitario)
     })
     .ToList();

            ViewBag.Clientes = _context.Clientes.ToList();
            ViewBag.Productos = _context.Productos.ToList();

            return View(pedidos);
        }

        // ===============================
        // 🔹 CREAR PEDIDO (HU01)
        // ===============================
        public IActionResult Create()
        {
            var clientes = _context.Clientes
                .Include(c => c.TipoCliente)
                .ToList();
            var productos = _context.Productos.ToList();

            ViewBag.Clientes = clientes;
            ViewBag.Productos = productos;
            ViewBag.CantidadClientes = clientes.Count;

            return View();
        }



        //metodo guardar

        [HttpPost]
        public IActionResult Guardar([FromBody] PedidoRequest data)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                // =========================
                // VALIDACIONES GENERALES
                // =========================

                if (data.IdCliente <= 0)
                {
                    return Json(new { ok = false, mensaje = "Cliente inválido" });
                }

                if (data.Detalles == null || !data.Detalles.Any())
                {
                    return Json(new { ok = false, mensaje = "Debe agregar al menos un producto" });
                }

                foreach (var item in data.Detalles)
                {
                    if (item.Cantidad <= 0)
                    {
                        return Json(new { ok = false, mensaje = "Cantidad inválida" });
                    }
                }

                // =========================
// VALIDAR STOCK AGRUPADO
// =========================
var agrupados = data.Detalles
    .GroupBy(d => d.IdProducto)
    .Select(g => new
    {
        IdProducto = g.Key,
        CantidadTotal = g.Sum(x => x.Cantidad)
    });

foreach (var item in agrupados)
{
    var inventario = _context.Inventarios
        .FirstOrDefault(i => i.IdProducto == item.IdProducto);

    if (inventario == null)
    {
        return Json(new
        {
            ok = false,
            mensaje = $"No existe inventario para producto {item.IdProducto}"
        });
    }

    if (inventario.Stock < item.CantidadTotal)
    {
        var producto = _context.Productos
            .FirstOrDefault(p => p.IdProducto == item.IdProducto);

        return Json(new
        {
            ok = false,
            mensaje = $"Stock insuficiente para {producto?.Nombre}. Disponible: {inventario.Stock}"
        });
    }
}

                // =========================
                // CREAR PEDIDO
                // =========================
                var pedido = new Pedido
                {
                    Fecha = DateTime.Now,
                    IdCliente = data.IdCliente,
                    Estado = "Pendiente", // 🔥 importante
                    Activo = true
                };

                _context.Pedidos.Add(pedido);
                _context.SaveChanges();

                // =========================
                // GUARDAR DETALLES + DESCONTAR STOCK
                // =========================
                foreach (var item in data.Detalles)
                {
                    var inventario = _context.Inventarios
                        .First(i => i.IdProducto == item.IdProducto);

                    inventario.Stock -= item.Cantidad;

                    var detalle = new PedidoProducto
                    {
                        IdPedido = pedido.IdPedido,
                        IdProducto = item.IdProducto,
                        Cantidad = item.Cantidad,
                        PrecioUnitario = item.Precio,
                        Descuento = 0
                    };

                    _context.PedidoProductos.Add(detalle);
                }

                _context.SaveChanges();

                // =========================
                // COMMIT
                // =========================
                transaction.Commit();

                return Json(new { ok = true });
            }
            catch (Exception ex)
            {
                transaction.Rollback();

                return StatusCode(500, new
                {
                    ok = false,
                    mensaje = "Stock insuficiente",
                    error = ex.Message
                });
            }
        }

        // ===============================
        // 🔹 CANCELAR PEDIDO (HU04)
        // ===============================

        public IActionResult Cancelar(int id)
        {
            var pedido = _context.Pedidos.Find(id);

            if (pedido != null)
            {
                var detalles = _context.PedidoProductos
                    .Where(p => p.IdPedido == id)
                    .ToList();

                // 🔺 devolver stock
                foreach (var d in detalles)
                {
                    var inv = _context.Inventarios
                        .FirstOrDefault(i => i.IdProducto == d.IdProducto);

                    if (inv != null)
                        inv.Stock += d.Cantidad;
                }

                // 🔥 ACA VA
                pedido.Activo = false;
                pedido.Estado = "Cancelado";

                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // ===============================
        // 🔹 GET EDITAR PEDIDO
        // ===============================
        public IActionResult Edit(int id)
        {
            var pedido = _context.Pedidos
                .FirstOrDefault(p => p.IdPedido == id);

            if (pedido == null)
                return NotFound();

            // 👈 agregar esto
            ViewBag.Productos = _context.Productos
                .Select(p => new { p.IdProducto, p.Nombre, p.Precio })
                .ToList();

            var vm = new PedidoViewModel
            {
                IdPedido = pedido.IdPedido,
                Fecha = pedido.Fecha,
                IdCliente = pedido.IdCliente,

                Clientes = _context.Clientes
                    .Select(c => new SelectListItem
                    {
                        Value = c.IdCliente.ToString(),
                        Text = c.Nombre
                    }).ToList(),

                Detalles = _context.PedidoProductos
                    .Where(p => p.IdPedido == id)
                    .Include(d => d.Producto)
                    .Select(d => new PedidoDetalleViewModel
                    {
                        IdProducto = d.IdProducto,
                        Cantidad = d.Cantidad,
                        NombreProducto = d.Producto!.Nombre,
                        Precio = d.PrecioUnitario
                    }).ToList()
            };

            return View(vm);
        }

        // ===============================
        // 🔹 POST EDITAR PEDIDO
        // ===============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PedidoViewModel vm)
        {
            // Helper para recargar el VM en caso de error
            void RecargarVM()
            {
                vm.Clientes = _context.Clientes
                    .Select(c => new SelectListItem
                    {
                        Value = c.IdCliente.ToString(),
                        Text = c.Nombre
                    }).ToList();

                vm.Detalles = _context.PedidoProductos
                    .Where(p => p.IdPedido == vm.IdPedido)
                    .Select(d => new PedidoDetalleViewModel
                    {
                        IdProducto = d.IdProducto,
                        NombreProducto = d.Producto.Nombre, // 👈 nombre, no ID
                        Cantidad = d.Cantidad,
                        Precio = d.PrecioUnitario
                    }).ToList();
            }
            // Antes del if (!ModelState.IsValid)
            foreach (var key in ModelState.Keys.Where(k => k.Contains("NombreProducto")).ToList())
                ModelState.Remove(key);

            if (!ModelState.IsValid)
            {
                RecargarVM();
                return View(vm);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var pedido = await _context.Pedidos
                    .FirstOrDefaultAsync(p => p.IdPedido == vm.IdPedido);

                if (pedido == null)
                    return NotFound();

                // 1️⃣ Actualizar cliente
                pedido.IdCliente = vm.IdCliente;

                // 2️⃣ Traer detalles viejos
                var detallesViejos = _context.PedidoProductos
                    .Include(p => p.Producto)
                    .Where(p => p.IdPedido == vm.IdPedido)
                    .ToList();

                // 3️⃣ Devolver stock viejo al inventario
                foreach (var viejo in detallesViejos)
                {
                    var inv = _context.Inventarios
                        .FirstOrDefault(i => i.IdProducto == viejo.IdProducto);

                    if (inv != null)
                        inv.Stock += viejo.Cantidad; // 👈 devolver stock
                }

                // 4️⃣ Eliminar detalles viejos
                _context.PedidoProductos.RemoveRange(detallesViejos); // 👈 esto faltaba

                // 5️⃣ Validar stock y agregar nuevos detalles
                foreach (var item in vm.Detalles)
                {
                    var inv = _context.Inventarios
                        .FirstOrDefault(i => i.IdProducto == item.IdProducto);

                    var nombreProducto = _context.Productos
                        .Where(p => p.IdProducto == item.IdProducto)
                        .Select(p => p.Nombre)
                        .FirstOrDefault() ?? item.IdProducto.ToString();

                    if (inv == null || inv.Stock < item.Cantidad)
                    {
                        ModelState.AddModelError("",
                            $"Stock insuficiente para '{nombreProducto}'. Stock disponible: {inv?.Stock ?? 0}");

                        await transaction.RollbackAsync(); // 👈 rollback antes de volver
                        RecargarVM();
                        return View(vm);
                    }

                    _context.PedidoProductos.Add(new PedidoProducto
                    {
                        IdPedido = vm.IdPedido,
                        IdProducto = item.IdProducto,
                        Cantidad = item.Cantidad,
                        PrecioUnitario = item.Precio,
                        Descuento = 0
                    });

                    // 6️⃣ Descontar stock nuevo
                    inv.Stock -= item.Cantidad;
                }

                // 7️⃣ Recalcular total del pedido
           

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["mensaje"] = "Pedido actualizado correctamente ✅";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, ex.Message);
            }
        }
        public IActionResult Factura(int id)
        {
            var pedido = _context.Pedidos.FirstOrDefault(p => p.IdPedido == id);
            var cliente = _context.Clientes.FirstOrDefault(c => c.IdCliente == pedido.IdCliente);

            var detalles = _context.PedidoProductos
                .Where(d => d.IdPedido == id)
                .Include(d => d.Producto) // 👈 IMPORTANTE
                .ToList();

            using (MemoryStream ms = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdf = new PdfDocument(writer);
                Document doc = new Document(pdf);

                // 🧾 TITULO
                doc.Add(new Paragraph("FACTURA")
    .SetTextAlignment(TextAlignment.CENTER)
    .SetFontSize(20)
    .SetFont(PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD))
);

                doc.Add(new Paragraph(" "));

                // 🧾 DATOS
                doc.Add(new Paragraph($"Pedido N°: {id}"));
                doc.Add(new Paragraph($"Cliente: {cliente.Nombre}"));
                doc.Add(new Paragraph($"Fecha: {pedido.Fecha:dd/MM/yyyy}"));

                doc.Add(new Paragraph(" "));

                // 🧾 TABLA
                Table table = new Table(4).UseAllAvailableWidth();

                table.AddHeaderCell("Producto");
                table.AddHeaderCell("Cantidad");
                table.AddHeaderCell("Precio Unit.");
                table.AddHeaderCell("Subtotal");

                decimal total = 0;

                foreach (var d in detalles)
                {
                    var nombre = d.Producto?.Nombre ?? "Sin nombre";
                    var subtotal = d.Cantidad * d.PrecioUnitario;

                    table.AddCell(nombre);
                    table.AddCell(d.Cantidad.ToString());
                    table.AddCell(d.PrecioUnitario.ToString("C"));
                    table.AddCell(subtotal.ToString("C"));

                    total += subtotal;
                }

                doc.Add(table);

                doc.Add(new Paragraph(" "));

                // 🧾 TOTAL
                doc.Add(new Paragraph($"TOTAL: {total.ToString("C")}")
    .SetTextAlignment(TextAlignment.RIGHT)
    .SetFont(PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD))
);

                doc.Close();

                return File(ms.ToArray(), "application/pdf", $"Factura_Pedido_{id}.pdf");
            }
        }
    }
}
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

                    Total = _context.PedidoProductos
                        .Where(pp => pp.IdPedido == p.IdPedido)
                        .Sum(pp => pp.Cantidad * pp.PrecioUnitario)
                })
                .ToList();

            return View(pedidos);
        }

        // ===============================
        // 🔹 CREAR PEDIDO (HU01)
        // ===============================
        public IActionResult Create()
        {
            var clientes = _context.Clientes.ToList();
            var productos = _context.Productos.ToList();

            ViewBag.Clientes = clientes;
            ViewBag.Productos = productos;
            ViewBag.CantidadClientes = clientes.Count;

            return View();
        }

        // ===============================
        // 🔹 GUARDAR PEDIDO + STOCK (HU01)
        // ===============================
        [HttpPost]
        public IActionResult Guardar([FromBody] PedidoRequest data)
        {
            try
            {
                var pedido = new Pedido
                {
                    Fecha = DateTime.Now,
                    IdCliente = data.IdCliente
                };

                _context.Pedidos.Add(pedido);
                _context.SaveChanges();

                foreach (var item in data.Detalles)
                {
                    var detalle = new PedidoProducto
                    {
                        IdPedido = pedido.IdPedido,
                        IdProducto = item.IdProducto,
                        Cantidad = item.Cantidad,
                        PrecioUnitario = item.Precio,
                        Descuento = 0
                    };

                    _context.PedidoProductos.Add(detalle);

                    // 🔻 descontar stock
                    var inventario = _context.Inventarios
                        .FirstOrDefault(i => i.IdProducto == item.IdProducto);

                    if (inventario != null)
                        inventario.Stock -= item.Cantidad;
                }

                _context.SaveChanges();

                return Json(new { ok = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
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
        // 🔹 EDITAR PEDIDO (HU02)
        // ===============================
        public IActionResult Edit(int id)
        {
            var pedido = _context.Pedidos
                .FirstOrDefault(p => p.IdPedido == id);

            if (pedido == null)
                return NotFound();

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
                    .Select(d => new PedidoDetalleViewModel
                    {
                        IdProducto = d.IdProducto,
                        Cantidad = d.Cantidad,
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
            if (!ModelState.IsValid)
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
                        Cantidad = d.Cantidad,
                        Precio = d.PrecioUnitario
                    }).ToList();

                return View(vm);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var pedido = await _context.Pedidos
                    .FirstOrDefaultAsync(p => p.IdPedido == vm.IdPedido);

                if (pedido == null)
                    return NotFound();

                // actualizar cliente
                pedido.IdCliente = vm.IdCliente;

                // devolver stock viejo
                var detallesViejos = _context.PedidoProductos
                    .Where(p => p.IdPedido == vm.IdPedido)
                    .ToList();

                foreach (var item in vm.Detalles)
                {
                    var inv = _context.Inventarios
                        .FirstOrDefault(i => i.IdProducto == item.IdProducto);

                    // 🔴 VALIDACIÓN
                    if (inv == null || inv.Stock < item.Cantidad)
                    {
                        ModelState.AddModelError("", $"Stock insuficiente para el producto seleccionado {item.IdProducto}");

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
                                Cantidad = d.Cantidad,
                                Precio = d.PrecioUnitario
                            }).ToList();

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

                    // descontar stock
                    inv.Stock -= item.Cantidad;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

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
                .ToList();

            using (MemoryStream ms = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdf = new PdfDocument(writer);
                Document doc = new Document(pdf);

                // 🧾 TITULO
                doc.Add(new Paragraph("FACTURA"));

                doc.Add(new Paragraph($"Pedido N°: {id}"));
                doc.Add(new Paragraph($"Cliente: {cliente.Nombre}"));
                doc.Add(new Paragraph($"Fecha: {pedido.Fecha.ToString("dd/MM/yyyy")}"));

                doc.Add(new Paragraph(" "));

                // 🧾 TABLA
                Table table = new Table(3);

                table.AddHeaderCell("Producto");
                table.AddHeaderCell("Cantidad");
                table.AddHeaderCell("Precio");

                decimal total = 0;

                foreach (var d in detalles)
                {
                    table.AddCell(d.IdProducto.ToString());
                    table.AddCell(d.Cantidad.ToString());
                    table.AddCell(d.PrecioUnitario.ToString("C"));

                    total += d.Cantidad * d.PrecioUnitario;
                }

                doc.Add(table);

                doc.Add(new Paragraph($"TOTAL: ${total}"));

                doc.Close();

                return File(ms.ToArray(), "application/pdf", $"Factura_Pedido_{id}.pdf");
            }
        }
    }
}
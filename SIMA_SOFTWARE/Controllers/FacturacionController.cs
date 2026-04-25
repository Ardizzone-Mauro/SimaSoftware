using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIMA_SOFTWARE.Data;
using SIMA_SOFTWARE.Models;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Font;

namespace SIMA_SOFTWARE.Controllers
{
    public class FacturacionController : Controller
    {
        private readonly SimaDbContext _context;

        public FacturacionController(SimaDbContext context)
        {
            _context = context;
        }

        // =========================
        // 🔹 LISTADO FACTURAS
        // =========================
        public IActionResult Index()
        {
            var facturas = _context.Facturas
                .Include(f => f.Cliente)
                .Include(f => f.Pedido)
                .OrderByDescending(f => f.FechaEmision)
                .ToList();

            return View(facturas ?? new List<Factura>());
        }

        // =========================
        // 🔹 GENERAR FACTURA DESDE PEDIDO
        // =========================
        public IActionResult Generar(int idPedido)
        {
            var pedido = _context.Pedidos
                .Include(p => p.Cliente)
                .FirstOrDefault(p => p.IdPedido == idPedido);

            if (pedido == null)
                return NotFound();

            // 🚫 evitar doble facturación
            var existe = _context.Facturas
                .Any(f => f.IdPedido == idPedido);

            if (existe)
            {
                TempData["mensaje"] = "⚠️ Este pedido ya está facturado";
                return RedirectToAction("Index");
            }

            // 🔍 traer detalles del pedido
            var detalles = _context.PedidoProductos
                .Where(p => p.IdPedido == idPedido)
                .ToList();

            if (!detalles.Any())
            {
                TempData["mensaje"] = "⚠️ El pedido no tiene productos";
                return RedirectToAction("Index");
            }

            // 💰 calcular total
            decimal total = detalles.Sum(d => d.Cantidad * d.PrecioUnitario);

            // 🔢 numeración automática
            var ultimaFactura = _context.Facturas
                .OrderByDescending(f => f.IdFactura)
                .FirstOrDefault();

            int nuevoNumero = ultimaFactura != null ? ultimaFactura.Numero + 1 : 1;

            // 🧾 crear factura
            var factura = new Factura
            {
                FechaEmision = DateTime.Now,
                IdCliente = pedido.IdCliente,
                IdPedido = pedido.IdPedido,
                Numero = nuevoNumero,
                PuntoVenta = "0001",
                Tipo = "A",
                MontoTotal = total,
                Impuestos = total * 0.21m
            };

            _context.Facturas.Add(factura);
            _context.SaveChanges();

            // 🧾 detalles factura (opcional pero PRO)
            foreach (var d in detalles)
            {
                var detalleProducto = new DetalleProducto
                {
                    IdProducto = d.IdProducto,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Descuento = 0
                };

                _context.DetalleProductos.Add(detalleProducto);

                _context.DetalleFacturas.Add(new DetalleFactura
                {
                    Factura = factura,
                    DetalleProducto = detalleProducto
                });
            }

            // 💾 UN SOLO SAVE
            _context.SaveChanges();

            // 📌 opcional: marcar pedido como facturado
            pedido.Estado = "Facturado";
            _context.SaveChanges();

            TempData["mensaje"] = "✅ Factura generada correctamente";

            // 🔥 SIEMPRE volvemos al listado
            return RedirectToAction("Index");
        }
        public IActionResult Imprimir(int id)
        {
            var factura = _context.Facturas
                .Include(f => f.Cliente)
                .Include(f => f.DetallesFactura)
                .ThenInclude(df => df.DetalleProducto)
                .ThenInclude(dp => dp.Producto)
                .AsSplitQuery() // 👈 IMPORTANTE cuando hay muchos includes
                .FirstOrDefault(f => f.IdFactura == id);

            if (factura == null)
                return NotFound();

            return View("FacturaAFIP", factura); // 👈 nombre de la vista
        }
    }

}
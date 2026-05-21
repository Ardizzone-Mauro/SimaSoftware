using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIMA_SOFTWARE.Data;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Font;
using System.Globalization;

namespace SIMA_SOFTWARE.Controllers
{
    public class ReportesController : Controller
    {
        private readonly SimaDbContext _context;

        public ReportesController(SimaDbContext context)
        {
            _context = context;
        }

        // ==================================================
        // 🔵 HU02 - INFORME GERENCIAL
        // ==================================================
        public IActionResult VentasMensuales(string categoria)
        {
            var hoy = DateTime.Now;

            var inicioMesActual = new DateTime(hoy.Year, hoy.Month, 1);
            var inicioMesAnterior = inicioMesActual.AddMonths(-1);
            var finMesAnterior = inicioMesActual.AddDays(-1);

            // 🔵 FACTURAS
            var facturas = _context.Facturas
                .Include(f => f.DetallesFactura)
                    .ThenInclude(df => df.DetalleProducto)
                        .ThenInclude(dp => dp.Producto)
                .AsQueryable();

            // 🔵 FILTRO CATEGORIA
            if (!string.IsNullOrEmpty(categoria))
            {
                facturas = facturas.Where(f =>
                    f.DetallesFactura.Any(df =>
                        df.DetalleProducto.Producto.Categoria == categoria
                    )
                );
            }

            // 🔵 MES ACTUAL
            var totalMesActual = facturas
                .Where(f => f.FechaEmision >= inicioMesActual)
                .Sum(f => (decimal?)f.MontoTotal) ?? 0;

            // 🔵 MES ANTERIOR
            var totalMesAnterior = facturas
                .Where(f =>
                    f.FechaEmision >= inicioMesAnterior &&
                    f.FechaEmision <= finMesAnterior)
                .Sum(f => (decimal?)f.MontoTotal) ?? 0;

            // 🔵 KPI TICKET PROMEDIO
            var pedidosMesActual = facturas
                .Count(f => f.FechaEmision >= inicioMesActual);

            decimal ticketPromedio = 0;

            if (pedidosMesActual > 0)
            {
                ticketPromedio = totalMesActual / pedidosMesActual;
            }

            // 🔵 VIEWBAG
            ViewBag.TotalMesActual = totalMesActual;
            ViewBag.TotalMesAnterior = totalMesAnterior;
            ViewBag.TicketPromedio = ticketPromedio;
            ViewBag.Categoria = categoria;


            // =====================================================
            // 🔵 TOP PRODUCTOS
            // =====================================================

            var topProductos = (
                from pp in _context.PedidoProductos
                join p in _context.Productos
                    on pp.IdProducto equals p.IdProducto
                group pp by p.Nombre into g
                select new
                {
                    Producto = g.Key,
                    CantidadVendida = g.Sum(x => x.Cantidad)
                }
            )
            .OrderByDescending(x => x.CantidadVendida)
            .Take(5)
            .ToList();

            ViewBag.TopProductos = topProductos;
            // =====================================================
            // 🔵 TOP CLIENTES
            // =====================================================

            var topClientes = _context.PedidoProductos
    .Include(pp => pp.Pedido)
        .ThenInclude(p => p.Cliente)
    .AsEnumerable()
    .GroupBy(pp => pp.Pedido.Cliente.Nombre)
    .Select(g => new
    {
        Cliente = g.Key,
        Total = g.Sum(x => x.Cantidad * x.PrecioUnitario)
    })
    .OrderByDescending(x => x.Total)
    .Take(5)
    .ToList();

            ViewBag.TopClientesLabels = topClientes
                .Select(x => x.Cliente)
                .ToList();

            ViewBag.TopClientesData = topClientes
                .Select(x => x.Total)
                .ToList();

            ViewBag.TopClientes = topClientes;
            // =====================================================
            // 🔵 STOCK POR DEPÓSITO
            // =====================================================

            var depositos = (
                from i in _context.Inventarios
                join d in _context.Depositos
                    on i.IdDeposito equals d.IdDeposito
                group i by d.Nombre into g
                select new
                {
                    Deposito = g.Key,
                    Stock = g.Sum(x => x.Stock)
                }
            )
            .OrderByDescending(x => x.Stock)
            .ToList();

            ViewBag.DepositosLabels = depositos
                .Select(x => x.Deposito)
                .ToList();

            ViewBag.DepositosData = depositos
                .Select(x => x.Stock)
                .ToList();

            ViewBag.Depositos = depositos;



            return View();
        }

        // ==================================================
        // 🔵 EXPORTAR PDF
        // ==================================================
        public IActionResult ExportarPdf(string categoria)
        {
            var hoy = DateTime.Now;

            var inicioMesActual = new DateTime(hoy.Year, hoy.Month, 1);

            var facturas = _context.Facturas
                .Include(f => f.DetallesFactura)
                    .ThenInclude(df => df.DetalleProducto)
                        .ThenInclude(dp => dp.Producto)
                .AsQueryable();

            if (!string.IsNullOrEmpty(categoria))
            {
                facturas = facturas.Where(f =>
                    f.DetallesFactura.Any(df =>
                        df.DetalleProducto.Producto.Categoria == categoria
                    )
                );
            }

            var total = facturas
                .Where(f => f.FechaEmision >= inicioMesActual)
                .Sum(f => (decimal?)f.MontoTotal) ?? 0;

            var cantidad = facturas
                .Count(f => f.FechaEmision >= inicioMesActual);

            decimal ticketPromedio = 0;

            if (cantidad > 0)
                ticketPromedio = total / cantidad;

            using MemoryStream ms = new MemoryStream();

            PdfWriter writer = new PdfWriter(ms);
            PdfDocument pdf = new PdfDocument(writer);
            Document doc = new Document(pdf);

            // 🔵 TITULO
            doc.Add(new Paragraph("SIMA SOFTWARE - Rueca Andina Hilados")
    .SetTextAlignment(TextAlignment.CENTER)
    .SetFontSize(20)
    .SetFont(
        PdfFontFactory.CreateFont(
            iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD
        )
    )
);

            doc.Add(new Paragraph("Informe Gerencial de Ventas")
    .SetTextAlignment(TextAlignment.CENTER)
    .SetFontSize(16)
    .SetFont(
        PdfFontFactory.CreateFont(
            iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD
        )
    )
);

            doc.Add(new Paragraph(" "));

            doc.Add(new Paragraph($"Fecha: {DateTime.Now:dd/MM/yyyy}"));
            doc.Add(new Paragraph($"Categoría: {categoria ?? "Todas"}"));

            doc.Add(new Paragraph(" "));

            doc.Add(new Paragraph($"Total Facturado Mes Actual: ${total:N2}"));
            doc.Add(new Paragraph($"Ticket Promedio: ${ticketPromedio:N2}"));

            doc.Close();

            return File(
                ms.ToArray(),
                "application/pdf",
                "InformeVentas.pdf"
            );
        }
    }
}

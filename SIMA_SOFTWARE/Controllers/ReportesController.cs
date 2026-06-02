using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Geom;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Layout.Borders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIMA_SOFTWARE.Data;
using System.Globalization;

namespace SIMA_SOFTWARE.Controllers
{
    [Authorize(Roles = "Administrador del Sistema")]
    public class ReportesController : Controller
    {
        private readonly SimaDbContext _context;

        public ReportesController(SimaDbContext context)
        {
            _context = context;
        }

        private void AgregarGrafico(Document doc, string base64)
        {
            if (string.IsNullOrEmpty(base64))
                return;

            var bytes = Convert.FromBase64String(
                base64.Substring(base64.IndexOf(",") + 1));

            var imageData =
                iText.IO.Image.ImageDataFactory.Create(bytes);

            var image =
                new iText.Layout.Element.Image(imageData);

            image.SetWidth(380);

            doc.Add(image);
            doc.Add(new Paragraph(" "));
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
            ViewBag.TopProductosLabels = topProductos
    .Select(x => x.Producto)
    .ToList();

            ViewBag.TopProductosData = topProductos
                .Select(x => x.CantidadVendida)
                .ToList();
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
        [HttpPost]
        public IActionResult ExportarPdf(
     string categoria,
     string ventasImg,
     string productosImg,
     string clientesImg,
     string depositosImg)
        {
            var hoy = DateTime.Now;

            var inicioMesActual = new DateTime(hoy.Year, hoy.Month, 1);
            var inicioMesAnterior = inicioMesActual.AddMonths(-1);
            var finMesAnterior = inicioMesActual.AddDays(-1);

            var facturas = _context.Facturas
                .Include(f => f.DetallesFactura)
                    .ThenInclude(df => df.DetalleProducto)
                        .ThenInclude(dp => dp.Producto)
                .AsQueryable();

            if (!string.IsNullOrEmpty(categoria))
            {
                facturas = facturas.Where(f =>
                    f.DetallesFactura.Any(df =>
                        df.DetalleProducto.Producto.Categoria == categoria));
            }

            decimal totalMesActual = facturas
                .Where(f => f.FechaEmision >= inicioMesActual)
                .Sum(f => (decimal?)f.MontoTotal) ?? 0;

            decimal totalMesAnterior = facturas
                .Where(f =>
                    f.FechaEmision >= inicioMesAnterior &&
                    f.FechaEmision <= finMesAnterior)
                .Sum(f => (decimal?)f.MontoTotal) ?? 0;

            int pedidosMesActual = facturas
                .Count(f => f.FechaEmision >= inicioMesActual);

            decimal ticketPromedio = pedidosMesActual > 0
                ? totalMesActual / pedidosMesActual
                : 0;

            // =====================================
            // TOP PRODUCTOS
            // =====================================

            var topProductos = (
                from pp in _context.PedidoProductos
                join p in _context.Productos
                    on pp.IdProducto equals p.IdProducto
                group pp by p.Nombre into g
                select new
                {
                    Producto = g.Key,
                    Cantidad = g.Sum(x => x.Cantidad)
                })
                .OrderByDescending(x => x.Cantidad)
                .Take(5)
                .ToList();

            // =====================================
            // TOP CLIENTES
            // =====================================

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

            // =====================================
            // STOCK DEPOSITOS
            // =====================================

            var depositos = (
                from i in _context.Inventarios
                join d in _context.Depositos
                    on i.IdDeposito equals d.IdDeposito
                group i by d.Nombre into g
                select new
                {
                    Deposito = g.Key,
                    Stock = g.Sum(x => x.Stock)
                })
                .OrderByDescending(x => x.Stock)
                .ToList();

            using MemoryStream ms = new MemoryStream();

            PdfWriter writer = new PdfWriter(ms);
            PdfDocument pdf = new PdfDocument(writer);
            Document doc = new Document(pdf);

            doc.SetMargins(30, 30, 30, 30);

            // =====================================
            // LOGO
            // =====================================

            var logoPath = System.IO.Path.Combine(
    Directory.GetCurrentDirectory(),
    "wwwroot",
    "images",
    "logo_rueca.png");

            if (System.IO.File.Exists(logoPath))
            {
                var logoData =
                    iText.IO.Image.ImageDataFactory.Create(logoPath);

                var logo =
                    new iText.Layout.Element.Image(logoData);

                logo.SetWidth(120);

                logo.SetHorizontalAlignment(
                    HorizontalAlignment.CENTER);

                doc.Add(logo);
            }

            // =====================================
            // TITULOS
            // =====================================

            doc.Add(
                new Paragraph("SIMA SOFTWARE")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(22));

            doc.Add(
                new Paragraph("Rueca Andina Hilados")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(16));

            doc.Add(
                new Paragraph("Informe Gerencial de Ventas")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(14));

            doc.Add(
                new Paragraph(
                    $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm:ss}")
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetFontSize(10));

            doc.Add(new Paragraph(" "));

            // =====================================
            // DATOS GENERALES
            // =====================================

            doc.Add(new Paragraph($"Categoría: {categoria ?? "Todas"}"));

            doc.Add(new Paragraph(" "));

            // =====================================
            // KPI
            // =====================================

            doc.Add(
                new Paragraph("Indicadores Principales")
                    .SetFontSize(14));

            Table kpiTable = new Table(3);
            kpiTable.SetWidth(UnitValue.CreatePercentValue(100));

            kpiTable.AddHeaderCell("Mes Actual");
            kpiTable.AddHeaderCell("Mes Anterior");
            kpiTable.AddHeaderCell("Ticket Promedio");

            kpiTable.AddCell($"${totalMesActual:N2}");
            kpiTable.AddCell($"${totalMesAnterior:N2}");
            kpiTable.AddCell($"${ticketPromedio:N2}");

            doc.Add(kpiTable);

            doc.Add(new Paragraph(" "));

            // =====================================
            // GRAFICO VENTAS
            // =====================================

            doc.Add(new Paragraph("Comparativa de Ventas"));

            AgregarGrafico(doc, ventasImg);

            // =====================================
            // TOP PRODUCTOS
            // =====================================

            doc.Add(new Paragraph("Top Productos"));

            Table tablaProductos = new Table(2);
            tablaProductos.SetWidth(UnitValue.CreatePercentValue(100));

            tablaProductos.AddHeaderCell("Producto");
            tablaProductos.AddHeaderCell("Cantidad Vendida");

            foreach (var item in topProductos)
            {
                tablaProductos.AddCell(item.Producto);
                tablaProductos.AddCell(item.Cantidad.ToString());
            }

            doc.Add(tablaProductos);

            doc.Add(new Paragraph(" "));
            AgregarGrafico(doc, productosImg);

            // =====================================
            // TOP CLIENTES
            // =====================================

            doc.Add(new Paragraph("Top Clientes"));

            Table tablaClientes = new Table(2);
            tablaClientes.SetWidth(UnitValue.CreatePercentValue(100));

            tablaClientes.AddHeaderCell("Cliente");
            tablaClientes.AddHeaderCell("Total Comprado");

            foreach (var item in topClientes)
            {
                tablaClientes.AddCell(item.Cliente);
                tablaClientes.AddCell($"${item.Total:N2}");
            }

            doc.Add(tablaClientes);

            doc.Add(new Paragraph(" "));
            AgregarGrafico(doc, clientesImg);

            // =====================================
            // STOCK DEPOSITOS
            // =====================================

            doc.Add(new Paragraph("Stock por Depósito"));

            Table tablaDepositos = new Table(2);
            tablaDepositos.SetWidth(UnitValue.CreatePercentValue(100));

            tablaDepositos.AddHeaderCell("Depósito");
            tablaDepositos.AddHeaderCell("Stock");

            foreach (var item in depositos)
            {
                tablaDepositos.AddCell(item.Deposito);
                tablaDepositos.AddCell(item.Stock.ToString());
            }

            doc.Add(tablaDepositos);

            doc.Add(new Paragraph(" "));
            AgregarGrafico(doc, depositosImg);

            // =====================================
            // PIE
            // =====================================

            doc.Add(new Paragraph(" "));

            doc.Add(
                new Paragraph(
                    "Documento generado automáticamente por SIMA SOFTWARE")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(10));

            doc.Close();

            return File(
                ms.ToArray(),
                "application/pdf",
                $"InformeVentas_{DateTime.Now:yyyyMMdd}.pdf");
        }
    }
}

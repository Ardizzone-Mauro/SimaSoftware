using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIMA_SOFTWARE.Data;
using SIMA_SOFTWARE.Models.ViewModels;

namespace SIMA_SOFTWARE.Controllers
{
    //[Authorize(Roles = "Administrador del Sistema, Cliente")]
    public class DashboardController : Controller
    {

        private readonly SimaDbContext _context;
        public DashboardController(SimaDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new DashboardViewModel();

            // 🔹 CLIENTES
            model.TotalClientes = await _context.Clientes.CountAsync();

            // 🔹 PEDIDOS ACTIVOS
            model.PedidosActivos = await _context.Pedidos
                .CountAsync(p => p.Estado != "Entregado");

            // 🔹 PEDIDOS LISTOS
            model.PedidosListosEnvio = await _context.Pedidos
                .CountAsync(p => p.Estado == "Listo");

            // 🔹 STOCK TOTAL
            model.StockTotal = await _context.Inventarios
                .SumAsync(i => (int?)i.Stock) ?? 0;

            // 🔹 STOCK BAJO
            model.StockBajo = await _context.Inventarios
                .CountAsync(i => i.Stock <= 5);

            // 🔹 INGRESOS
            var inicioMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            model.IngresosMensuales = await _context.Facturas
                .Where(f => f.FechaEmision >= inicioMes)
                .SumAsync(f => (decimal?)f.MontoTotal) ?? 0;

            // 🔹 VARIACIÓN
            var inicioMesAnterior = inicioMes.AddMonths(-1);
            var finMesAnterior = inicioMes.AddDays(-1);

            var ingresosMesAnterior = await _context.Facturas
                .Where(f => f.FechaEmision >= inicioMesAnterior && f.FechaEmision <= finMesAnterior)
                .SumAsync(f => (decimal?)f.MontoTotal) ?? 0;

            model.VariacionIngresos = ingresosMesAnterior == 0
                ? 100
                : ((double)(model.IngresosMensuales - ingresosMesAnterior) / (double)ingresosMesAnterior) * 100;

            // =========================================================
            // 🟣 PASO 1 — TRAER PEDIDOS (DB)
            // =========================================================

            var pedidos = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.PedidoProductos)
                .OrderByDescending(p => p.Fecha)
                .Take(5)
                .ToListAsync();

            // =========================================================
            // 🟣 PASO 2 — CONVERTIR A VIEWMODEL (MEMORIA)
            // =========================================================

            model.PedidosRecientes = pedidos.Select(p => new PedidoViewModel
            {
                IdPedido = p.IdPedido,
                Fecha = p.Fecha,
                Estado = p.Estado,
                ClienteNombre = p.Cliente?.Nombre,

                Total = p.PedidoProductos != null
                    ? p.PedidoProductos.Sum(pp => pp.Cantidad * pp.PrecioUnitario)
                    : 0
            }).ToList();

            return View(model);
        }
    }
}
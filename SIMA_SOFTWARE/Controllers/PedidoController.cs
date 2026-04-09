using Microsoft.AspNetCore.Mvc;
using SIMA_Software.Models;
using SIMA_SOFTWARE.Data;
using System.Linq;

namespace SIMA_SOFTWARE.Controllers
{
    public class PedidoController : Controller
    {
        private readonly SimaDbContext _context;

        public PedidoController(SimaDbContext context)
        {
            _context = context;
        }

        public IActionResult Create()
        {
            ViewBag.Clientes = _context.Clientes.ToList();
            ViewBag.Productos = _context.Productos.ToList();

            return View();
        }
    
    [HttpPost]
        public IActionResult Guardar([FromBody] dynamic data)
        {
            int idCliente = data.idCliente;

            var pedido = new Pedido
            {
                Fecha = DateTime.Now,
                IdCliente = idCliente
            };

            _context.Pedidos.Add(pedido);
            _context.SaveChanges();

            // recorrer productos
            foreach (var item in data.detalles)
            {
                int idProducto = item.idProducto;
                int cantidad = item.cantidad;
                decimal precio = item.precio;

                var detalle = new PedidoProducto
                {
                    IdPedido = pedido.IdPedido,
                    IdProducto = idProducto,
                    Cantidad = cantidad,
                    PrecioUnitario = precio,
                    Descuento = 0
                };

                _context.PedidoProductos.Add(detalle);

                // descontar stock
                var inventario = _context.Inventarios
                    .FirstOrDefault(i => i.IdProducto == idProducto);

                if (inventario != null)
                    inventario.Stock -= cantidad;
            }

            _context.SaveChanges();

            return Json(new { ok = true });
        }
        public IActionResult Index()
        {
            var pedidos = _context.Pedidos
                .Select(p => new
                {
                    p.IdPedido,
                    p.Fecha,
                    Cliente = p.Cliente.Nombre,
                    Total = _context.PedidoProductos
                        .Where(pp => pp.IdPedido == p.IdPedido)
                        .Sum(pp => pp.Cantidad * pp.PrecioUnitario)
                }).ToList();

            return View(pedidos);
        }
    }
}
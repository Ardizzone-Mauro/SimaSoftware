using Microsoft.AspNetCore.Mvc;
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
    }
}
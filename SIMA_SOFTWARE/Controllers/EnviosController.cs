using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIMA_SOFTWARE.Data;

namespace SIMA_SOFTWARE.Controllers
{
    public class EnviosController : Controller
    {
        private readonly SimaDbContext _context;

        public EnviosController(SimaDbContext context)
        {
            _context = context;
        }

        // LISTADO
        public IActionResult Index()
        {
            var envios = _context.Envios
                .Include(e => e.Pedido)
                .Include(e => e.Estado)
                .OrderByDescending(e => e.Fecha)
                .ToList();

            return View(envios);
        }
    }
}
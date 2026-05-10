using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIMA_SOFTWARE.Data;

namespace SIMA_SOFTWARE.Controllers
{
    public class NotificacionesController : Controller
    {
        private readonly SimaDbContext _context;

        public NotificacionesController(SimaDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var notificaciones = _context.Notificaciones
                .Include(n => n.Cliente)
                .Include(n => n.Pedido)
                .OrderByDescending(n => n.IdNotificacion)
                .ToList();

            return View(notificaciones);
        }
    }
}
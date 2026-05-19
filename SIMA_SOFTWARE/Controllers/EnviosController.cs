using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIMA_SOFTWARE.Data;
using SIMA_SOFTWARE.Models;

namespace SIMA_SOFTWARE.Controllers
{
    public class EnviosController : Controller
    {
        private readonly SimaDbContext _context;

        public EnviosController(SimaDbContext context)
        {
            _context = context;
        }

        // =========================
        // INDEX
        // =========================
        public async Task<IActionResult> Index()
        {
            var envios = await _context.Envios
                .Include(e => e.Pedido)
                    .ThenInclude(p => p.Cliente)
                .Include(e => e.Estado)
                .OrderByDescending(e => e.Fecha)
                .ToListAsync();

            return View(envios);
        }

        // =========================
        // CREATE GET
        // =========================
        public IActionResult Create()
        {
            CargarCombos();

            return View(new Envio
            {
                Fecha = DateTime.Now
            });
        }

        // =========================
        // CREATE POST
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Envio envio)
        {
            // DEBUG
            if (envio.IdEstado == 0)
            {
                ModelState.AddModelError("", "Debe seleccionar un estado.");
            }

            if (envio.IdPedido == 0)
            {
                ModelState.AddModelError("", "Debe seleccionar un pedido.");
            }

            if (ModelState.IsValid)
            {
                _context.Envios.Add(envio);

                await _context.SaveChangesAsync();

                TempData["mensaje"] = "✅ Envío registrado correctamente";

                return RedirectToAction(nameof(Index));
            }

            CargarCombos();

            return View(envio);
        }

        // =========================
        // DELETE
        // =========================
        public async Task<IActionResult> Delete(int id)
        {
            var envio = await _context.Envios.FindAsync(id);

            if (envio != null)
            {
                _context.Envios.Remove(envio);

                await _context.SaveChangesAsync();

                TempData["mensaje"] = "❌ Envío eliminado";
            }

            return RedirectToAction(nameof(Index));
        }
        // =========================
        // DETAILS
        // =========================
        public async Task<IActionResult> Details(int id)
        {
            var envio = await _context.Envios
                .Include(e => e.Pedido)
                    .ThenInclude(p => p.Cliente)
                .Include(e => e.Estado)
                .FirstOrDefaultAsync(e => e.IdEnvio == id);

            if (envio == null)
            {
                return NotFound();
            }

            return View(envio);
        }
        // =========================
        // EDIT GET
        // =========================
        public async Task<IActionResult> Edit(int id)
        {
            var envio = await _context.Envios.FindAsync(id);

            if (envio == null)
            {
                return NotFound();
            }

            CargarCombos();

            return View(envio);
        }

        // =========================
        // EDIT POST
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Envio envio)
        {
            if (id != envio.IdEnvio)
                return NotFound();

            if (envio.IdEstado == 0 || envio.IdPedido == 0)
                return View(envio);

            var envioDb = await _context.Envios
                .Include(e => e.Estado)
                .FirstOrDefaultAsync(e => e.IdEnvio == id);

            if (envioDb == null)
                return NotFound();

            // actualizar estado del envío
            envioDb.IdEstado = envio.IdEstado;
            envioDb.Fecha = DateTime.Now;

            await _context.SaveChangesAsync();

            // 🔥 ACTUALIZAR ESTADO DEL PEDIDO AUTOMÁTICAMENTE
            var pedido = await _context.Pedidos.FindAsync(envio.IdPedido);

            var estadoActual = await _context.Estados
                .FirstOrDefaultAsync(e => e.IdEstado == envio.IdEstado);

            if (pedido != null && estadoActual != null)
            {
                pedido.Estado = estadoActual.Descripcion;
            }

            await _context.SaveChangesAsync();

            TempData["mensaje"] = "Envío actualizado y pedido sincronizado";
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // MÉTODO PRIVADO
        // =========================
        private void CargarCombos()
        {
            var pedidos = _context.Pedidos
                .Include(p => p.Cliente)
                .ToList()
                .Select(p => new
                {
                    IdPedido = p.IdPedido,
                    Texto = $"Pedido #{p.IdPedido} - {p.Cliente!.Nombre}"
                });

            ViewBag.Pedidos = new SelectList(
                pedidos,
                "IdPedido",
                "Texto"
            );

            ViewBag.Estados = new SelectList(
                _context.Estados.ToList(),
                "IdEstado",
                "Descripcion"
            );
        }
    }
}
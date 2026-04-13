using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SIMA_SOFTWARE.Controllers
{
    [Authorize(Roles = "Administrador del Sistema, Cliente")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SIMA_SOFTWARE.Models;
using SIMA_SOFTWARE.Models.ViewModels;

namespace SIMA_SOFTWARE.Controllers
{
    //[Authorize(Roles = "Administrador del Sistema")]
    public class RolesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        //Listar Usuarios , roles y permisos
        public async Task<IActionResult> Index()
        {
            var usuarios = _userManager.Users.ToList();
            var roles = _roleManager.Roles.ToList();

            var lista = new List<UsuarioRolViewModel>();

            foreach (var user in usuarios)
            {
                var rolesUsuario = await _userManager.GetRolesAsync(user);
                var claims = await _userManager.GetClaimsAsync(user);

                lista.Add(new UsuarioRolViewModel
                {
                    UserId = user.Id,
                    Email = user.Email,
                    NombreCompleto = $"{user.Nombre} {user.Apellido}",

                    RolesAsignados = rolesUsuario.ToList(),

                    TodosLosRoles = roles.Select(r => new SelectListItem
                    {
                        Value = r.Name,
                        Text = r.Name
                    }).ToList(),

                    PermisosAsignados = claims
                        .Where(c => c.Type == "Permiso")
                        .Select(c => c.Value)
                        .ToList()
                });
            }

            return View(lista);
        }


        //Guardar multiples roles para un usuario
        [HttpPost]
    public async Task<IActionResult> AsignarRoles(string userId, List<string> rolesSeleccionados)
    {
         var user = await _userManager.FindByIdAsync(userId);

        if (user != null)
        {
            var rolesActuales = await _userManager.GetRolesAsync(user);

             // eliminar todos
            await _userManager.RemoveFromRolesAsync(user, rolesActuales);

            // agregar nuevos
            if (rolesSeleccionados != null && rolesSeleccionados.Any())
            {
                await _userManager.AddToRolesAsync(user, rolesSeleccionados);
            }
        }

        return RedirectToAction("Index");
        }




        public async Task<IActionResult> Permisos(string rol)
        {
            var roles = _roleManager.Roles
                .Select(r => r.Name)
                .ToList();

            if (string.IsNullOrEmpty(rol))
                rol = roles.FirstOrDefault();

            var role = await _roleManager.FindByNameAsync(rol);

            var claims = await _roleManager.GetClaimsAsync(role);

            // 🔥 LISTA COMPLETA DE PERMISOS
            var todosLosPermisos = new List<string>
    {
        // Usuarios
        "Usuarios.Ver",
        "Usuarios.Crear",
        "Usuarios.Editar",
        "Usuarios.Eliminar",

        // Productos
        "Productos.Ver",
        "Productos.Crear",
        "Productos.Editar",
        "Productos.Eliminar",

        // Ventas
        "Ventas.Ver",
        "Ventas.Crear",
        "Ventas.Editar",
        "Ventas.Eliminar"
    };

            var vm = new RolPermisoViewModel
            {
                RolSeleccionado = rol,

                Roles = roles,

                // 🔥 IMPORTANTE
                TodosLosPermisos = todosLosPermisos,

                PermisosSeleccionados = claims
                    .Where(c => c.Type == "Permiso")
                    .Select(c => c.Value)
                    .ToList(),

                CantidadUsuarios = (await _userManager
                    .GetUsersInRoleAsync(rol)).Count
            };

            return View(vm);
        }











        //POST (guardar permisos)


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuardarPermisosRol(string rol, List<string> permisosSeleccionados)
        {
            var role = await _roleManager.FindByNameAsync(rol);

            if (role != null)
            {
                var claims = await _roleManager.GetClaimsAsync(role);

                foreach (var c in claims.Where(c => c.Type == "Permiso"))
                    await _roleManager.RemoveClaimAsync(role, c);

                if (permisosSeleccionados != null)
                {
                    foreach (var p in permisosSeleccionados)
                    {
                        await _roleManager.AddClaimAsync(role,
                            new System.Security.Claims.Claim("Permiso", p));
                    }
                }
            }

            return RedirectToAction("Permisos", new { rol });
        }

      

    }
}

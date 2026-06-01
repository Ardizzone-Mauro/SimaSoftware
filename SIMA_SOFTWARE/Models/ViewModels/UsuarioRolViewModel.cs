using Microsoft.AspNetCore.Mvc.Rendering;

namespace SIMA_SOFTWARE.Models.ViewModels
{
    public class UsuarioRolViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string NombreCompleto { get; set; }
        public bool Activo { get; set; }

        // ROLES
        public List<string> RolesAsignados { get; set; } = new();
        public List<SelectListItem> TodosLosRoles { get; set; } = new();

        // PERMISOS
        public List<string> PermisosAsignados { get; set; } = new();

        public List<string> TodosLosPermisos { get; set; } = new()
    {
        "Usuarios.Ver", "Usuarios.Crear", "Usuarios.Editar", "Usuarios.Eliminar",
        "Productos.Ver", "Productos.Crear", "Productos.Editar", "Productos.Eliminar",
        "Ventas.Ver", "Ventas.Crear", "Ventas.Editar", "Ventas.Eliminar"
    };
    }
}

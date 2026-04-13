namespace SIMA_SOFTWARE.Models.ViewModels
{
    public class RolPermisoViewModel
    {
        public string RolSeleccionado { get; set; }

        public List<string> Roles { get; set; } = new();

        public List<string> PermisosSeleccionados { get; set; } = new();

        public List<string> TodosLosPermisos { get; set; } = new()
    {
        "Usuarios.Ver", "Usuarios.Crear", "Usuarios.Editar", "Usuarios.Eliminar",
        "Productos.Ver", "Productos.Crear", "Productos.Editar", "Productos.Eliminar",
        "Ventas.Ver", "Ventas.Crear", "Ventas.Editar", "Ventas.Eliminar",
        "Reportes.Ver", "Reportes.Crear", "Reportes.Editar", "Reportes.Eliminar"
    };

        public int CantidadUsuarios { get; set; }
    }
}

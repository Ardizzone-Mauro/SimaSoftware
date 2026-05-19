using System.ComponentModel.DataAnnotations;

namespace SIMA_SOFTWARE.Models
{
    public class Producto
    {
        [Key]
        public int IdProducto { get; set; }

        // 🧶 Nombre base del producto (OVEJA, LLAMA, etc.)
        public string? Nombre { get; set; }

        // 💰 Precio
        public decimal Precio { get; set; }

        // 📝 Descripción opcional
        public string? Descripcion { get; set; }

        // 🖼 Imagen
        public string? UrlImagen { get; set; } = null;

        // 🟢 LINEA DE PRODUCTO
        public string? Categoria { get; set; } // "Linea Rueca" / "Linea Telar"

        // 🎨 COLOR DEL PRODUCTO
        public string? Color { get; set; }

        // 🧵 CANTIDAD DE HEBRAS
        public int? CantidadHebras { get; set; }

        // 🔥 BORRADO LÓGICO
        public bool Eliminado { get; set; } = false;
        public DateTime? FechaEliminacion { get; set; }

        // RELACIONES
        public ICollection<Inventario>? Inventarios { get; set; }
        public ICollection<PedidoProducto>? PedidoProductos { get; set; }
        public ICollection<DetalleProducto>? DetallesProducto { get; set; }
    }
}

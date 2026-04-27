using System.ComponentModel.DataAnnotations;

namespace SIMA_SOFTWARE.Models
{
    public class Producto
    {
        [Key]
        public int IdProducto { get; set; }
        public string? Nombre { get; set; }
        public decimal Precio { get; set; }
        public string? Descripcion { get; set; }
        public string? UrlImagen { get; set; } = null;
        public bool Eliminado { get; set; } = false;//agrego para borrado logico 27/4
        public DateTime? FechaEliminacion { get; set; }//agrego borrado logico 27/4

        public ICollection<Inventario>? Inventarios { get; set; }
        public ICollection<PedidoProducto>? PedidoProductos { get; set; }
        public ICollection<DetalleProducto>? DetallesProducto { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace SIMA_Software.Models
{
    public class Producto
    {
        [Key]
        public int IdProducto { get; set; }
        public string? Nombre { get; set; }
        public decimal Precio { get; set; }

        public string? UrlImagen { get; set; }

        public ICollection<Inventario>? Inventarios { get; set; }
        public ICollection<PedidoProducto>? PedidoProductos { get; set; }
        public ICollection<DetalleProducto>? DetallesProducto { get; set; }
    }
}

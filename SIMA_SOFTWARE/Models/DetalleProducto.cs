using System.ComponentModel.DataAnnotations;

namespace SIMA_Software.Models
{
    public class DetalleProducto
    {
        [Key]
        public int IdDetalleProducto { get; set; }

        public int IdProducto { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
        public decimal Descuento { get; set; }

        public Producto? Producto { get; set; }

        public ICollection<DetalleFactura>? DetallesFactura { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace SIMA_SOFTWARE.Models
{
    public class DetalleFactura
    {
        [Key]
        public int IdDetalleFactura { get; set; }

        public int IdFactura { get; set; }
        public int IdDetalleProducto { get; set; }

        public Factura? Factura { get; set; }
        public DetalleProducto? DetalleProducto { get; set; }
    }
}

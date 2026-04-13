using System.ComponentModel.DataAnnotations;

namespace SIMA_SOFTWARE.Models
{
    public class PedidoProducto
    {
        [Key]
        public int IdPedidoProducto { get; set; }

        public int IdPedido { get; set; }
        public Pedido? Pedido { get; set; }
        public int IdProducto { get; set; }
        public Producto? Producto { get; set; }

        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
        public decimal Descuento { get; set; }

    }
}


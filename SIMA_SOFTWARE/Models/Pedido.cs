using System.ComponentModel.DataAnnotations;

namespace SIMA_SOFTWARE.Models
{
    public class Pedido
    {
        [Key]
        public int IdPedido { get; set; }
        public DateTime Fecha { get; set; }

        public int IdCliente { get; set; }
        public Cliente? Cliente { get; set; }
        public bool Activo { get; set; } = true;
        public string Estado { get; set; } = "Pendiente";

        public ICollection<PedidoProducto>? PedidoProductos { get; set; }
        public ICollection<Envio>? Envios { get; set; }
    }
}

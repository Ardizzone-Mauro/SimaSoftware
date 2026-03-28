using System.ComponentModel.DataAnnotations;
namespace SIMA_Software.Models
{
    public class Notificacion
    {
        [Key]
        public int IdNotificacion { get; set; }
        public string? Mensaje { get; set; }

        public int IdCliente { get; set; }
        public int IdPedido { get; set; }

        public Cliente? Cliente { get; set; }
        public Pedido? Pedido { get; set; }
    }
}

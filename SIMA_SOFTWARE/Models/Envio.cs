using System.ComponentModel.DataAnnotations;

namespace SIMA_Software.Models
{
    public class Envio
    {
        [Key]
        public int IdEnvio { get; set; }
        public DateTime Fecha { get; set; }

        public int IdPedido { get; set; }
        public int IdEstado { get; set; }

        public Pedido? Pedido { get; set; }
        public Estado? Estado { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIMA_SOFTWARE.Models
{
    public class Envio
    {
        [Key]
        public int IdEnvio { get; set; }

        public DateTime Fecha { get; set; }

        // 🔥 FK
        public int IdPedido { get; set; }

        [ForeignKey("IdPedido")]
        public Pedido? Pedido { get; set; }

        // 🔥 OPCIONAL (estado del envío)
        public int? IdEstado { get; set; }
        public Estado? Estado { get; set; }
    }
}
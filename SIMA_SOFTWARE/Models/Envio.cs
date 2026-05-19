using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIMA_SOFTWARE.Models
{
    public class Envio
    {
        [Key]
        public int IdEnvio { get; set; }

        public DateTime Fecha { get; set; }

        // =========================
        // PEDIDO
        // =========================

        public int IdPedido { get; set; }

        [ForeignKey(nameof(IdPedido))]
        public Pedido? Pedido { get; set; }

        // =========================
        // ESTADO
        // =========================

        public int? IdEstado { get; set; }

        [ForeignKey(nameof(IdEstado))]
        public Estado? Estado { get; set; }
    }
}
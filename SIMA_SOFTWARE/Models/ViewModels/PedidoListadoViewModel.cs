using System;

namespace SIMA_SOFTWARE.Models.ViewModels
{
    public class PedidoListadoViewModel
    {
        public int IdPedido { get; set; }

        public string Cliente { get; set; } = string.Empty;

        public DateTime Fecha { get; set; }

        public string Estado { get; set; } = string.Empty;

        public decimal Total { get; set; }
    }
}
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SIMA_SOFTWARE.Models.ViewModels
{
    public class PedidoViewModel
    {
        public int IdPedido { get; set; }
        public DateTime Fecha { get; set; }
        public int IdCliente { get; set; }

        public string? ClienteNombre { get; set; }

        public List<SelectListItem>? Clientes { get; set; }

        public List<PedidoDetalleViewModel> Detalles { get; set; } = new();

        public decimal Total { get; set; }
        public string? Estado { get; set; }
    }

    public class PedidoDetalleViewModel
    {
        public int IdProducto { get; set; }
        public string? NombreProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal Subtotal => Cantidad * Precio;
    }
}
namespace SIMA_SOFTWARE.Models
{
    public class PedidoRequest
    {
        public int IdCliente { get; set; }
        public int IdPedido { get; set; } // para editar
        public List<DetalleRequest> Detalles { get; set; }
    }

    public class DetalleRequest
    {
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
    }
}

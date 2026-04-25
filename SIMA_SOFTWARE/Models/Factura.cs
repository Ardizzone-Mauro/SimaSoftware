using System.ComponentModel.DataAnnotations;



namespace SIMA_SOFTWARE.Models
{
    public class Factura
    {
        [Key]
        public int IdFactura { get; set; }
        public DateTime FechaEmision { get; set; }
        public decimal MontoTotal { get; set; }
        public decimal Impuestos { get; set; }

        public int IdCliente { get; set; }
        public int IdPedido { get; set; }

        public Cliente? Cliente { get; set; }
        public Pedido? Pedido { get; set; }
        public int Numero { get; set; }
        public string PuntoVenta { get; set; } = "0001";
        public string Tipo { get; set; } = "A";

        public ICollection<DetalleFactura>? DetallesFactura { get; set; }
    }
}

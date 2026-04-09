using System.ComponentModel.DataAnnotations;

namespace SIMA_SOFTWARE.Models
{
    public class Stock
    {
        [Key]
        public int IdStock { get; set; }
        public int Cantidad { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public decimal PrecioUnitario { get; set; }

        public int IdInventario { get; set; }
        public Inventario? Inventario { get; set; }
    }
}


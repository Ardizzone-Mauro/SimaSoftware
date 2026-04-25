using SIMA_SOFTWARE.Models;
using System.ComponentModel.DataAnnotations;

namespace SIMA_SOFTWARE.Models
{

    public class Inventario
    {
        [Key]
        public int IdInventario { get; set; }

        public int IdProducto { get; set; }
        public int IdDeposito { get; set; } // agrego el IdDeposito para relacionar con el depósito correspondiente

        public int Stock { get; set; }

        public Producto? Producto { get; set; }
        public Deposito? Deposito { get; set; }// agrego la propiedad de navegación para el depósito

        public ICollection<Stock>? Stocks { get; set; }
    }
}
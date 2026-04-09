using System.ComponentModel.DataAnnotations;

namespace SIMA_SOFTWARE.Models
{
    public class Inventario
    {
        [Key]
        public int IdInventario { get; set; }
        public int IdProducto { get; set; }
        public int Stock { get; set; }

        public Producto? Producto { get; set; }
        public ICollection<Stock>? Stocks { get; set; }
    }
}

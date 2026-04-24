using System.ComponentModel.DataAnnotations;

namespace SIMA_SOFTWARE.Models
{
    public class Deposito
    {
        [Key]
        public int IdDeposito { get; set; }
        public string Nombre { get; set; } = string.Empty;

        public ICollection<Inventario>? Inventarios { get; set; }
    }
}

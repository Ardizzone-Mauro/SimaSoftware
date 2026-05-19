using System.ComponentModel.DataAnnotations;

namespace SIMA_SOFTWARE.Models
{
    public class Barrio
    {
        [Key]
        public int IdBarrio { get; set; }

        public string Descripcion { get; set; } = string.Empty;

        public ICollection<Direccion>? Direcciones { get; set; }
    }
}

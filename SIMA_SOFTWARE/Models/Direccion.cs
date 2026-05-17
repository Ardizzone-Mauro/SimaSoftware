using System.ComponentModel.DataAnnotations;

namespace SIMA_SOFTWARE.Models
{

    public class Direccion
    {
        [Key]
        public int IdDireccion { get; set; }

        public int IdBarrio { get; set; }

        public string? Calle { get; set; }

        public string? NroPuerta { get; set; }

        public Barrio? Barrio { get; set; }

        public List<Cliente>? Clientes { get; set; }
    }
}

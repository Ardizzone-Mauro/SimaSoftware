using System.ComponentModel.DataAnnotations;

namespace SIMA_Software.Models
{

    public class Direccion
    {
        [Key]
        public int IdDireccion { get; set; }
        public string? Barrio { get; set; }
        public string? Calle { get; set; }
        public string? NroPuerta { get; set; }

        public ICollection<Cliente>? Clientes { get; set; }
    }
}

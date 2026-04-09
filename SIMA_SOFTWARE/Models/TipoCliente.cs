

using System.ComponentModel.DataAnnotations;

namespace SIMA_SOFTWARE.Models
{
    public class TipoCliente
    {
        [Key]
        public int IdTipoCliente { get; set; }
        public string? Categoria { get; set; }

        public List<Cliente>? Clientes { get; set; }
    }
}



using System.ComponentModel.DataAnnotations;

namespace SIMA_Software.Models
{
    public class TipoCliente
    {
        [Key]
        public int IdTipoCliente { get; set; }
        public string? Categoria { get; set; }

        public ICollection<Cliente>? Clientes { get; set; }
    }
}

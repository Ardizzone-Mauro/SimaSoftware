using System.ComponentModel.DataAnnotations;

namespace SIMA_Software.Models
{
    public class Estado
    {
        [Key]
        public int IdEstado { get; set; }
        public string? Descripcion { get; set; }

        public ICollection<Envio>? Envios { get; set; }
    }
}


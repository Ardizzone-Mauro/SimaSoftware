using Microsoft.AspNetCore.Identity;
using SIMA_Software.Models;
using System.ComponentModel.DataAnnotations;

namespace SIMA_SOFTWARE.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(50)]
        public string? Nombre { get; set; }

        [Required]
        [StringLength(50)]
        public string? Apellido { get; set; }
        public string? ImagenUrlPerfil { get; set; }
        //Vinculamos el usuario de acceso con la entidad Cliente del mapeo
        public int? IdCliente { get; set; }
        public virtual Cliente? ClienteUser { get; set; }
    }
}

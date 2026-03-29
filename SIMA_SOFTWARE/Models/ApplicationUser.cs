using Microsoft.AspNetCore.Identity;
using SIMA_Software.Models;
using System.ComponentModel.DataAnnotations;

namespace SIMA_SOFTWARE.Models
{
    public class ApplicationUser : IdentityUser
    {
        //Vinculamos el usuario de acceso con la entidad Cliente del mapeo
        public int? IdCliente { get; set; }
        public virtual Cliente? ClienteUser { get; set; }
    }
}

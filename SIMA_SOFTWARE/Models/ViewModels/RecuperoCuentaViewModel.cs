using System.ComponentModel.DataAnnotations;

namespace SIMA_SOFTWARE.Models.ViewModels
{
    public class RecuperoCuentaViewModel
    {
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Ingresa un email válido")]
        [Display(Name = "Correo electrónico")]
        public required string Email { get; set; }
    }
}


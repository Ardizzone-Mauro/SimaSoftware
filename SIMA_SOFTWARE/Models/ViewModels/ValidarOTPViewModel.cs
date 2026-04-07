namespace SIMA_SOFTWARE.Models.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class ValidarOTPViewModel
    {
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "El código es obligatorio")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "El código debe ser numérico")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "El código debe tener 6 dígitos")]
        public string Codigo { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [MinLength(6, ErrorMessage = "Mínimo 6 caracteres")]
        public string NuevaPassword { get; set; }

        [Required(ErrorMessage = "Confirmá la contraseña")]
        [Compare("NuevaPassword", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmarPassword { get; set; }
    }
}

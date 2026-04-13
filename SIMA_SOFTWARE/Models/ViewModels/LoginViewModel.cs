using System.ComponentModel;
using System.ComponentModel.DataAnnotations;



namespace SIMA_SOFTWARE.Models.ViewModels
{
    public class LoginViewModel
    {
        [EmailAddress(ErrorMessage = "Ingresa un email válido.")]
        [Required(ErrorMessage = "El email es obligatorio.")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "La clave es obligatoria")]
        public string Clave { get; set; }
        
        [DisplayName("Recordarme")]
        public bool Recordarme { get; set; }
    }
}

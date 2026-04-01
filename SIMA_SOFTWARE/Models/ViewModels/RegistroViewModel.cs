using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SIMA_SOFTWARE.Models.ViewModels
{
    public class RegistroViewModel
    {
        [Required(ErrorMessage = "El campo Nombre es obligatorio.")]
        [StringLength(50)]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El campo Apellido es obligatorio.")]
        [StringLength(50)]
        public string Apellido { get; set; }

        [EmailAddress(ErrorMessage = "Ingresa un email válido.")]
        [Required(ErrorMessage = "El email es obligatorio.")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "El campo Clave es obligatorio.")]
        public string Clave { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "El campo Confirmar Clave es obligatorio.")]
        [Compare("Clave", ErrorMessage = "Las claves no coinciden.")]
        public string ConfirmarClave { get; set; }
    }
}

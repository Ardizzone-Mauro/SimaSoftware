using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SIMA_SOFTWARE.Models.ViewModels
{
    public class ClienteViewModel
    {
        public int IdCliente { get; set; }

        [Required(ErrorMessage = "El campo Nombre es obligatorio.")]
        [StringLength(100)]
        [Display(Name = "Nombre Completo")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El Email es obligatorio.")]
        [EmailAddress]
        [StringLength(150)]
        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; }

        [StringLength(20)]
        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        public bool Activo { get; set; }

        // DIRECCION
        [Required(ErrorMessage = "El campo Calle es obligatorio.")]
        [StringLength(100)]
        public string Calle { get; set; }

        [Range(1, 99999)]
        [Display(Name = "Número")]
        public string NroPuerta { get; set; }

        [Required(ErrorMessage = "El campo Barrio es obligatorio.")]
        [StringLength(100)]
        public string Barrio { get; set; }

        // CUENTA
        [Range(0, 100000000)]
        public decimal Saldo { get; set; }

        [Required(ErrorMessage = "El campo Condiciones de Pago es obligatorio.")]
        [StringLength(200)]
        [Display(Name = "Condición de Pago")]
        public string CondicionesPago { get; set; }

        // TIPO CLIENTE
        [Required(ErrorMessage = "Debe seleccionar un tipo de cliente.")]
        [Display(Name = "Tipo de Cliente")]
        public int TipoClienteId { get; set; }

        public List<SelectListItem>? TiposCliente { get; set; }
    }
}

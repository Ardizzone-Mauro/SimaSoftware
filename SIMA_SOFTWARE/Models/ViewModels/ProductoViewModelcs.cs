using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace SIMA_SOFTWARE.Models.ViewModels
{
    public class ProductoViewModel
    {
        public int IdProducto { get; set; } // Solo para edición

        [Required(ErrorMessage = "El nombre del producto es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe ingresar un precio")]
        public decimal Precio { get; set; }
        public int IdDeposito { get; set; }//  asignar el depósito al crear un producto nuevo

        [Required(ErrorMessage = "Debe ingresar stock inicial")]
        public int StockInicial { get; set; }//  asignar el stock inicial al crear un producto nuevo
        public int StockTotal { get; set; }

        // Este campo permite gestionar la carga de archivos (HU 01)
        public IFormFile? Imagen { get; set; }
        public string? UrlImagenExistente { get; set; }

        [StringLength(500)]
        public string? Descripcion { get; set; } = null;
    }
}

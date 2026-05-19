using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace SIMA_SOFTWARE.Models.ViewModels
{
    public class ProductoViewModel
    {
        public int IdProducto { get; set; }

        // ===== PRODUCTO =====
        [Required(ErrorMessage = "El nombre del producto es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe ingresar un precio")]
        public decimal Precio { get; set; }

        [StringLength(500)]
        public string? Descripcion { get; set; }

        // ===== NUEVOS CAMPOS =====
        [Required]
        public string? Categoria { get; set; }   // LINEA RUECA / LINEA TELAR

        public string? Color { get; set; }

        public int? CantidadHebras { get; set; }

        // ===== STOCK =====
        public int IdDeposito { get; set; }

        [Required]
        public int StockInicial { get; set; }

        public int StockTotal { get; set; }

        // ===== IMAGEN =====
        public IFormFile? Imagen { get; set; }
        public string? UrlImagenExistente { get; set; }
    }
}

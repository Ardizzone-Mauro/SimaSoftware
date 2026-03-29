using SIMA_SOFTWARE.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIMA_Software.Models
{
    [Table("Clientes")]
    public class Cliente
    {

        [Key]
        public int IdCliente { get; set; }

        [Required]
        [StringLength(100)]
        public string? Nombre { get; set; }

        [EmailAddress]
        [Required]
        [StringLength(50)]
        public string? Email { get; set; }
        
        [StringLength(20)]
        [Phone]
        public string? Telefono { get; set; }

        public int IdDireccion { get; set; }
        public int IdTipoCliente { get; set; }
        public int IdCuenta { get; set; }

        public Direccion? Direccion { get; set; }
        public TipoCliente? TipoCliente { get; set; }
        public Cuenta? Cuenta { get; set; }

        public List<Pedido>? Pedidos { get; set; }
        public List<Factura>? Facturas { get; set; }

        // Propiedad de navegación hacia Identity
        public string? ApplicationUserId { get; set; }
        public virtual ApplicationUser? User { get; set; }



    }
}


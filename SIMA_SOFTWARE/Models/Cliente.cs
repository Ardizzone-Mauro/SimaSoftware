using SIMA_SOFTWARE.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SIMA_SOFTWARE.Models
{
    [Table("Clientes")]
    [Index(nameof(Email), IsUnique = true)]
    public class Cliente
    {
        [Key]
        public int IdCliente { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(50)]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        [Phone]
        public string? Telefono { get; set; }

        public int DireccionId { get; set; }
        public int TipoClienteId { get; set; }
        public int CuentaId { get; set; }

        public Direccion? Direccion { get; set; }
        public TipoCliente? TipoCliente { get; set; }
        public Cuenta? Cuenta { get; set; }

        public List<Pedido>? Pedidos { get; set; }
        public List<Factura>? Facturas { get; set; }

        // Identity
        public string? ApplicationUserId { get; set; }
        public ApplicationUser? User { get; set; }
        

        //  BAJA LÓGICA
        public bool Activo { get; set; } = true;
    }
}


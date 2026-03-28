using System.ComponentModel.DataAnnotations;

namespace SIMA_Software.Models
{
    public class Cuenta
    {
        [Key]
        public int IdCuenta { get; set; }
        public decimal Saldo { get; set; }
        public string? CondicionesPago { get; set; }

        public ICollection<Cliente>? Clientes { get; set; }
    }
}

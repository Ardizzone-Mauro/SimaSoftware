using System.ComponentModel.DataAnnotations;

namespace SIMA_SOFTWARE.Models
{
    public class Cuenta
    {
        [Key]
        public int IdCuenta { get; set; }
        public decimal Saldo { get; set; }
        public string? CondicionesPago { get; set; }

        public List<Cliente>? Clientes { get; set; }
    }
}

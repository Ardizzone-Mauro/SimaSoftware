using SIMA_SOFTWARE.Models;

namespace SIMA_SOFTWARE.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalClientes { get; set; }
        public int PedidosActivos { get; set; }
        public int PedidosListosEnvio { get; set; }

        public int StockTotal { get; set; }
        public int StockBajo { get; set; }

        public decimal IngresosMensuales { get; set; }
        public double VariacionIngresos { get; set; }

        public List<PedidoViewModel> PedidosRecientes { get; set; } = new();

        public List<Inventario> ProductosStockBajo { get; set; }
                = new List<Inventario>();
    }
}

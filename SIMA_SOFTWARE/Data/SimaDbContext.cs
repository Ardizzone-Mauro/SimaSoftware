using Microsoft.EntityFrameworkCore;
using SIMA_Software.Models;

namespace SIMA_SOFTWARE.Data
{
    public class SimaDbContext : DbContext
    {
        public SimaDbContext(DbContextOptions<SimaDbContext> options) : base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<TipoCliente> TipoClientes { get; set; }
        public DbSet<Direccion> Direcciones { get; set; }
        public DbSet<Cuenta> Cuentas { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Inventario> Inventarios { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<PedidoProducto> PedidoProductos { get; set; }
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<DetalleFactura> DetalleFacturas { get; set; }
        public DbSet<DetalleProducto> DetalleProductos { get; set; }
        public DbSet<Estado> Estados { get; set; }
        public DbSet<Envio> Envios { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar precisión de propiedades decimal
            modelBuilder.Entity<Cuenta>()
                .Property(c => c.Saldo)
                .HasPrecision(18, 2);

            modelBuilder.Entity<DetalleProducto>()
                .Property(d => d.PrecioUnitario)
                .HasPrecision(18, 2);
            modelBuilder.Entity<DetalleProducto>()
                .Property(d => d.Descuento)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Factura>()
                .Property(f => f.Impuestos)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Factura>()
                .Property(f => f.MontoTotal)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PedidoProducto>()
                .Property(p => p.PrecioUnitario)
                .HasPrecision(18, 2);
            modelBuilder.Entity<PedidoProducto>()
                .Property(p => p.Descuento)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Producto>()
                .Property(p => p.Precio)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Stock>()
                .Property(s => s.PrecioUnitario)
                .HasPrecision(18, 2);

            // Aquí puedes agregar más configuraciones específicas si lo necesitas
        }
    }
}


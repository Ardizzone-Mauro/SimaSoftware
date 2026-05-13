using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SIMA_SOFTWARE.Models;

namespace SIMA_SOFTWARE.Data
{
    public class SimaDbContext : IdentityDbContext<ApplicationUser>
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
        public DbSet<Deposito> Depositos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<PedidoProducto> PedidoProductos { get; set; }
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<DetalleFactura> DetalleFacturas { get; set; }
        public DbSet<DetalleProducto> DetalleProductos { get; set; }
        public DbSet<Estado> Estados { get; set; }
        public DbSet<Envio> Envios { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }
        public DbSet<CodigoRecuperacion> CodigosRecuperacion { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // PEDIDO → CLIENTE (Muchos a 1)

            modelBuilder.Entity<Pedido>()
               .HasOne(p => p.Cliente)
               .WithMany(c => c.Pedidos)
               .HasForeignKey(p => p.IdCliente)
               .OnDelete(DeleteBehavior.Restrict); //  evita borrado en cascada


            // PEDIDO → PEDIDO PRODUCTO (1 a muchos)

            modelBuilder.Entity<PedidoProducto>()
               .HasOne(pp => pp.Pedido)
               .WithMany(p => p.PedidoProductos)
               .HasForeignKey(pp => pp.IdPedido)
               .OnDelete(DeleteBehavior.Cascade); // opcional

            // BAJA LÓGICA GLOBAL (Pedido)

            modelBuilder.Entity<Pedido>()
                .HasQueryFilter(p => p.Activo); //  SOLO trae activos por defecto

            // BAJA LÓGICA GLOBAL (Producto)

            modelBuilder.Entity<Producto>()
                .HasQueryFilter(p => !p.Eliminado);


            // INVENTARIO → PRODUCTO (Muchos a 1)
            modelBuilder.Entity<Inventario>()
                .HasOne(i => i.Producto)
                .WithMany(p => p.Inventarios)
                .HasForeignKey(i => i.IdProducto)
                .OnDelete(DeleteBehavior.Cascade);


            // INVENTARIO → DEPOSITO (Muchos a 1)
            modelBuilder.Entity<Inventario>()
                .HasOne(i => i.Deposito)
                .WithMany(d => d.Inventarios)
                .HasForeignKey(i => i.IdDeposito)
                .OnDelete(DeleteBehavior.Restrict);


            // STOCK → INVENTARIO (Muchos a 1)
            modelBuilder.Entity<Stock>()
                .HasOne(s => s.Inventario)
                .WithMany(i => i.Stocks)
                .HasForeignKey(s => s.IdInventario)
                .OnDelete(DeleteBehavior.Cascade);


            //............................................................

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
            modelBuilder.Entity<DetalleProducto>()
                .HasOne(dp => dp.Producto)
                .WithMany()
                .HasForeignKey(dp => dp.IdProducto)
                .OnDelete(DeleteBehavior.Restrict);

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
            modelBuilder.Entity<Factura>()
                .HasOne(f => f.Cliente)
                .WithMany()
                .HasForeignKey(f => f.IdCliente)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Factura>()
                .HasOne(f => f.Pedido)
                .WithMany()
                .HasForeignKey(f => f.IdPedido)
                .OnDelete(DeleteBehavior.Restrict);
    

            //  EVITA DUPLICADOS DE PRODUCTO EN EL MISMO DEPÓSITO
            modelBuilder.Entity<Inventario>()
                .HasIndex(i => new { i.IdProducto, i.IdDeposito })
                .IsUnique();
            // Aquí puedes agregar más configuraciones específicas si lo necesitas

            // PEDIDO → ENVIO (1 a muchos)
            // ENVIO -> PEDIDO
            modelBuilder.Entity<Envio>()
                .HasOne(e => e.Pedido)
                .WithMany(p => p.Envios)
                .HasForeignKey(e => e.IdPedido)
                .OnDelete(DeleteBehavior.Cascade);

            // ENVIO -> ESTADO
            modelBuilder.Entity<Envio>()
                .HasOne(e => e.Estado)
                .WithMany()
                .HasForeignKey(e => e.IdEstado);

        }
    }
}


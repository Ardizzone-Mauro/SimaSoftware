using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIMA_SOFTWARE.Migrations
{
    /// <inheritdoc />
    public partial class InicialNueva : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Facturas_Pedidos_PedidoIdPedido",
                table: "Facturas");

            migrationBuilder.DropForeignKey(
                name: "FK_Inventarios_Productos_ProductoIdProducto",
                table: "Inventarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Stocks_Inventarios_InventarioIdInventario",
                table: "Stocks");

            migrationBuilder.DropIndex(
                name: "IX_Stocks_InventarioIdInventario",
                table: "Stocks");

            migrationBuilder.DropIndex(
                name: "IX_Inventarios_ProductoIdProducto",
                table: "Inventarios");

            migrationBuilder.DropIndex(
                name: "IX_Facturas_PedidoIdPedido",
                table: "Facturas");

            migrationBuilder.DropColumn(
                name: "InventarioIdInventario",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "ProductoIdProducto",
                table: "Inventarios");

            migrationBuilder.DropColumn(
                name: "PedidoIdPedido",
                table: "Facturas");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Productos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "IdDeposito",
                table: "Inventarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Numero",
                table: "Facturas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PuntoVenta",
                table: "Facturas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tipo",
                table: "Facturas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Depositos",
                columns: table => new
                {
                    IdDeposito = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Depositos", x => x.IdDeposito);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_IdInventario",
                table: "Stocks",
                column: "IdInventario");

            migrationBuilder.CreateIndex(
                name: "IX_Inventarios_IdDeposito",
                table: "Inventarios",
                column: "IdDeposito");

            migrationBuilder.CreateIndex(
                name: "IX_Inventarios_IdProducto_IdDeposito",
                table: "Inventarios",
                columns: new[] { "IdProducto", "IdDeposito" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Facturas_IdCliente",
                table: "Facturas",
                column: "IdCliente");

            migrationBuilder.CreateIndex(
                name: "IX_Facturas_IdPedido",
                table: "Facturas",
                column: "IdPedido");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleProductos_IdProducto",
                table: "DetalleProductos",
                column: "IdProducto");

            migrationBuilder.AddForeignKey(
                name: "FK_DetalleProductos_Productos_IdProducto",
                table: "DetalleProductos",
                column: "IdProducto",
                principalTable: "Productos",
                principalColumn: "IdProducto",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Facturas_Clientes_IdCliente",
                table: "Facturas",
                column: "IdCliente",
                principalTable: "Clientes",
                principalColumn: "IdCliente",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Facturas_Pedidos_IdPedido",
                table: "Facturas",
                column: "IdPedido",
                principalTable: "Pedidos",
                principalColumn: "IdPedido",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Inventarios_Depositos_IdDeposito",
                table: "Inventarios",
                column: "IdDeposito",
                principalTable: "Depositos",
                principalColumn: "IdDeposito",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Inventarios_Productos_IdProducto",
                table: "Inventarios",
                column: "IdProducto",
                principalTable: "Productos",
                principalColumn: "IdProducto",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Stocks_Inventarios_IdInventario",
                table: "Stocks",
                column: "IdInventario",
                principalTable: "Inventarios",
                principalColumn: "IdInventario",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetalleProductos_Productos_IdProducto",
                table: "DetalleProductos");

            migrationBuilder.DropForeignKey(
                name: "FK_Facturas_Clientes_IdCliente",
                table: "Facturas");

            migrationBuilder.DropForeignKey(
                name: "FK_Facturas_Pedidos_IdPedido",
                table: "Facturas");

            migrationBuilder.DropForeignKey(
                name: "FK_Inventarios_Depositos_IdDeposito",
                table: "Inventarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Inventarios_Productos_IdProducto",
                table: "Inventarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Stocks_Inventarios_IdInventario",
                table: "Stocks");

            migrationBuilder.DropTable(
                name: "Depositos");

            migrationBuilder.DropIndex(
                name: "IX_Stocks_IdInventario",
                table: "Stocks");

            migrationBuilder.DropIndex(
                name: "IX_Inventarios_IdDeposito",
                table: "Inventarios");

            migrationBuilder.DropIndex(
                name: "IX_Inventarios_IdProducto_IdDeposito",
                table: "Inventarios");

            migrationBuilder.DropIndex(
                name: "IX_Facturas_IdCliente",
                table: "Facturas");

            migrationBuilder.DropIndex(
                name: "IX_Facturas_IdPedido",
                table: "Facturas");

            migrationBuilder.DropIndex(
                name: "IX_DetalleProductos_IdProducto",
                table: "DetalleProductos");

            migrationBuilder.DropColumn(
                name: "IdDeposito",
                table: "Inventarios");

            migrationBuilder.DropColumn(
                name: "Numero",
                table: "Facturas");

            migrationBuilder.DropColumn(
                name: "PuntoVenta",
                table: "Facturas");

            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "Facturas");

            migrationBuilder.AddColumn<int>(
                name: "InventarioIdInventario",
                table: "Stocks",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Productos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductoIdProducto",
                table: "Inventarios",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PedidoIdPedido",
                table: "Facturas",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_InventarioIdInventario",
                table: "Stocks",
                column: "InventarioIdInventario");

            migrationBuilder.CreateIndex(
                name: "IX_Inventarios_ProductoIdProducto",
                table: "Inventarios",
                column: "ProductoIdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_Facturas_PedidoIdPedido",
                table: "Facturas",
                column: "PedidoIdPedido");

            migrationBuilder.AddForeignKey(
                name: "FK_Facturas_Pedidos_PedidoIdPedido",
                table: "Facturas",
                column: "PedidoIdPedido",
                principalTable: "Pedidos",
                principalColumn: "IdPedido");

            migrationBuilder.AddForeignKey(
                name: "FK_Inventarios_Productos_ProductoIdProducto",
                table: "Inventarios",
                column: "ProductoIdProducto",
                principalTable: "Productos",
                principalColumn: "IdProducto");

            migrationBuilder.AddForeignKey(
                name: "FK_Stocks_Inventarios_InventarioIdInventario",
                table: "Stocks",
                column: "InventarioIdInventario",
                principalTable: "Inventarios",
                principalColumn: "IdInventario");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIMA_SOFTWARE.Migrations
{
    /// <inheritdoc />
    public partial class AgregarActivoYEstado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PedidoProductos_Pedidos_PedidoIdPedido",
                table: "PedidoProductos");

            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_Clientes_ClienteIdCliente",
                table: "Pedidos");

            migrationBuilder.DropIndex(
                name: "IX_Pedidos_ClienteIdCliente",
                table: "Pedidos");

            migrationBuilder.DropIndex(
                name: "IX_PedidoProductos_PedidoIdPedido",
                table: "PedidoProductos");

            migrationBuilder.DropColumn(
                name: "ClienteIdCliente",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "PedidoIdPedido",
                table: "PedidoProductos");

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Pedidos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Pedidos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_IdCliente",
                table: "Pedidos",
                column: "IdCliente");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoProductos_IdPedido",
                table: "PedidoProductos",
                column: "IdPedido");

            migrationBuilder.AddForeignKey(
                name: "FK_PedidoProductos_Pedidos_IdPedido",
                table: "PedidoProductos",
                column: "IdPedido",
                principalTable: "Pedidos",
                principalColumn: "IdPedido",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_Clientes_IdCliente",
                table: "Pedidos",
                column: "IdCliente",
                principalTable: "Clientes",
                principalColumn: "IdCliente",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PedidoProductos_Pedidos_IdPedido",
                table: "PedidoProductos");

            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_Clientes_IdCliente",
                table: "Pedidos");

            migrationBuilder.DropIndex(
                name: "IX_Pedidos_IdCliente",
                table: "Pedidos");

            migrationBuilder.DropIndex(
                name: "IX_PedidoProductos_IdPedido",
                table: "PedidoProductos");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Pedidos");

            migrationBuilder.AddColumn<int>(
                name: "ClienteIdCliente",
                table: "Pedidos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PedidoIdPedido",
                table: "PedidoProductos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_ClienteIdCliente",
                table: "Pedidos",
                column: "ClienteIdCliente");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoProductos_PedidoIdPedido",
                table: "PedidoProductos",
                column: "PedidoIdPedido");

            migrationBuilder.AddForeignKey(
                name: "FK_PedidoProductos_Pedidos_PedidoIdPedido",
                table: "PedidoProductos",
                column: "PedidoIdPedido",
                principalTable: "Pedidos",
                principalColumn: "IdPedido");

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_Clientes_ClienteIdCliente",
                table: "Pedidos",
                column: "ClienteIdCliente",
                principalTable: "Clientes",
                principalColumn: "IdCliente");
        }
    }
}

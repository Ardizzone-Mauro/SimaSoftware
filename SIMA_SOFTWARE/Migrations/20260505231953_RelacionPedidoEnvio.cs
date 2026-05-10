using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIMA_SOFTWARE.Migrations
{
    /// <inheritdoc />
    public partial class RelacionPedidoEnvio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Envios_Pedidos_PedidoIdPedido",
                table: "Envios");

            migrationBuilder.DropIndex(
                name: "IX_Envios_PedidoIdPedido",
                table: "Envios");

            migrationBuilder.DropColumn(
                name: "PedidoIdPedido",
                table: "Envios");

            migrationBuilder.AlterColumn<int>(
                name: "IdEstado",
                table: "Envios",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Envios_IdPedido",
                table: "Envios",
                column: "IdPedido");

            migrationBuilder.AddForeignKey(
                name: "FK_Envios_Pedidos_IdPedido",
                table: "Envios",
                column: "IdPedido",
                principalTable: "Pedidos",
                principalColumn: "IdPedido",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Envios_Pedidos_IdPedido",
                table: "Envios");

            migrationBuilder.DropIndex(
                name: "IX_Envios_IdPedido",
                table: "Envios");

            migrationBuilder.AlterColumn<int>(
                name: "IdEstado",
                table: "Envios",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PedidoIdPedido",
                table: "Envios",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Envios_PedidoIdPedido",
                table: "Envios",
                column: "PedidoIdPedido");

            migrationBuilder.AddForeignKey(
                name: "FK_Envios_Pedidos_PedidoIdPedido",
                table: "Envios",
                column: "PedidoIdPedido",
                principalTable: "Pedidos",
                principalColumn: "IdPedido");
        }
    }
}

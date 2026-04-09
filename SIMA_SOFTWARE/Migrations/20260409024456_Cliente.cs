using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIMA_SOFTWARE.Migrations
{
    /// <inheritdoc />
    public partial class Cliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_Cuentas_CuentaIdCuenta",
                table: "Clientes");

            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_Direcciones_DireccionIdDireccion",
                table: "Clientes");

            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_TipoClientes_TipoClienteIdTipoCliente",
                table: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_CuentaIdCuenta",
                table: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_DireccionIdDireccion",
                table: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_TipoClienteIdTipoCliente",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "CuentaIdCuenta",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "DireccionIdDireccion",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "TipoClienteIdTipoCliente",
                table: "Clientes");

            migrationBuilder.RenameColumn(
                name: "IdTipoCliente",
                table: "Clientes",
                newName: "TipoClienteId");

            migrationBuilder.RenameColumn(
                name: "IdDireccion",
                table: "Clientes",
                newName: "DireccionId");

            migrationBuilder.RenameColumn(
                name: "IdCuenta",
                table: "Clientes",
                newName: "CuentaId");

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Clientes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_CuentaId",
                table: "Clientes",
                column: "CuentaId");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_DireccionId",
                table: "Clientes",
                column: "DireccionId");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Email",
                table: "Clientes",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_TipoClienteId",
                table: "Clientes",
                column: "TipoClienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_Cuentas_CuentaId",
                table: "Clientes",
                column: "CuentaId",
                principalTable: "Cuentas",
                principalColumn: "IdCuenta",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_Direcciones_DireccionId",
                table: "Clientes",
                column: "DireccionId",
                principalTable: "Direcciones",
                principalColumn: "IdDireccion",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_TipoClientes_TipoClienteId",
                table: "Clientes",
                column: "TipoClienteId",
                principalTable: "TipoClientes",
                principalColumn: "IdTipoCliente",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_Cuentas_CuentaId",
                table: "Clientes");

            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_Direcciones_DireccionId",
                table: "Clientes");

            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_TipoClientes_TipoClienteId",
                table: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_CuentaId",
                table: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_DireccionId",
                table: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_Email",
                table: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_TipoClienteId",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Clientes");

            migrationBuilder.RenameColumn(
                name: "TipoClienteId",
                table: "Clientes",
                newName: "IdTipoCliente");

            migrationBuilder.RenameColumn(
                name: "DireccionId",
                table: "Clientes",
                newName: "IdDireccion");

            migrationBuilder.RenameColumn(
                name: "CuentaId",
                table: "Clientes",
                newName: "IdCuenta");

            migrationBuilder.AddColumn<int>(
                name: "CuentaIdCuenta",
                table: "Clientes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DireccionIdDireccion",
                table: "Clientes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TipoClienteIdTipoCliente",
                table: "Clientes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_CuentaIdCuenta",
                table: "Clientes",
                column: "CuentaIdCuenta");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_DireccionIdDireccion",
                table: "Clientes",
                column: "DireccionIdDireccion");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_TipoClienteIdTipoCliente",
                table: "Clientes",
                column: "TipoClienteIdTipoCliente");

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_Cuentas_CuentaIdCuenta",
                table: "Clientes",
                column: "CuentaIdCuenta",
                principalTable: "Cuentas",
                principalColumn: "IdCuenta");

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_Direcciones_DireccionIdDireccion",
                table: "Clientes",
                column: "DireccionIdDireccion",
                principalTable: "Direcciones",
                principalColumn: "IdDireccion");

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_TipoClientes_TipoClienteIdTipoCliente",
                table: "Clientes",
                column: "TipoClienteIdTipoCliente",
                principalTable: "TipoClientes",
                principalColumn: "IdTipoCliente");
        }
    }
}

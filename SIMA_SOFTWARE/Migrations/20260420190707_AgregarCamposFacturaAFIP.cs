using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIMA_SOFTWARE.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCamposFacturaAFIP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<string>(
                name: "Cuit",
                table: "Clientes",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Numero",
                table: "Facturas");

            migrationBuilder.DropColumn(
                name: "PuntoVenta",
                table: "Facturas");

            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "Facturas");

            migrationBuilder.DropColumn(
                name: "Cuit",
                table: "Clientes");
        }
    }
}

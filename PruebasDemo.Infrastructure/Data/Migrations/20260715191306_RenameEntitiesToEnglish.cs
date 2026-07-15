using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PruebasDemo.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameEntitiesToEnglish : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TasaInteres",
                table: "Creditos",
                newName: "InterestRate");

            migrationBuilder.RenameColumn(
                name: "Saldo",
                table: "Creditos",
                newName: "Balance");

            migrationBuilder.RenameColumn(
                name: "Monto",
                table: "Creditos",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "Meses",
                table: "Creditos",
                newName: "Months");

            migrationBuilder.RenameColumn(
                name: "Estado",
                table: "Creditos",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "FechaCreacion",
                table: "Creditos",
                newName: "CreationDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Months",
                table: "Creditos",
                newName: "Meses");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Creditos",
                newName: "Estado");

            migrationBuilder.RenameColumn(
                name: "InterestRate",
                table: "Creditos",
                newName: "TasaInteres");

            migrationBuilder.RenameColumn(
                name: "CreationDate",
                table: "Creditos",
                newName: "FechaCreacion");

            migrationBuilder.RenameColumn(
                name: "Balance",
                table: "Creditos",
                newName: "Saldo");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "Creditos",
                newName: "Monto");
        }
    }
}

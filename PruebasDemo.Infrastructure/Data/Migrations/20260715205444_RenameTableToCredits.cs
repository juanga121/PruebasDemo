using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PruebasDemo.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameTableToCredits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Creditos",
                table: "Creditos");

            migrationBuilder.RenameTable(
                name: "Creditos",
                newName: "Credits");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Credits",
                table: "Credits",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Credits",
                table: "Credits");

            migrationBuilder.RenameTable(
                name: "Credits",
                newName: "Creditos");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Creditos",
                table: "Creditos",
                column: "Id");
        }
    }
}

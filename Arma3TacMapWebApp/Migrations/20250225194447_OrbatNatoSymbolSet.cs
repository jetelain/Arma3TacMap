using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arma3TacMapWebApp.Migrations
{
    /// <inheritdoc />
    public partial class OrbatNatoSymbolSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NatoSymbolSet",
                table: "OrbatUnit",
                type: "TEXT",
                nullable: true,
                defaultValue: "10");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NatoSymbolSet",
                table: "OrbatUnit");
        }
    }
}

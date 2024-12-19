using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arma3TacMapWebApp.Migrations
{
    /// <inheritdoc />
    public partial class PhasesAndOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "TacMap",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Phase",
                table: "TacMap",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "TacMap");

            migrationBuilder.DropColumn(
                name: "Phase",
                table: "TacMap");
        }
    }
}

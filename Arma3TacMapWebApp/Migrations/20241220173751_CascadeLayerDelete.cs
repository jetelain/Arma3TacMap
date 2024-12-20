using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arma3TacMapWebApp.Migrations
{
    /// <inheritdoc />
    public partial class CascadeLayerDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TacMap_TacMap_ParentTacMapID",
                table: "TacMap");

            migrationBuilder.AddForeignKey(
                name: "FK_TacMap_TacMap_ParentTacMapID",
                table: "TacMap",
                column: "ParentTacMapID",
                principalTable: "TacMap",
                principalColumn: "TacMapID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TacMap_TacMap_ParentTacMapID",
                table: "TacMap");

            migrationBuilder.AddForeignKey(
                name: "FK_TacMap_TacMap_ParentTacMapID",
                table: "TacMap",
                column: "ParentTacMapID",
                principalTable: "TacMap",
                principalColumn: "TacMapID");
        }
    }
}

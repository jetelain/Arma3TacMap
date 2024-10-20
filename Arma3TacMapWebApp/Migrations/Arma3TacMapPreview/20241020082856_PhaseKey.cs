using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arma3TacMapWebApp.Migrations.Arma3TacMapPreview
{
    /// <inheritdoc />
    public partial class PhaseKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TacMapPreview",
                table: "TacMapPreview");

            migrationBuilder.AlterColumn<byte[]>(
                name: "Data",
                table: "TacMapPreview",
                type: "BLOB",
                nullable: false,
                defaultValue: new byte[0],
                oldClrType: typeof(byte[]),
                oldType: "BLOB",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhaseKey",
                table: "TacMapPreview",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TacMapPreview",
                table: "TacMapPreview",
                columns: new[] { "TacMapID", "Size", "PhaseKey" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TacMapPreview",
                table: "TacMapPreview");

            migrationBuilder.DropColumn(
                name: "PhaseKey",
                table: "TacMapPreview");

            migrationBuilder.AlterColumn<byte[]>(
                name: "Data",
                table: "TacMapPreview",
                type: "BLOB",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "BLOB");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TacMapPreview",
                table: "TacMapPreview",
                columns: new[] { "TacMapID", "Size" });
        }
    }
}

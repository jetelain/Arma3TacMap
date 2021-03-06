using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Arma3TacMapWebApp.Migrations.Arma3TacMapPreview
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TacMapPreview",
                columns: table => new
                {
                    TacMapID = table.Column<int>(type: "INTEGER", nullable: false),
                    Size = table.Column<int>(type: "INTEGER", nullable: false),
                    Data = table.Column<byte[]>(type: "BLOB", nullable: true),
                    LastUpdate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TacMapPreview", x => new { x.TacMapID, x.Size });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TacMapPreview");
        }
    }
}

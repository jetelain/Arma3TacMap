using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Arma3TacMapWebApp.Migrations
{
    public partial class PreviewAndApi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsService",
                table: "User",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdate",
                table: "TacMapMarker",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventHref",
                table: "TacMap",
                type: "TEXT",
                nullable: true);

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
                    table.ForeignKey(
                        name: "FK_TacMapPreview_TacMap_TacMapID",
                        column: x => x.TacMapID,
                        principalTable: "TacMap",
                        principalColumn: "TacMapID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserApiKey",
                columns: table => new
                {
                    UserApiKeyID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HashedKey = table.Column<string>(type: "TEXT", nullable: true),
                    Salt = table.Column<string>(type: "TEXT", nullable: true),
                    ValidUntil = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UserID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserApiKey", x => x.UserApiKeyID);
                    table.ForeignKey(
                        name: "FK_UserApiKey_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserApiKey_UserID",
                table: "UserApiKey",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TacMapPreview");

            migrationBuilder.DropTable(
                name: "UserApiKey");

            migrationBuilder.DropColumn(
                name: "IsService",
                table: "User");

            migrationBuilder.DropColumn(
                name: "LastUpdate",
                table: "TacMapMarker");

            migrationBuilder.DropColumn(
                name: "EventHref",
                table: "TacMap");
        }
    }
}

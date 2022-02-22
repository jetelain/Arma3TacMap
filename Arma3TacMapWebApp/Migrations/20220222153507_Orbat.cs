using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Arma3TacMapWebApp.Migrations
{
    public partial class Orbat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FriendlyOrbatID",
                table: "TacMap",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HostileOrbatID",
                table: "TacMap",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Orbat",
                columns: table => new
                {
                    OrbatID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OwnerUserID = table.Column<int>(type: "INTEGER", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Label = table.Column<string>(type: "TEXT", nullable: false),
                    Visibility = table.Column<int>(type: "INTEGER", nullable: false),
                    Token = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orbat", x => x.OrbatID);
                    table.ForeignKey(
                        name: "FK_Orbat_User_OwnerUserID",
                        column: x => x.OwnerUserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrbatUnit",
                columns: table => new
                {
                    OrbatUnitID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrbatID = table.Column<int>(type: "INTEGER", nullable: false),
                    ParentOrbatUnitID = table.Column<int>(type: "INTEGER", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    UniqueDesignation = table.Column<string>(type: "TEXT", nullable: true),
                    NatoSymbolIcon = table.Column<string>(type: "TEXT", nullable: true),
                    NatoSymbolMod1 = table.Column<string>(type: "TEXT", nullable: true),
                    NatoSymbolMod2 = table.Column<string>(type: "TEXT", nullable: true),
                    NatoSymbolSize = table.Column<string>(type: "TEXT", nullable: true),
                    NatoSymbolHQ = table.Column<string>(type: "TEXT", nullable: true),
                    NatoSymbolFriendlyImageBase64 = table.Column<string>(type: "TEXT", nullable: true),
                    NatoSymbolHostileImageBase64 = table.Column<string>(type: "TEXT", nullable: true),
                    NatoSymbolHostileAssumedImageBase64 = table.Column<string>(type: "TEXT", nullable: true),
                    Position = table.Column<int>(type: "INTEGER", nullable: false),
                    Trigram = table.Column<string>(type: "TEXT", maxLength: 3, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrbatUnit", x => x.OrbatUnitID);
                    table.ForeignKey(
                        name: "FK_OrbatUnit_Orbat_OrbatID",
                        column: x => x.OrbatID,
                        principalTable: "Orbat",
                        principalColumn: "OrbatID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrbatUnit_OrbatUnit_ParentOrbatUnitID",
                        column: x => x.ParentOrbatUnitID,
                        principalTable: "OrbatUnit",
                        principalColumn: "OrbatUnitID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TacMap_FriendlyOrbatID",
                table: "TacMap",
                column: "FriendlyOrbatID");

            migrationBuilder.CreateIndex(
                name: "IX_TacMap_HostileOrbatID",
                table: "TacMap",
                column: "HostileOrbatID");

            migrationBuilder.CreateIndex(
                name: "IX_Orbat_OwnerUserID",
                table: "Orbat",
                column: "OwnerUserID");

            migrationBuilder.CreateIndex(
                name: "IX_OrbatUnit_OrbatID",
                table: "OrbatUnit",
                column: "OrbatID");

            migrationBuilder.CreateIndex(
                name: "IX_OrbatUnit_ParentOrbatUnitID",
                table: "OrbatUnit",
                column: "ParentOrbatUnitID");

            migrationBuilder.AddForeignKey(
                name: "FK_TacMap_Orbat_FriendlyOrbatID",
                table: "TacMap",
                column: "FriendlyOrbatID",
                principalTable: "Orbat",
                principalColumn: "OrbatID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TacMap_Orbat_HostileOrbatID",
                table: "TacMap",
                column: "HostileOrbatID",
                principalTable: "Orbat",
                principalColumn: "OrbatID",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TacMap_Orbat_FriendlyOrbatID",
                table: "TacMap");

            migrationBuilder.DropForeignKey(
                name: "FK_TacMap_Orbat_HostileOrbatID",
                table: "TacMap");

            migrationBuilder.DropTable(
                name: "OrbatUnit");

            migrationBuilder.DropTable(
                name: "Orbat");

            migrationBuilder.DropIndex(
                name: "IX_TacMap_FriendlyOrbatID",
                table: "TacMap");

            migrationBuilder.DropIndex(
                name: "IX_TacMap_HostileOrbatID",
                table: "TacMap");

            migrationBuilder.DropColumn(
                name: "FriendlyOrbatID",
                table: "TacMap");

            migrationBuilder.DropColumn(
                name: "HostileOrbatID",
                table: "TacMap");
        }
    }
}

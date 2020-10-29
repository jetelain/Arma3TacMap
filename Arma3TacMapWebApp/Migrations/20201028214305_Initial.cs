using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Arma3TacMapWebApp.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserLabel = table.Column<string>(nullable: true),
                    SteamId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "TacMap",
                columns: table => new
                {
                    TacMapID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OwnerUserID = table.Column<int>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Label = table.Column<string>(nullable: false),
                    ReadOnlyToken = table.Column<string>(nullable: true),
                    ReadWriteToken = table.Column<string>(nullable: true),
                    WorldName = table.Column<string>(nullable: false),
                    ParentTacMapID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TacMap", x => x.TacMapID);
                    table.ForeignKey(
                        name: "FK_TacMap_User_OwnerUserID",
                        column: x => x.OwnerUserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TacMap_TacMap_ParentTacMapID",
                        column: x => x.ParentTacMapID,
                        principalTable: "TacMap",
                        principalColumn: "TacMapID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TacMapAccess",
                columns: table => new
                {
                    TacMapAccessID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TacMapID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    CanWrite = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TacMapAccess", x => x.TacMapAccessID);
                    table.ForeignKey(
                        name: "FK_TacMapAccess_TacMap_TacMapID",
                        column: x => x.TacMapID,
                        principalTable: "TacMap",
                        principalColumn: "TacMapID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TacMapAccess_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TacMapMarker",
                columns: table => new
                {
                    TacMapMarkerID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TacMapID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: true),
                    MarkerData = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TacMapMarker", x => x.TacMapMarkerID);
                    table.ForeignKey(
                        name: "FK_TacMapMarker_TacMap_TacMapID",
                        column: x => x.TacMapID,
                        principalTable: "TacMap",
                        principalColumn: "TacMapID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TacMapMarker_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TacMap_OwnerUserID",
                table: "TacMap",
                column: "OwnerUserID");

            migrationBuilder.CreateIndex(
                name: "IX_TacMap_ParentTacMapID",
                table: "TacMap",
                column: "ParentTacMapID");

            migrationBuilder.CreateIndex(
                name: "IX_TacMapAccess_TacMapID",
                table: "TacMapAccess",
                column: "TacMapID");

            migrationBuilder.CreateIndex(
                name: "IX_TacMapAccess_UserID",
                table: "TacMapAccess",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_TacMapMarker_TacMapID",
                table: "TacMapMarker",
                column: "TacMapID");

            migrationBuilder.CreateIndex(
                name: "IX_TacMapMarker_UserID",
                table: "TacMapMarker",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TacMapAccess");

            migrationBuilder.DropTable(
                name: "TacMapMarker");

            migrationBuilder.DropTable(
                name: "TacMap");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}

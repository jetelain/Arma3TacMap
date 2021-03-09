using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Arma3TacMapWebApp.Migrations
{
    public partial class Replay : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReplayMap",
                columns: table => new
                {
                    ReplayMapID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OwnerUserID = table.Column<int>(type: "INTEGER", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Label = table.Column<string>(type: "TEXT", nullable: false),
                    ReadOnlyToken = table.Column<string>(type: "TEXT", nullable: true),
                    WorldName = table.Column<string>(type: "TEXT", nullable: false),
                    TacMapID = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReplayMap", x => x.ReplayMapID);
                    table.ForeignKey(
                        name: "FK_ReplayMap_TacMap_TacMapID",
                        column: x => x.TacMapID,
                        principalTable: "TacMap",
                        principalColumn: "TacMapID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReplayMap_User_OwnerUserID",
                        column: x => x.OwnerUserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReplayFrame",
                columns: table => new
                {
                    FrameNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    ReplayMapID = table.Column<int>(type: "INTEGER", nullable: false),
                    GameDateTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ServerDateTimeUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Data = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReplayFrame", x => new { x.ReplayMapID, x.FrameNumber });
                    table.ForeignKey(
                        name: "FK_ReplayFrame_ReplayMap_ReplayMapID",
                        column: x => x.ReplayMapID,
                        principalTable: "ReplayMap",
                        principalColumn: "ReplayMapID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReplayGroup",
                columns: table => new
                {
                    GroupNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    ReplayMapID = table.Column<int>(type: "INTEGER", nullable: false),
                    Side = table.Column<int>(type: "INTEGER", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReplayGroup", x => new { x.ReplayMapID, x.GroupNumber });
                    table.ForeignKey(
                        name: "FK_ReplayGroup_ReplayMap_ReplayMapID",
                        column: x => x.ReplayMapID,
                        principalTable: "ReplayMap",
                        principalColumn: "ReplayMapID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReplayPlayer",
                columns: table => new
                {
                    PlayerNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    ReplayMapID = table.Column<int>(type: "INTEGER", nullable: false),
                    Uid = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReplayPlayer", x => new { x.ReplayMapID, x.PlayerNumber });
                    table.ForeignKey(
                        name: "FK_ReplayPlayer_ReplayMap_ReplayMapID",
                        column: x => x.ReplayMapID,
                        principalTable: "ReplayMap",
                        principalColumn: "ReplayMapID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReplayUnit",
                columns: table => new
                {
                    UnitNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    ReplayMapID = table.Column<int>(type: "INTEGER", nullable: false),
                    ClassName = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Side = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReplayUnit", x => new { x.ReplayMapID, x.UnitNumber });
                    table.ForeignKey(
                        name: "FK_ReplayUnit_ReplayMap_ReplayMapID",
                        column: x => x.ReplayMapID,
                        principalTable: "ReplayMap",
                        principalColumn: "ReplayMapID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReplayVehicle",
                columns: table => new
                {
                    VehicleNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    ReplayMapID = table.Column<int>(type: "INTEGER", nullable: false),
                    ClassName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReplayVehicle", x => new { x.ReplayMapID, x.VehicleNumber });
                    table.ForeignKey(
                        name: "FK_ReplayVehicle_ReplayMap_ReplayMapID",
                        column: x => x.ReplayMapID,
                        principalTable: "ReplayMap",
                        principalColumn: "ReplayMapID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReplayMap_OwnerUserID",
                table: "ReplayMap",
                column: "OwnerUserID");

            migrationBuilder.CreateIndex(
                name: "IX_ReplayMap_TacMapID",
                table: "ReplayMap",
                column: "TacMapID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReplayFrame");

            migrationBuilder.DropTable(
                name: "ReplayGroup");

            migrationBuilder.DropTable(
                name: "ReplayPlayer");

            migrationBuilder.DropTable(
                name: "ReplayUnit");

            migrationBuilder.DropTable(
                name: "ReplayVehicle");

            migrationBuilder.DropTable(
                name: "ReplayMap");
        }
    }
}

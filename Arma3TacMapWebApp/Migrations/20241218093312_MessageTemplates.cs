using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arma3TacMapWebApp.Migrations
{
    /// <inheritdoc />
    public partial class MessageTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MessageTemplate",
                columns: table => new
                {
                    MessageTemplateID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OwnerUserID = table.Column<int>(type: "INTEGER", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Label = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Visibility = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    CountryCode = table.Column<string>(type: "TEXT", nullable: true),
                    Token = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageTemplate", x => x.MessageTemplateID);
                    table.ForeignKey(
                        name: "FK_MessageTemplate_User_OwnerUserID",
                        column: x => x.OwnerUserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MessageLineTemplate",
                columns: table => new
                {
                    MessageLineTemplateID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MessageTemplateID = table.Column<int>(type: "INTEGER", nullable: false),
                    SortNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageLineTemplate", x => x.MessageLineTemplateID);
                    table.ForeignKey(
                        name: "FK_MessageLineTemplate_MessageTemplate_MessageTemplateID",
                        column: x => x.MessageTemplateID,
                        principalTable: "MessageTemplate",
                        principalColumn: "MessageTemplateID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MessageFieldTemplate",
                columns: table => new
                {
                    MessageFieldTemplateID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MessageLineTemplateID = table.Column<int>(type: "INTEGER", nullable: false),
                    SortNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Type = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageFieldTemplate", x => x.MessageFieldTemplateID);
                    table.ForeignKey(
                        name: "FK_MessageFieldTemplate_MessageLineTemplate_MessageLineTemplateID",
                        column: x => x.MessageLineTemplateID,
                        principalTable: "MessageLineTemplate",
                        principalColumn: "MessageLineTemplateID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MessageFieldTemplate_MessageLineTemplateID",
                table: "MessageFieldTemplate",
                column: "MessageLineTemplateID");

            migrationBuilder.CreateIndex(
                name: "IX_MessageLineTemplate_MessageTemplateID",
                table: "MessageLineTemplate",
                column: "MessageTemplateID");

            migrationBuilder.CreateIndex(
                name: "IX_MessageTemplate_OwnerUserID",
                table: "MessageTemplate",
                column: "OwnerUserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageFieldTemplate");

            migrationBuilder.DropTable(
                name: "MessageLineTemplate");

            migrationBuilder.DropTable(
                name: "MessageTemplate");
        }
    }
}

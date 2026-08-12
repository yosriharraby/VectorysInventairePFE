using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VectorysInventaire.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Sprint3_ScansSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Scans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmplacementId = table.Column<int>(type: "int", nullable: false),
                    AgentId = table.Column<int>(type: "int", nullable: false),
                    CodeBarresScan = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    DateHeureScan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateHeureSyncServeur = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstOffline = table.Column<bool>(type: "bit", nullable: false),
                    IdLocalFlutter = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Scans_Emplacements_EmplacementId",
                        column: x => x.EmplacementId,
                        principalTable: "Emplacements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Scans_Utilisateurs_AgentId",
                        column: x => x.AgentId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Scans_AgentId",
                table: "Scans",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_Scans_EmplacementId_DateHeureScan",
                table: "Scans",
                columns: new[] { "EmplacementId", "DateHeureScan" });

            migrationBuilder.CreateIndex(
                name: "IX_Scans_IdLocalFlutter",
                table: "Scans",
                column: "IdLocalFlutter",
                unique: true,
                filter: "[IdLocalFlutter] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Scans");
        }
    }
}

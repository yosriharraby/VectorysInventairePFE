using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VectorysInventaire.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Sprint8_RecomptagesSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Recomptages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InventaireId = table.Column<int>(type: "int", nullable: false),
                    PassageC2Id = table.Column<int>(type: "int", nullable: false),
                    Mode = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    AssignationMode = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    FichierNom = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    SuperviseurId = table.Column<int>(type: "int", nullable: false),
                    EmplacementsCrees = table.Column<int>(type: "int", nullable: false),
                    EcartsTraites = table.Column<int>(type: "int", nullable: false),
                    Statut = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recomptages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recomptages_Inventaires_InventaireId",
                        column: x => x.InventaireId,
                        principalTable: "Inventaires",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Recomptages_Passages_PassageC2Id",
                        column: x => x.PassageC2Id,
                        principalTable: "Passages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Recomptages_Utilisateurs_SuperviseurId",
                        column: x => x.SuperviseurId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecomptageAgents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecomptageId = table.Column<int>(type: "int", nullable: false),
                    AgentId = table.Column<int>(type: "int", nullable: false),
                    EmplacementsAssignes = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecomptageAgents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecomptageAgents_Recomptages_RecomptageId",
                        column: x => x.RecomptageId,
                        principalTable: "Recomptages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecomptageAgents_Utilisateurs_AgentId",
                        column: x => x.AgentId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecomptageAgents_AgentId",
                table: "RecomptageAgents",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_RecomptageAgents_RecomptageId_AgentId",
                table: "RecomptageAgents",
                columns: new[] { "RecomptageId", "AgentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recomptages_InventaireId_CreatedAt",
                table: "Recomptages",
                columns: new[] { "InventaireId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Recomptages_PassageC2Id",
                table: "Recomptages",
                column: "PassageC2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Recomptages_SuperviseurId",
                table: "Recomptages",
                column: "SuperviseurId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecomptageAgents");

            migrationBuilder.DropTable(
                name: "Recomptages");
        }
    }
}

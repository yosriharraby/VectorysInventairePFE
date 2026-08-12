using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VectorysInventaire.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Sprint2_InventaireSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Inventaires",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Reference = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Intitule = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Zone = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateDebut = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Statut = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    SuperviseurId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventaires", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inventaires_Utilisateurs_SuperviseurId",
                        column: x => x.SuperviseurId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Passages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InventaireId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Numero = table.Column<int>(type: "int", nullable: false),
                    Statut = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    DateDebut = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Passages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Passages_Inventaires_InventaireId",
                        column: x => x.InventaireId,
                        principalTable: "Inventaires",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Emplacements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PassageId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CodeBarres = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Statut = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    AgentAssigneId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Emplacements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Emplacements_Passages_PassageId",
                        column: x => x.PassageId,
                        principalTable: "Passages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Emplacements_Utilisateurs_AgentAssigneId",
                        column: x => x.AgentAssigneId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "StockTheorique",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmplacementId = table.Column<int>(type: "int", nullable: false),
                    CodeUS = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    DesignationUS = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    QteTheorique = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTheorique", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockTheorique_Emplacements_EmplacementId",
                        column: x => x.EmplacementId,
                        principalTable: "Emplacements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Emplacements_AgentAssigneId",
                table: "Emplacements",
                column: "AgentAssigneId");

            migrationBuilder.CreateIndex(
                name: "IX_Emplacements_PassageId_AgentAssigneId",
                table: "Emplacements",
                columns: new[] { "PassageId", "AgentAssigneId" });

            migrationBuilder.CreateIndex(
                name: "IX_Inventaires_Reference",
                table: "Inventaires",
                column: "Reference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inventaires_SuperviseurId",
                table: "Inventaires",
                column: "SuperviseurId");

            migrationBuilder.CreateIndex(
                name: "IX_Passages_InventaireId_Type",
                table: "Passages",
                columns: new[] { "InventaireId", "Type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockTheorique_EmplacementId_CodeUS",
                table: "StockTheorique",
                columns: new[] { "EmplacementId", "CodeUS" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockTheorique");

            migrationBuilder.DropTable(
                name: "Emplacements");

            migrationBuilder.DropTable(
                name: "Passages");

            migrationBuilder.DropTable(
                name: "Inventaires");
        }
    }
}

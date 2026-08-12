using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VectorysInventaire.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Sprint5_EcartsSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ecart",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InventaireId = table.Column<int>(type: "int", nullable: false),
                    PassageId = table.Column<int>(type: "int", nullable: false),
                    EmplacementId = table.Column<int>(type: "int", nullable: false),
                    CodeEmplacement = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    DesignationEmplacement = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CodeUS = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    DesignationUS = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    QteTheorique = table.Column<int>(type: "int", nullable: false),
                    QteScannee = table.Column<int>(type: "int", nullable: false),
                    Delta = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Statut = table.Column<int>(type: "int", nullable: false),
                    DetecteAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TraiteAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ecart", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ecart_Emplacements_EmplacementId",
                        column: x => x.EmplacementId,
                        principalTable: "Emplacements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ecart_Inventaires_InventaireId",
                        column: x => x.InventaireId,
                        principalTable: "Inventaires",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ecart_Passages_PassageId",
                        column: x => x.PassageId,
                        principalTable: "Passages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ecart_EmplacementId",
                table: "Ecart",
                column: "EmplacementId");

            migrationBuilder.CreateIndex(
                name: "IX_Ecart_InventaireId_EmplacementId_CodeUS_Statut",
                table: "Ecart",
                columns: new[] { "InventaireId", "EmplacementId", "CodeUS", "Statut" });

            migrationBuilder.CreateIndex(
                name: "IX_Ecart_PassageId",
                table: "Ecart",
                column: "PassageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ecart");
        }
    }
}

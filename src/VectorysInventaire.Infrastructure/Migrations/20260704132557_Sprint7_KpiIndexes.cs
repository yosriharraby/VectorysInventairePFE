using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VectorysInventaire.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Sprint7_KpiIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Scans_DateHeureScan",
                table: "Scans",
                column: "DateHeureScan");

            migrationBuilder.CreateIndex(
                name: "IX_Ecart_InventaireId_DetecteAt",
                table: "Ecart",
                columns: new[] { "InventaireId", "DetecteAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Ecart_Statut_DetecteAt",
                table: "Ecart",
                columns: new[] { "Statut", "DetecteAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Scans_DateHeureScan",
                table: "Scans");

            migrationBuilder.DropIndex(
                name: "IX_Ecart_InventaireId_DetecteAt",
                table: "Ecart");

            migrationBuilder.DropIndex(
                name: "IX_Ecart_Statut_DetecteAt",
                table: "Ecart");
        }
    }
}

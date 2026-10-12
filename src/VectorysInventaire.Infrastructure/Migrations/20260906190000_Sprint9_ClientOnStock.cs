using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VectorysInventaire.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Sprint9_ClientOnStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Client",
                table: "StockTheorique",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Client",
                table: "Ecart",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Client",
                table: "StockTheorique");

            migrationBuilder.DropColumn(
                name: "Client",
                table: "Ecart");
        }
    }
}

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using VectorysInventaire.Infrastructure.Persistence;

#nullable disable

namespace VectorysInventaire.Infrastructure.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(VectorysDbContext))]
    [Migration("20260907020000_Sprint6_RecomptageProgress")]
    public partial class Sprint6_RecomptageProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF COL_LENGTH('Emplacements', 'RecomptageId') IS NULL
                    ALTER TABLE [Emplacements] ADD [RecomptageId] int NULL;

                IF NOT EXISTS (
                    SELECT 1 FROM sys.indexes
                    WHERE name = N'IX_Emplacements_RecomptageId' AND object_id = OBJECT_ID(N'Emplacements'))
                    CREATE INDEX [IX_Emplacements_RecomptageId] ON [Emplacements] ([RecomptageId]);

                IF NOT EXISTS (
                    SELECT 1 FROM sys.foreign_keys
                    WHERE name = N'FK_Emplacements_Recomptages_RecomptageId')
                    ALTER TABLE [Emplacements]
                        ADD CONSTRAINT [FK_Emplacements_Recomptages_RecomptageId]
                        FOREIGN KEY ([RecomptageId]) REFERENCES [Recomptages] ([Id]) ON DELETE NO ACTION;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Emplacements_Recomptages_RecomptageId",
                table: "Emplacements");

            migrationBuilder.DropIndex(
                name: "IX_Emplacements_RecomptageId",
                table: "Emplacements");

            migrationBuilder.DropColumn(
                name: "RecomptageId",
                table: "Emplacements");
        }
    }
}

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using VectorysInventaire.Infrastructure.Persistence;

#nullable disable

namespace VectorysInventaire.Infrastructure.Migrations
{
    [DbContext(typeof(VectorysDbContext))]
    [Migration("20261022180000_WidenExcelStringColumns")]
    public partial class WidenExcelStringColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE [Emplacements] ALTER COLUMN [Code] nvarchar(256) NOT NULL;
                ALTER TABLE [Emplacements] ALTER COLUMN [CodeBarres] nvarchar(256) NOT NULL;
                ALTER TABLE [Emplacements] ALTER COLUMN [Designation] nvarchar(500) NOT NULL;

                ALTER TABLE [StockTheorique] ALTER COLUMN [CodeUS] nvarchar(256) NOT NULL;
                ALTER TABLE [StockTheorique] ALTER COLUMN [DesignationUS] nvarchar(500) NOT NULL;
                ALTER TABLE [StockTheorique] ALTER COLUMN [Client] nvarchar(256) NULL;

                ALTER TABLE [Ecart] ALTER COLUMN [CodeEmplacement] nvarchar(256) NOT NULL;
                ALTER TABLE [Ecart] ALTER COLUMN [CodeUS] nvarchar(256) NOT NULL;
                ALTER TABLE [Ecart] ALTER COLUMN [DesignationEmplacement] nvarchar(500) NOT NULL;
                ALTER TABLE [Ecart] ALTER COLUMN [DesignationUS] nvarchar(500) NOT NULL;
                ALTER TABLE [Ecart] ALTER COLUMN [Client] nvarchar(256) NULL;

                ALTER TABLE [Scans] ALTER COLUMN [CodeBarresScan] nvarchar(256) NOT NULL;
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE [Emplacements] ALTER COLUMN [Code] nvarchar(64) NOT NULL;
                ALTER TABLE [Emplacements] ALTER COLUMN [CodeBarres] nvarchar(64) NOT NULL;
                ALTER TABLE [Emplacements] ALTER COLUMN [Designation] nvarchar(200) NOT NULL;

                ALTER TABLE [StockTheorique] ALTER COLUMN [CodeUS] nvarchar(64) NOT NULL;
                ALTER TABLE [StockTheorique] ALTER COLUMN [DesignationUS] nvarchar(200) NOT NULL;
                ALTER TABLE [StockTheorique] ALTER COLUMN [Client] nvarchar(128) NULL;

                ALTER TABLE [Ecart] ALTER COLUMN [CodeEmplacement] nvarchar(64) NOT NULL;
                ALTER TABLE [Ecart] ALTER COLUMN [CodeUS] nvarchar(64) NOT NULL;
                ALTER TABLE [Ecart] ALTER COLUMN [DesignationEmplacement] nvarchar(200) NOT NULL;
                ALTER TABLE [Ecart] ALTER COLUMN [DesignationUS] nvarchar(200) NOT NULL;
                ALTER TABLE [Ecart] ALTER COLUMN [Client] nvarchar(128) NULL;

                ALTER TABLE [Scans] ALTER COLUMN [CodeBarresScan] nvarchar(128) NOT NULL;
                """);
        }
    }
}

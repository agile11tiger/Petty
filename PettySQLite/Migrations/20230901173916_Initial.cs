using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PettySQLite.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UseFrontCamera = table.Column<bool>(type: "INTEGER", nullable: true),
                    TryHarder = table.Column<bool>(type: "INTEGER", nullable: true),
                    TryInverted = table.Column<bool>(type: "INTEGER", nullable: true),
                    LanguageType = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Settings");
        }
    }
}

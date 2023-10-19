using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PettySQLite.Migrations;

/// <inheritdoc />
public partial class initial : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "BaseSettings",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false),
                InformationPerceptionMode = table.Column<int>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_BaseSettings", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "VoiceSettings",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false),
                Pitch = table.Column<float>(type: "REAL", nullable: false),
                Volume = table.Column<float>(type: "REAL", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_VoiceSettings", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Settings",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false),
                BaseSettingsId = table.Column<int>(type: "INTEGER", nullable: true),
                VoiceSettingsId = table.Column<int>(type: "INTEGER", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Settings", x => x.Id);
                table.ForeignKey(
                    name: "FK_Settings_BaseSettings_BaseSettingsId",
                    column: x => x.BaseSettingsId,
                    principalTable: "BaseSettings",
                    principalColumn: "Id");
                table.ForeignKey(
                    name: "FK_Settings_VoiceSettings_VoiceSettingsId",
                    column: x => x.VoiceSettingsId,
                    principalTable: "VoiceSettings",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateIndex(
            name: "IX_Settings_BaseSettingsId",
            table: "Settings",
            column: "BaseSettingsId");

        migrationBuilder.CreateIndex(
            name: "IX_Settings_VoiceSettingsId",
            table: "Settings",
            column: "VoiceSettingsId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Settings");

        migrationBuilder.DropTable(
            name: "BaseSettings");

        migrationBuilder.DropTable(
            name: "VoiceSettings");
    }
}

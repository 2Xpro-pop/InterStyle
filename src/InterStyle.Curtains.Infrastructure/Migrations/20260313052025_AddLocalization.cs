using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterStyle.Curtains.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLocalization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "description",
                schema: "curtains",
                table: "curtains");

            migrationBuilder.DropColumn(
                name: "name",
                schema: "curtains",
                table: "curtains");

            migrationBuilder.CreateTable(
                name: "curtain_translations",
                schema: "curtains",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    locale = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    curtain_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_curtain_translations", x => x.id);
                    table.ForeignKey(
                        name: "FK_curtain_translations_curtains_curtain_id",
                        column: x => x.curtain_id,
                        principalSchema: "curtains",
                        principalTable: "curtains",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_curtain_translations_curtain_id_locale",
                schema: "curtains",
                table: "curtain_translations",
                columns: new[] { "curtain_id", "locale" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "curtain_translations",
                schema: "curtains");

            migrationBuilder.AddColumn<string>(
                name: "description",
                schema: "curtains",
                table: "curtains",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "name",
                schema: "curtains",
                table: "curtains",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}

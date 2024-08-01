using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace StatisticDbContext.Migrations
{
    /// <inheritdoc />
    public partial class Army : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "count",
                table: "inventory");

            migrationBuilder.AlterColumn<string>(
                name: "asset",
                table: "items",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "user_id",
                table: "inventory",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<int>(
                name: "army_id",
                table: "inventory",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "heroes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    level = table.Column<int>(type: "integer", nullable: false),
                    asset = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_heroes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "armies",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    barrack_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_armies", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "barracks",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    hero_id = table.Column<int>(type: "integer", nullable: false),
                    army_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_barracks", x => x.id);
                    table.ForeignKey(
                        name: "FK_barracks_armies_army_id",
                        column: x => x.army_id,
                        principalTable: "armies",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_barracks_heroes_hero_id",
                        column: x => x.hero_id,
                        principalTable: "heroes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_inventory_army_id",
                table: "inventory",
                column: "army_id");

            migrationBuilder.CreateIndex(
                name: "IX_armies_barrack_id",
                table: "armies",
                column: "barrack_id");

            migrationBuilder.CreateIndex(
                name: "IX_barracks_army_id",
                table: "barracks",
                column: "army_id");

            migrationBuilder.CreateIndex(
                name: "IX_barracks_hero_id",
                table: "barracks",
                column: "hero_id");

            migrationBuilder.AddForeignKey(
                name: "FK_inventory_armies_army_id",
                table: "inventory",
                column: "army_id",
                principalTable: "armies",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_armies_barracks_barrack_id",
                table: "armies",
                column: "barrack_id",
                principalTable: "barracks",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_inventory_armies_army_id",
                table: "inventory");

            migrationBuilder.DropForeignKey(
                name: "FK_armies_barracks_barrack_id",
                table: "armies");

            migrationBuilder.DropTable(
                name: "barracks");

            migrationBuilder.DropTable(
                name: "armies");

            migrationBuilder.DropTable(
                name: "heroes");

            migrationBuilder.DropIndex(
                name: "IX_inventory_army_id",
                table: "inventory");

            migrationBuilder.DropColumn(
                name: "army_id",
                table: "inventory");

            migrationBuilder.AlterColumn<string>(
                name: "asset",
                table: "items",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "user_id",
                table: "inventory",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "count",
                table: "inventory",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}

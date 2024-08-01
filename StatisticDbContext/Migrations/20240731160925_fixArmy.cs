using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StatisticDbContext.Migrations
{
    /// <inheritdoc />
    public partial class fixArmy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_barracks_armies_army_id",
                table: "barracks");

            migrationBuilder.DropIndex(
                name: "IX_barracks_army_id",
                table: "barracks");

            migrationBuilder.DropIndex(
                name: "IX_armies_barrack_id",
                table: "armies");

            migrationBuilder.DropColumn(
                name: "army_id",
                table: "barracks");

            migrationBuilder.CreateIndex(
                name: "IX_armies_barrack_id",
                table: "armies",
                column: "barrack_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_armies_barrack_id",
                table: "armies");

            migrationBuilder.AddColumn<int>(
                name: "army_id",
                table: "barracks",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_barracks_army_id",
                table: "barracks",
                column: "army_id");

            migrationBuilder.CreateIndex(
                name: "IX_armies_barrack_id",
                table: "armies",
                column: "barrack_id");

            migrationBuilder.AddForeignKey(
                name: "FK_barracks_armies_army_id",
                table: "barracks",
                column: "army_id",
                principalTable: "armies",
                principalColumn: "id");
        }
    }
}

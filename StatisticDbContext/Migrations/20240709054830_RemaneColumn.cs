using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StatisticDbContext.Migrations
{
    /// <inheritdoc />
    public partial class RemaneColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "coin",
                table: "storage",
                newName: "main_coin");

            migrationBuilder.AddColumn<long>(
                name: "refer_coin",
                table: "storage",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "refer_coin",
                table: "storage");

            migrationBuilder.RenameColumn(
                name: "main_coin",
                table: "storage",
                newName: "coin");
        }
    }
}

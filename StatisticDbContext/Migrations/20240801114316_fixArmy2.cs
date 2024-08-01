using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StatisticDbContext.Migrations
{
    /// <inheritdoc />
    public partial class fixArmy2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "useType",
                table: "armies",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "useType",
                table: "armies");
        }
    }
}

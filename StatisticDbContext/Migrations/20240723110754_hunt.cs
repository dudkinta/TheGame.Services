using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StatisticDbContext.Migrations
{
    /// <inheritdoc />
    public partial class Hunt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "aim",
                table: "storage",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "hunts",
                table: "storage",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "shots",
                table: "storage",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "aim",
                table: "storage");

            migrationBuilder.DropColumn(
                name: "hunts",
                table: "storage");

            migrationBuilder.DropColumn(
                name: "shots",
                table: "storage");
        }
    }
}

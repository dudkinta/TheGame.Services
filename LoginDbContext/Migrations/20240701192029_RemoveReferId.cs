using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoginDbContext.Migrations
{
    /// <inheritdoc />
    public partial class RemoveReferId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_users_refer_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "refer_id",
                table: "users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "refer_id",
                table: "users",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_refer_id",
                table: "users",
                column: "refer_id");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FriendDbContex.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "friends",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    refer_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_friends", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_friends_id",
                table: "friends",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_friends_refer_id",
                table: "friends",
                column: "refer_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
            name: "IX_friends_id",
            table: "friends");

            migrationBuilder.DropIndex(
                name: "IX_friends_refer_id",
                table: "friends");

            migrationBuilder.DropTable(
                name: "friends");
        }
    }
}

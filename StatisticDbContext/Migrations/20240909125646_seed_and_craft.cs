using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StatisticDbContext.Migrations
{
    /// <inheritdoc />
    public partial class seed_and_craft : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "recipes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    item_id = table.Column<int>(type: "integer", nullable: false),
                    craft_time = table.Column<int>(type: "integer", nullable: false),
                    crafting_station = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recipes", x => x.id);
                    table.ForeignKey(
                        name: "FK_recipes_items_item_id",
                        column: x => x.item_id,
                        principalTable: "items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "crafts",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    recire_id = table.Column<int>(type: "integer", nullable: false),
                    dt_end = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_crafts", x => x.id);
                    table.ForeignKey(
                        name: "FK_crafts_recipes_recire_id",
                        column: x => x.recire_id,
                        principalTable: "recipes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ingredients",
                columns: table => new
                {
                    recipe_id = table.Column<int>(type: "integer", nullable: false),
                    ingredient_id = table.Column<int>(type: "integer", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ingredients", x => new { x.recipe_id, x.ingredient_id });
                    table.ForeignKey(
                        name: "FK_ingredients_items_ingredient_id",
                        column: x => x.ingredient_id,
                        principalTable: "items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_recipe_ingredients_recipes_recipe_id",
                        column: x => x.recipe_id,
                        principalTable: "recipes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "heroes",
                columns: new[] { "id", "asset", "description", "level", "name", "type" },
                values: new object[,]
                {
                    { 2, "standart", "", 1, "Младший дружинник", "Богатыри" },
                    { 3, "standart", "", 2, "Дружинник", "Богатыри" },
                    { 4, "standart", "", 3, "Старший дружинник", "Богатыри" },
                    { 5, "standart", "", 4, "Сотник", "Богатыри" },
                    { 6, "standart", "", 5, "Старший сотник", "Богатыри" },
                    { 7, "standart", "", 6, "Молодой богатырь", "Богатыри" },
                    { 8, "standart", "", 7, "Богатырь", "Богатыри" },
                    { 9, "standart", "", 8, "Старший богатырь", "Богатыри" },
                    { 10, "standart", "", 9, "Витязь", "Богатыри" },
                    { 11, "standart", "", 10, "Илья Муромец", "Богатыри" },
                    { 12, "standart", "", 1, "Ученик волхва", "Волхвы" },
                    { 13, "standart", "", 2, "Младший волхв", "Волхвы" },
                    { 14, "standart", "", 3, "Волхв", "Волхвы" },
                    { 15, "standart", "", 4, "Старший волхв", "Волхвы" },
                    { 16, "standart", "", 5, "Мастер волхвов", "Волхвы" },
                    { 17, "standart", "", 6, "Хранитель тайн", "Волхвы" },
                    { 18, "standart", "", 7, "Мудрец", "Волхвы" },
                    { 19, "standart", "", 8, "Старейшина", "Волхвы" },
                    { 20, "standart", "", 9, "Верховный волхв", "Волхвы" },
                    { 21, "standart", "", 10, "Великий волхв", "Волхвы" },
                    { 22, "standart", "", 1, "Лесовичок", "Лесные существа" },
                    { 23, "standart", "", 2, "Лесной дух", "Лесные существа" },
                    { 24, "standart", "", 3, "Младший леший", "Лесные существа" },
                    { 25, "standart", "", 4, "Леший", "Лесные существа" },
                    { 26, "standart", "", 5, "Старший леший", "Лесные существа" },
                    { 27, "standart", "", 6, "Мастер леса", "Лесные существа" },
                    { 28, "standart", "", 7, "Хранитель леса", "Лесные существа" },
                    { 29, "standart", "", 8, "Лесной мудрец", "Лесные существа" },
                    { 30, "standart", "", 9, "Лесной старейшина", "Лесные существа" },
                    { 31, "standart", "", 10, "Владыка леса", "Лесные существа" },
                    { 32, "standart", "", 1, "Маленький домовой", "Домовые и духи" },
                    { 33, "standart", "", 2, "Домовой", "Домовые и духи" },
                    { 34, "standart", "", 3, "Старший домовой", "Домовые и духи" },
                    { 35, "standart", "", 4, "Хранитель дома", "Домовые и духи" },
                    { 36, "standart", "", 5, "Мастер оберега", "Домовые и духи" },
                    { 37, "standart", "", 6, "Домовой мудрец", "Домовые и духи" },
                    { 38, "standart", "", 7, "Домовой старейшина", "Домовые и духи" },
                    { 39, "standart", "", 8, "Великий хранитель", "Домовые и духи" },
                    { 40, "standart", "", 9, "Дух дома", "Домовые и духи" },
                    { 41, "standart", "", 10, "Домовой маг", "Домовые и духи" },
                    { 42, "standart", "", 1, "Младший помощник", "Сказочные персонажи" },
                    { 43, "standart", "", 2, "Помощник", "Сказочные персонажи" },
                    { 44, "standart", "", 3, "Мудрый помощник", "Сказочные персонажи" },
                    { 45, "standart", "", 4, "Лихо одноглазое", "Сказочные персонажи" },
                    { 46, "standart", "", 5, "Соловей-разбойник", "Сказочные персонажи" },
                    { 47, "standart", "", 6, "Баба-яга", "Сказочные персонажи" },
                    { 48, "standart", "", 7, "Змей Горыныч", "Сказочные персонажи" },
                    { 49, "standart", "", 8, "Кощей Бессмертный", "Сказочные персонажи" },
                    { 50, "standart", "", 9, "Ведунья", "Сказочные персонажи" },
                    { 51, "standart", "", 10, "Верховная колдунья", "Сказочные персонажи" }
                });

            migrationBuilder.InsertData(
                table: "items",
                columns: new[] { "id", "asset", "description", "level", "name", "type" },
                values: new object[,]
                {
                    { 1, "standart", "Тяжелая деревянная дубина, простое, но эффективное оружие для ближнего боя.", 1, "Дубина дружинника", "gun" },
                    { 2, "standart", "Копьё, используемое для охоты на крупную дичь, украшенное перьями и резьбой.", 2, "Копьё охотника", "gun" },
                    { 3, "standart", "Топор с укрепленным лезвием, используемый в бою.", 3, "Боевой топор воина", "gun" },
                    { 4, "standart", "Лесное оружие, созданное из волшебного дерева и камня.", 4, "Секира лешего", "gun" },
                    { 5, "standart", "Длинное оружие с лезвием, используемое для защиты священных мест.", 5, "Алебарда старейшины", "gun" },
                    { 6, "standart", "Меч с широким лезвием, украшенный символами силы и отваги.", 6, "Широкий меч витязя", "gun" },
                    { 7, "standart", "Легендарный меч, передающийся из поколения в поколение среди богатырей.", 7, "Клинок богатыря", "gun" },
                    { 8, "standart", "Тяжелый молот, выкованный в священных кузницах.", 8, "Боевой молот кузнеца", "gun" },
                    { 9, "standart", "Изогнутая сабля, используемая воинами степей для быстрого и точного удара.", 9, "Сабля степного воителя", "gun" },
                    { 10, "standart", "Могучий клинок, которым пользовался сам Илья Муромец.", 10, "Клюв Ильи Муромца", "gun" },
                    { 11, "standart", "Кинжал, заряженный магией и способный наносить удары с ядовитым эффектом.", 11, "Зачарованный кинжал ведьмы", "gun" },
                    { 12, "standart", "Волшебный посох, позволяющий использовать силы природы и древних духов.", 12, "Посох волхва", "gun" },
                    { 14, "standart", "Жезл, контролирующий стихии огня, воды, земли и воздуха.", 13, "Жезл стихий", "gun" },
                    { 15, "standart", "Лук, сделанный из костей змия, стреляющий огненными стрелами.", 14, "Лук змия Горыныча", "gun" },
                    { 16, "standart", "Легендарный клинок, наделенный бессмертной силой, с которым нет равных.", 15, "Клинок Кощея Бессмертного", "gun" },
                    { 17, "standart", "Шкура мелкого животного испорченная выстрелом", 1, "Пушнина испорченная", "material" },
                    { 18, "standart", "Шкура животного обычного качества", 1, "Пушнина обычная", "material" },
                    { 19, "standart", "Шкура животного отличного качества", 2, "Пушнина царская", "material" },
                    { 20, "standart", "Кусок упавшего дерева", 1, "Бревно", "material" },
                    { 21, "standart", "Кусок железной руды", 1, "Железо", "material" }
                });

            migrationBuilder.InsertData(
                table: "recipes",
                columns: new[] { "id", "craft_time", "crafting_station", "item_id" },
                values: new object[,]
                {
                    { 1, 30, "forge", 1 },
                    { 2, 30, "forge", 2 },
                    { 3, 30, "forge", 3 },
                    { 4, 30, "forge", 4 },
                    { 5, 30, "forge", 5 }
                });

            migrationBuilder.InsertData(
                table: "ingredients",
                columns: new[] { "ingredient_id", "recipe_id", "quantity" },
                values: new object[,]
                {
                    { 20, 1, 1 },
                    { 1, 2, 1 },
                    { 20, 2, 1 },
                    { 2, 3, 1 },
                    { 20, 3, 1 },
                    { 21, 3, 1 },
                    { 3, 4, 1 },
                    { 20, 4, 1 },
                    { 21, 4, 1 },
                    { 4, 5, 1 },
                    { 20, 5, 1 },
                    { 21, 5, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_crafts_recire_id",
                table: "crafts",
                column: "recire_id");

            migrationBuilder.CreateIndex(
                name: "IX_ingredients_ingredient_id",
                table: "ingredients",
                column: "ingredient_id");

            migrationBuilder.CreateIndex(
                name: "IX_recipes_item_id",
                table: "recipes",
                column: "item_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "crafts");

            migrationBuilder.DropTable(
                name: "ingredients");

            migrationBuilder.DropTable(
                name: "recipes");
        }
    }
}

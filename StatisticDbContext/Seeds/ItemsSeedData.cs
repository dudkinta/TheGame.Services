using Microsoft.EntityFrameworkCore;
using StatisticDbContext.Models;

namespace StatisticDbContext.Seeds
{
    public static class ItemsSeedData
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ItemModel>().HasData(
                new ItemModel() { id = 1, name = "Дубина дружинника", description = "Тяжелая деревянная дубина, простое, но эффективное оружие для ближнего боя.", type = "gun", level = 1, asset = "standart" },
                new ItemModel() { id = 2, name = "Копьё охотника", description = "Копьё, используемое для охоты на крупную дичь, украшенное перьями и резьбой.", type = "gun", level = 2, asset = "standart" },
                new ItemModel() { id = 4, name = "Секира лешего", description = "Лесное оружие, созданное из волшебного дерева и камня.", type = "gun", level = 4, asset = "standart" },
                new ItemModel() { id = 3, name = "Боевой топор воина", description = "Топор с укрепленным лезвием, используемый в бою.", type = "gun", level = 3, asset = "standart" },
                new ItemModel() { id = 5, name = "Алебарда старейшины", description = "Длинное оружие с лезвием, используемое для защиты священных мест.", type = "gun", level = 5, asset = "standart" },
                new ItemModel() { id = 6, name = "Широкий меч витязя", description = "Меч с широким лезвием, украшенный символами силы и отваги.", type = "gun", level = 6, asset = "standart" },
                new ItemModel() { id = 7, name = "Клинок богатыря", description = "Легендарный меч, передающийся из поколения в поколение среди богатырей.", type = "gun", level = 7, asset = "standart" },
                new ItemModel() { id = 8, name = "Боевой молот кузнеца", description = "Тяжелый молот, выкованный в священных кузницах.", type = "gun", level = 8, asset = "standart" },
                new ItemModel() { id = 9, name = "Сабля степного воителя", description = "Изогнутая сабля, используемая воинами степей для быстрого и точного удара.", type = "gun", level = 9, asset = "standart" },
                new ItemModel() { id = 10, name = "Клюв Ильи Муромца", description = "Могучий клинок, которым пользовался сам Илья Муромец.", type = "gun", level = 10, asset = "standart" },
                new ItemModel() { id = 11, name = "Зачарованный кинжал ведьмы", description = "Кинжал, заряженный магией и способный наносить удары с ядовитым эффектом.", type = "gun", level = 11, asset = "standart" },
                new ItemModel() { id = 12, name = "Посох волхва", description = "Волшебный посох, позволяющий использовать силы природы и древних духов.", type = "gun", level = 12, asset = "standart" },
                new ItemModel() { id = 14, name = "Жезл стихий", description = "Жезл, контролирующий стихии огня, воды, земли и воздуха.", type = "gun", level = 13, asset = "standart" },
                new ItemModel() { id = 15, name = "Лук змия Горыныча", description = "Лук, сделанный из костей змия, стреляющий огненными стрелами.", type = "gun", level = 14, asset = "standart" },
                new ItemModel() { id = 16, name = "Клинок Кощея Бессмертного", description = "Легендарный клинок, наделенный бессмертной силой, с которым нет равных.", type = "gun", level = 15, asset = "standart" },
                new ItemModel() { id = 17, name = "Пушнина испорченная", description = "Шкура мелкого животного испорченная выстрелом", type = "material", level = 1, asset = "standart" },
                new ItemModel() { id = 18, name = "Пушнина обычная", description = "Шкура животного обычного качества", type = "material", level = 1, asset = "standart" },
                new ItemModel() { id = 19, name = "Пушнина царская", description = "Шкура животного отличного качества", type = "material", level = 2, asset = "standart" },
                new ItemModel() { id = 20, name = "Бревно", description = "Кусок упавшего дерева", type = "material", level = 1, asset = "standart" },
                new ItemModel() { id = 21, name = "Железо", description = "Кусок железной руды", type = "material", level = 1, asset = "standart" }
                );
        }
    }
}

using Microsoft.EntityFrameworkCore;
using StatisticDbContext.Models;

namespace StatisticDbContext.Seeds
{
    public static class HeroSeedData
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HeroModel>().HasData(
                new HeroModel() { id = 2, name = "Младший дружинник", description = "", type = "Богатыри", level = 1, asset = "standart" },
                new HeroModel() { id = 3, name = "Дружинник", description = "", type = "Богатыри", level = 2, asset = "standart" },
                new HeroModel() { id = 4, name = "Старший дружинник", description = "", type = "Богатыри", level = 3, asset = "standart" },
                new HeroModel() { id = 5, name = "Сотник", description = "", type = "Богатыри", level = 4, asset = "standart" },
                new HeroModel() { id = 6, name = "Старший сотник", description = "", type = "Богатыри", level = 5, asset = "standart" },
                new HeroModel() { id = 7, name = "Молодой богатырь", description = "", type = "Богатыри", level = 6, asset = "standart" },
                new HeroModel() { id = 8, name = "Богатырь", description = "", type = "Богатыри", level = 7, asset = "standart" },
                new HeroModel() { id = 9, name = "Старший богатырь", description = "", type = "Богатыри", level = 8, asset = "standart" },
                new HeroModel() { id = 10, name = "Витязь", description = "", type = "Богатыри", level = 9, asset = "standart" },
                new HeroModel() { id = 11, name = "Илья Муромец", description = "", type = "Богатыри", level = 10, asset = "standart" },
                new HeroModel() { id = 12, name = "Ученик волхва", description = "", type = "Волхвы", level = 1, asset = "standart" },
                new HeroModel() { id = 13, name = "Младший волхв", description = "", type = "Волхвы", level = 2, asset = "standart" },
                new HeroModel() { id = 14, name = "Волхв", description = "", type = "Волхвы", level = 3, asset = "standart" },
                new HeroModel() { id = 15, name = "Старший волхв", description = "", type = "Волхвы", level = 4, asset = "standart" },
                new HeroModel() { id = 16, name = "Мастер волхвов", description = "", type = "Волхвы", level = 5, asset = "standart" },
                new HeroModel() { id = 17, name = "Хранитель тайн", description = "", type = "Волхвы", level = 6, asset = "standart" },
                new HeroModel() { id = 18, name = "Мудрец", description = "", type = "Волхвы", level = 7, asset = "standart" },
                new HeroModel() { id = 19, name = "Старейшина", description = "", type = "Волхвы", level = 8, asset = "standart" },
                new HeroModel() { id = 20, name = "Верховный волхв", description = "", type = "Волхвы", level = 9, asset = "standart" },
                new HeroModel() { id = 21, name = "Великий волхв", description = "", type = "Волхвы", level = 10, asset = "standart" },
                new HeroModel() { id = 22, name = "Лесовичок", description = "", type = "Лесные существа", level = 1, asset = "standart" },
                new HeroModel() { id = 23, name = "Лесной дух", description = "", type = "Лесные существа", level = 2, asset = "standart" },
                new HeroModel() { id = 24, name = "Младший леший", description = "", type = "Лесные существа", level = 3, asset = "standart" },
                new HeroModel() { id = 25, name = "Леший", description = "", type = "Лесные существа", level = 4, asset = "standart" },
                new HeroModel() { id = 26, name = "Старший леший", description = "", type = "Лесные существа", level = 5, asset = "standart" },
                new HeroModel() { id = 27, name = "Мастер леса", description = "", type = "Лесные существа", level = 6, asset = "standart" },
                new HeroModel() { id = 28, name = "Хранитель леса", description = "", type = "Лесные существа", level = 7, asset = "standart" },
                new HeroModel() { id = 29, name = "Лесной мудрец", description = "", type = "Лесные существа", level = 8, asset = "standart" },
                new HeroModel() { id = 30, name = "Лесной старейшина", description = "", type = "Лесные существа", level = 9, asset = "standart" },
                new HeroModel() { id = 31, name = "Владыка леса", description = "", type = "Лесные существа", level = 10, asset = "standart" },
                new HeroModel() { id = 32, name = "Маленький домовой", description = "", type = "Домовые и духи", level = 1, asset = "standart" },
                new HeroModel() { id = 33, name = "Домовой", description = "", type = "Домовые и духи", level = 2, asset = "standart" },
                new HeroModel() { id = 34, name = "Старший домовой", description = "", type = "Домовые и духи", level = 3, asset = "standart" },
                new HeroModel() { id = 35, name = "Хранитель дома", description = "", type = "Домовые и духи", level = 4, asset = "standart" },
                new HeroModel() { id = 36, name = "Мастер оберега", description = "", type = "Домовые и духи", level = 5, asset = "standart" },
                new HeroModel() { id = 37, name = "Домовой мудрец", description = "", type = "Домовые и духи", level = 6, asset = "standart" },
                new HeroModel() { id = 38, name = "Домовой старейшина", description = "", type = "Домовые и духи", level = 7, asset = "standart" },
                new HeroModel() { id = 39, name = "Великий хранитель", description = "", type = "Домовые и духи", level = 8, asset = "standart" },
                new HeroModel() { id = 40, name = "Дух дома", description = "", type = "Домовые и духи", level = 9, asset = "standart" },
                new HeroModel() { id = 41, name = "Домовой маг", description = "", type = "Домовые и духи", level = 10, asset = "standart" },
                new HeroModel() { id = 42, name = "Младший помощник", description = "", type = "Сказочные персонажи", level = 1, asset = "standart" },
                new HeroModel() { id = 43, name = "Помощник", description = "", type = "Сказочные персонажи", level = 2, asset = "standart" },
                new HeroModel() { id = 44, name = "Мудрый помощник", description = "", type = "Сказочные персонажи", level = 3, asset = "standart" },
                new HeroModel() { id = 45, name = "Лихо одноглазое", description = "", type = "Сказочные персонажи", level = 4, asset = "standart" },
                new HeroModel() { id = 46, name = "Соловей-разбойник", description = "", type = "Сказочные персонажи", level = 5, asset = "standart" },
                new HeroModel() { id = 47, name = "Баба-яга", description = "", type = "Сказочные персонажи", level = 6, asset = "standart" },
                new HeroModel() { id = 48, name = "Змей Горыныч", description = "", type = "Сказочные персонажи", level = 7, asset = "standart" },
                new HeroModel() { id = 49, name = "Кощей Бессмертный", description = "", type = "Сказочные персонажи", level = 8, asset = "standart" },
                new HeroModel() { id = 50, name = "Ведунья", description = "", type = "Сказочные персонажи", level = 9, asset = "standart" },
                new HeroModel() { id = 51, name = "Верховная колдунья", description = "", type = "Сказочные персонажи", level = 10, asset = "standart" }
                );
        }
    }
}

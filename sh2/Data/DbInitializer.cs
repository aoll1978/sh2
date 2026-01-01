
using sh2.Models;

namespace sh2.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Убеждаемся, что БД существует
            context.Database.EnsureCreated();

            // Если есть товары - выходим
            if (context.Products.Any())
            {
                return;
            }

            // 1. Создаем категории
            var electronics = new Category { Name = "Электроника", Description = "Гаджеты" };
            var books = new Category { Name = "Книги", Description = "Бумажные издания" };
            var clothes = new Category { Name = "Одежда", Description = "Вещи" };

            context.Categories.AddRange(electronics, books, clothes);
            context.SaveChanges(); // Сохраняем, чтобы получить ID

            // 2. Создаем товары
            // ВАЖНО: Мы НЕ указываем здесь Category, так как этого свойства больше нет в классе Product
            var prod1 = new Product
            {
                Name = "Смартфон SuperPhone",
                Price = 999.99m,
                InStock = 10, // Исправлено на int
                Material = "Пластик",
                Rating = 4.5
            };

            var prod2 = new Product
            {
                Name = "Книга 'Путь Героя'",
                Price = 15.50m,
                InStock = 100, // Исправлено на int
                Material = "Бумага",
                Rating = 5.0
            };

            var prod3 = new Product
            {
                Name = "Футболка C#",
                Price = 25.00m,
                InStock = 50, // Исправлено на int
                Material = "Хлопок",
                Rating = 4.8
            };

            context.Products.AddRange(prod1, prod2, prod3);
            context.SaveChanges(); // Сохраняем, чтобы получить ID товаров

            // 3. Создаем связи (Таблица ProductCategory)
            var productCategories = new ProductCategory[]
            {
                new ProductCategory { ProductId = prod1.Id, CategoryId = electronics.Id },
                new ProductCategory { ProductId = prod2.Id, CategoryId = books.Id },
                new ProductCategory { ProductId = prod3.Id, CategoryId = clothes.Id }
            };

            context.ProductCategories.AddRange(productCategories);
            context.SaveChanges();
        }
    }
}
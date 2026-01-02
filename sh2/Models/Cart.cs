namespace sh2.Models
{
    // Одна строчка в корзине (Товар + Количество)
    public class CartItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }

    // Сама корзина
    public class Cart
    {
        // Список товаров
        public List<CartItem> Items { get; set; } = new List<CartItem>();

        // Метод добавления товара
        public void AddItem(Product product, int quantity)
        {
            // Проверяем, есть ли уже такой товар в корзине
            var existingItem = Items.FirstOrDefault(i => i.Product.Id == product.Id);

            if (existingItem != null)
            {
                // Если есть - увеличиваем количество
                existingItem.Quantity += quantity;
            }
            else
            {
                // Если нет - добавляем новую запись
                Items.Add(new CartItem { Product = product, Quantity = quantity });
            }
        }

        // Метод удаления товара
        public void RemoveItem(int productId)
        {
            Items.RemoveAll(i => i.Product.Id == productId);
        }

        // Подсчет общей суммы
        public decimal ComputeTotalValue() => Items.Sum(e => e.Product.Price * e.Quantity);

        // Очистка корзины
        public void Clear() => Items.Clear();
    }
}

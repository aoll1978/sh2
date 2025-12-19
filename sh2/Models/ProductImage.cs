namespace sh2.Models
{
    public class ProductImage
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public string ImageUrl { get; set; }
        public int Order { get; set; } // Для сортировки изображений
        public string? Caption { get; set; }
    }
}
namespace sh2.Models
{
    public class CategoryImage
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public string ImageUrl { get; set; }
        public int Order { get; set; } // Для сортировки изображений
        public string? Caption { get; set; }
    }
}
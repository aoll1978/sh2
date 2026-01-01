using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace sh2.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        public int InStock { get; set; } // наличие на складе исправил с bool на int для количества провести миграцию !
        public double? Rating { get; set; }
        public string? Material { get; set; }
        public string? InsertMaterial { get; set; }

        // Убираем '?', добавляем '= new List<...>()'
        public ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
        public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
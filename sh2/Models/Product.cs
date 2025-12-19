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
        public bool InStock { get; set; }
        public double? Rating { get; set; }
        public string? Material { get; set; }
        public string? InsertMaterial { get; set; }
        public ICollection<ProductCategory>? ProductCategories { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<ProductImage>? ProductImages { get; set; }
    }
}
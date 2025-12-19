using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace sh2.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        public int? ParentCategoryId { get; set; }
        public Category? ParentCategory { get; set; }
        public ICollection<Category>? SubCategories { get; set; }
        public ICollection<ProductCategory>? ProductCategories { get; set; }
    }
}
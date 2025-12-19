using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace sh2.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public bool EmailConfirmed { get; set; }
        public ICollection<UserRole>? UserRoles { get; set; }
        public ICollection<Order>? Orders { get; set; }
        public ICollection<Review>? Reviews { get; set; }
    }
}
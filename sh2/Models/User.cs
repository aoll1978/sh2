using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace sh2.Models
{
    // Наследуем от IdentityUser<int> для поддержки ASP.NET Core Identity
    public class User : IdentityUser<int>
    {
        // public int Id { get; set; } // Id уже определён в IdentityUser<int>
        // [Required]
        // public string Email { get; set; } // Email уже определён в IdentityUser
        // [Required]
        // public string PasswordHash { get; set; } // PasswordHash уже определён в IdentityUser
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        // public bool EmailConfirmed { get; set; } // уже есть в IdentityUser
        // public ICollection<UserRole>? UserRoles { get; set; } // Identity сам управляет ролями
        public ICollection<Order>? Orders { get; set; }
        public ICollection<Review>? Reviews { get; set; }
    }
}
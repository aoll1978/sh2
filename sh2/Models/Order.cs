using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace sh2.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int? UserId { get; set; } // null for guest
        public User? User { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public ICollection<OrderItem>? OrderItems { get; set; }
        public Payment? Payment { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using sh2.Models;

namespace sh2.Models
{
    public enum OrderStatus
    {
        New = 0,
        Processing = 1,
        Shipped = 2,
        Delivered = 3,
        Cancelled = 4
    }

    public class Order
    {
        public int Id { get; set; }

        // Связь с User (int, так как User наследуется от IdentityUser<int>)
        public int? UserId { get; set; }
        public User? User { get; set; }

        [Required(ErrorMessage = "Введите ФИО")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Введите адрес")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Введите телефон")]
        [Phone]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Введите Email")]
        [EmailAddress]
        public string Email { get; set; }

        // Дата заказа (важно: в контроллере мы используем OrderDate)
        public DateTime OrderDate { get; set; } = DateTime.Now;

        public OrderStatus Status { get; set; } = OrderStatus.New;

        public decimal TotalAmount { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        // ВЕРНУЛИ PAYMENT, чтобы ApplicationDbContext не ругался
        public Payment? Payment { get; set; }
    }
}
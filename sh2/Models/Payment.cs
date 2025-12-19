using System;

namespace sh2.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public string PaymentProvider { get; set; } // Stripe, PayPal
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaidAt { get; set; }
        public bool IsTest { get; set; }
    }
}
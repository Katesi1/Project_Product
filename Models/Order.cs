using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcLaptop.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? FullName { get; set; }

        [Required]
        public string? PhoneNumber { get; set; }

        [Required]
        public string? Address { get; set; }

        [Required]
        public string? PaymentMethod { get; set; }

        public DateTime OrderDate { get; set; }

        public string? Status { get; set; } = "Đang chờ";

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 0)")]
        public double TotalPrice { get; set; }
    // Thêm khóa ngoại liên kết đến User
        [ForeignKey("User")]
        public string?  UserId { get; set; }
        public User? User { get; set; }

        public ICollection<OrderDetail> orderDetails = default!;
    }
}

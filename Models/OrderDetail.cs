using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MvcLaptop.Models;

public class OrderDetail
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Order")]
    public int OrderId { get; set; }
    public Order? Order { get; set; }

    [ForeignKey("Product")]
    public int ProductId { get; set; }
    public Product? Product { get; set; }

    public int Quantity { get; set; }

    [DataType(DataType.Currency)]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal UnitPrice { get; set; }
}

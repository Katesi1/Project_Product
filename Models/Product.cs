using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace MvcLaptop.Models;

public class Product
{
    public int Id { get; set; }
    public string? Title { get; set; }

    public int CategoryId { get; set; }
    // Quan hệ 1-nhiều với Category
    public Category? Category { get; set; }
    public string? Description { get; set; }
    public int Quantity { get; set; }

    [DataType(DataType.Currency)]
    [Column(TypeName = "decimal(18, 0)")]
    public decimal Price { get; set; }
     // Thuộc tính chỉ hiển thị định dạng giá tiền
    [NotMapped]
    public string FormattedPrice
    {
        get
        {
            return string.Format(CultureInfo.GetCultureInfo("vi-VN"), "{0:C0}", Price);
        }
    }

     // Quan hệ 1-nhiều với ProductImage
    public ICollection<ProductImage>? ProductImages { get; set; } = new List<ProductImage>();

}

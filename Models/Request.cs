using System.ComponentModel.DataAnnotations;

namespace MvcLaptop.Models;

public class LaptopRequest
{
    public int Id { get; set; }

    [StringLength(60, MinimumLength = 3, ErrorMessage = "Tiêu đề phải bằng 3 đến 60 ký tự")]
    [Required(ErrorMessage = "Vui lòng nhập tiêu đề")]
    [Display(Name = "Tiêu đề")]
    public string? Title { get; set; }

    [Display(Name = "Danh mục")]
    public int CategoryId  { get; set; }

    [Display(Name = "Tên danh mục")]
    public string? Name_Category { get; set; }

    [StringLength(20000, MinimumLength = 10, ErrorMessage = "Mô tả phải từ 10 đến 20000 ký tự")]
    [Required(ErrorMessage = "Vui lòng nhập mô tả")]
    [Display(Name = "Mô tả")]
    public string? Description { get; set; }
    
    [Range(1, 1000, ErrorMessage = "Số lượng phải nằm trong khoảng từ 1 đến 1000")]
    [Required(ErrorMessage = "Vui lòng nhập số lượng")]
    [Display(Name = "Số lượng")]
    public int Quantity { get; set; }
    
    [Range(1000, 1000000000, ErrorMessage = "Giá phải nằm trong khoảng từ 1.000 VNĐ đến 1.000.000.000 VNĐ")]
    [DataType(DataType.Currency)]
    [Required(ErrorMessage = "Vui lòng nhập giá")]
    [Display(Name = "Giá (VNĐ)")]
    public decimal Price { get; set; }

    [Display(Name = "Hình ảnh chính")]
    public bool IsMainImage { get; set; } = false;
    // Tùy chỉnh kiểm tra dữ liệu
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Price < 1000 || Price > 1000000000)
        {
            yield return new ValidationResult("Giá phải nằm trong khoảng từ 1.000 VNĐ đến 1.000.000.000 VNĐ", new[] { nameof(Price) });
        }
    }
}
public class OrderRequest
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập họ tên")]
    [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
    public string? FullName { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
    [RegularExpression(@"^(0[1-9]{1}[0-9]{8})$", ErrorMessage = "Số điện thoại không hợp lệ")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng")]
    [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự")]
    public string? Address { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán")]
    [StringLength(50)]
    public string? PaymentMethod { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tổng giá trị đơn hàng")]
    [Range(0, 1000000000, ErrorMessage = "Giá trị đơn hàng phải từ 0 đến 1 tỷ")]
    public decimal TotalPrice { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập ngày đặt hàng")]
    public DateTime OrderDate { get; set; }

    [StringLength(50)]
    public string? UserName { get; set; }

    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string? Email { get; set; }
}
public class UserRequest
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên người dùng")]
    [StringLength(100, ErrorMessage = "Tên người dùng không được vượt quá 100 ký tự")]
    public string? UserName { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập email")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
    [StringLength(100, ErrorMessage = "Mật khẩu không được vượt quá 100 ký tự")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập vai trò")]
    [StringLength(50)]
    public string Role { get; set; } = "User";

    [Display(Name = "Trạng thái tài khoản")]
    public bool IsActive { get; set; } = true;
}

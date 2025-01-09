using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcLaptop.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string? UserName { get; set; }
    
    [Required]
    [EmailAddress]  
    public string? Email { get; set; }
    [Required]
    [StringLength(100)]
    public string? Password { get; set; }
    public User()
    {}
    public User(string userName, string password, string email)
    {
        UserName = userName;
        Email = email;        
        Password = password;
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace MvcLaptop.Models;

public class User : IdentityUser
{
    [PersonalData]
    public string? Name { get; set; }
    [PersonalData]
    public DateTime DOB { get; set; }
    public User()
    { }
    public User(string userName, string email)
    {
        UserName = userName;
        Email = email;
    }

    public ICollection<Order>? Orders {get ; set;} = default!;
};
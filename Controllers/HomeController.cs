using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MvcLaptop.Models;
using MvcLaptop.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
namespace MvcLaptop.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger; //Dependency Injection

    private readonly MvcLaptopContext _context; //Dependency Injection

    public HomeController(MvcLaptopContext context, ILogger<HomeController> logger)
    {
        _context = context;
        _logger = logger;
    }
    public async Task<IActionResult> Index(int? categoryId, string searchString)
    {
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        ViewData["Categories"] = _context.Category!
            .OrderBy(c => c.Name_Category)
            .ToList();
        var products = _context.Product
         .Include(p => p.ProductImages)
         .Include(p => p.Category).AsQueryable();
        if (categoryId.HasValue)
        {
            products = products.Where(p => p.CategoryId == categoryId.Value);
        }

        if (!string.IsNullOrEmpty(searchString))
        {
            products = products.Where(p => p.Title!.ToUpper().Contains(searchString.ToUpper()));
        }

        ViewData["CurrentCategory"] = categoryId;
        ViewData["SearchString"] = searchString;
        return View(await products.ToListAsync());
    }
    public async Task<IActionResult> Product(int? categoryId, string searchString)
    {
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        ViewData["Categories"] = _context.Category!
            .OrderBy(c => c.Name_Category)
            .ToList();
          var products = _context.Product
          .Include(p => p.ProductImages)
          .Include(p => p.Category)
          .AsQueryable();

        if (categoryId.HasValue)
        {
            products = products.Where(p => p.CategoryId == categoryId.Value);
        }

        if (!string.IsNullOrEmpty(searchString))
        {
            products = products.Where(p => p.Title!.ToUpper().Contains(searchString.ToUpper()));
        }

        ViewData["CurrentCategory"] = categoryId;
        ViewData["SearchString"] = searchString;

        return View(await products.ToListAsync());
    }

    public IActionResult About()
    {
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        return View();
    }
    public IActionResult Details(int id)
    {
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        var laptop = _context.Product
            .Include(l => l.ProductImages) // Nạp danh sách ảnh
            .Include(l => l.Category) // Nạp danh mục
            .FirstOrDefault(l => l.Id == id);
        if (laptop == null)
        {
            return NotFound();
        }
        // Truyền danh sách Genres vào ViewData
        ViewData["Categories"] = _context.Category!
            .OrderBy(c => c.Name_Category)
            .ToList();
        return View(laptop);
    }
    public IActionResult About()
    {
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        return View();
    }
    // Action trả về danh sách Genre
    public IActionResult PartialGenres()
    {
        var categories = _context.Category!
            .OrderBy(c => c.Name_Category)
            .ToList();

        return PartialView("_CategoryMenu", categories);
    }
    public IActionResult Login()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string userName, string password)
    {
        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
        {
            ModelState.AddModelError(string.Empty, "Username Và password Không được để trống.");
            return View();
        }
        if (_context?.Users == null)
        {
            ModelState.AddModelError(string.Empty, "Database connection error.");
            return View();
        }
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName && u.Password == password);

        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "username Hoặc password không hợp lệ.");
            return View();
        }

        // Lưu thông tin người dùng vào session
        HttpContext.Session.SetString("UserName", user.UserName);

        return RedirectToAction(nameof(Index));
    }
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction(nameof(Login));
    }
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register([Bind("UserName,Email,Password")] User user)
    {
        if (ModelState.IsValid)
        {
            // Kiểm tra xem tên người dùng đã tồn tại trong cơ sở dữ liệu chưa
            var existingUser = await _context.Users!.FirstOrDefaultAsync(u => u.UserName == user.UserName);
            if (existingUser != null)
            {
                // Nếu tên người dùng đã tồn tại, thêm lỗi vào ModelState
                ModelState.AddModelError("UserName", "Tên tài khoản này đã tồn tại. Vui lòng chọn tên khác.");
                return View(user);  // Trả về view đăng ký với thông báo lỗi
            }
            // Nếu tên người dùng chưa tồn tại, thêm người dùng mới vào cơ sở dữ liệu
            _context.Add(user);
            await _context.SaveChangesAsync();
            // Đăng ký thành công, chuyển hướng tới trang đăng nhập
            return RedirectToAction(nameof(Login));
        }
        return View(user);
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

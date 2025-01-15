using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcLaptop.Data;
using MvcLaptop.Models;
using MvcLaptop.Services;
namespace MvcLaptop.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        // Constructor để inject DbContext
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }
        // Thêm sản phẩm vào giỏ hàng
        public IActionResult AddToCart(int id, int quantity = 1)
        {
            _cartService.AddToCart(id, quantity);
            TempData["Message"] = "Sản phẩm đã được thêm vào giỏ hàng!";
            return RedirectToAction("Details", "Home", new { id = id });
        }

        // Mua sản phẩm ngay lập tức
        public IActionResult BuyNow(int id, int quantity = 1)
        {
            _cartService.AddToCart(id, quantity);
            return RedirectToAction("Index");
        }

        // Hiển thị giỏ hàng (tuỳ chọn)
        public async Task<IActionResult> Index()
        {
            var userName = HttpContext.Session.GetString("UserName");
            ViewData["UserName"] = userName;
            // var cartItems = _cartService.GetCartFromSession();
            // var cartProducts = cartItems.Select(ci => new
            // {
            //     Product = _context.Product.FirstOrDefault(l => l.Id == ci.Key),
            //     Quantity = ci.Value
            // }).ToList();
            var cartProducts = await _cartService.GetCartProductsAsync();
            return View(cartProducts);
        }
        public IActionResult UpdateQuantity(int id, int quantity)
        {
            var cartItems = _cartService.GetCartFromSession();
            if (cartItems.ContainsKey(id))
            {
                cartItems[id] = quantity;
            }
            _cartService.SaveCartToSession(cartItems);
            return Json(new { success = true });
        }
        public IActionResult GetCartItemCount()
        {
            var cartItems = _cartService.GetCartFromSession();

            var totalQuantity = cartItems.Values.Sum();

            return Json(new { totalQuantity });
        }
        public IActionResult RemoveFromCart(int id)
        {
            _cartService.RemoveFromCart(id);
            TempData["Message"] = "Sản phẩm đã được xóa khỏi giỏ hàng!";
            return RedirectToAction("Index");
        }
        // Thanh toán giỏ hàng
        [HttpPost]
        public async Task<IActionResult> Checkout(Dictionary<int, int> quantities)
        {
             var userName = User.Identity!.Name;
            // var cartItems = _cartService.GetCartFromSession();
            if (User.Identity.IsAuthenticated)
            {
                // Gán thông tin vào ViewData
                ViewData["UserName"] = userName;
            }
            if (string.IsNullOrEmpty(userName))
            {
                TempData["Message"] = "Bạn cần đăng nhập để tiếp tục đặt hàng.";
                TempData["ShowLoginModal"] = true;
                return RedirectToAction("Index");
            }
            // foreach (var item in quantities)
            // {
            //     int productId = item.Key;
            //     int quantityInCart = item.Value;

            //     var product = _context.Product.FirstOrDefault(l => l.Id == productId);

            //     if (product != null && product.Quantity < quantityInCart)
            //     {
            //         TempData["Error"] = $"Sản phẩm {product.Title} không đủ số lượng trong kho!";
            //         return RedirectToAction("Index");
            //     }
            // }
            // var cartProducts = cartItems.Select(ci => new
            // {
            //     Product = _context.Product.FirstOrDefault(l => l.Id == ci.Key),
            //     Quantity = ci.Value
            // }).ToList();
            // var totalPrice = _cartService.CalculateTotalPrice(cartItems);
            var isStockAvailable = await _cartService.CheckProductStockAsync(quantities);
            if (!isStockAvailable)
            {
                TempData["Error"] = "Một số sản phẩm không đủ số lượng trong kho!";
                return RedirectToAction("Index");
            }

            var cartProducts = await _cartService.GetCartProductsAsync();
            var totalPrice = _cartService.CalculateTotalPrice(quantities);
            ViewData["CartItems"] = cartProducts;
            ViewData["TotalPrice"] = totalPrice;
            return View(new Order());
        }
        [HttpPost]
public async Task<IActionResult> ProcessCheckout(Order order,User user, string paymentMethod)
{
    var userName = User.Identity!.Name;
    var email =  User.Identity!.Name;
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    Console.WriteLine($"UserId from Claims: {userId}");

    if (string.IsNullOrEmpty(userId))
    {
        TempData["Error"] = "Bạn cần đăng nhập để tiếp tục đặt hàng.";
        return RedirectToAction("Index");
    }
    
    Console.WriteLine(userName);
    Console.WriteLine(email);
    Console.WriteLine(userId);

    // if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(email) || userId == null)
    // {
    //     TempData["Error"] = "Bạn cần đăng nhập để tiếp tục đặt hàng.";
    //     return RedirectToAction("Index");
    // }

    // Kiểm tra số lượng sản phẩm trong kho
    var cartItems = _cartService.GetCartFromSession();
    // var isStockAvailable = await _cartService.CheckProductStockAsync(cartItems);
    // if (!isStockAvailable)
    // {
    //     TempData["Error"] = "Một số sản phẩm không đủ số lượng trong kho!";
    //     return RedirectToAction("Index");
    // }

    // Xử lý thông tin đơn hàng
    order.UserId = userId!;
    order.OrderDate = DateTime.Now;
    order.TotalPrice = _cartService.CalculateTotalPrice(cartItems);

    // // Kiểm tra phương thức thanh toán
    // if (paymentMethod == "Payment") 
    // {
    //     // Chuyển hướng đến trang thanh toán (Pay)
    //     return RedirectToAction("Pay", new { orderId = order.Id });
    // }
    // else if (paymentMethod == "COD") 
    // {
    //     // Lưu đơn hàng vào database và chuyển đến trang xác nhận thanh toán
        
    // }

        await _cartService.ProcessCheckoutAsync(order, user.UserName!, email!);
        TempData["Message"] = "Đơn hàng của bạn đã được ghi nhận và sẽ giao tận nơi.";
        return RedirectToAction("Confirmation", new { orderId = order.Id });

    //return RedirectToAction("Index");
    }
        public IActionResult Confirmation(int orderId)
        {
            ViewData["Message"] = TempData["Message"];
            return View();
        }
    }
}

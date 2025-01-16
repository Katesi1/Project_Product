using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcLaptop.Authorization;
using MvcLaptop.Data;
using MvcLaptop.Models;
using MvcLaptop.Utils.Constants;
using System.Linq;
using System.Threading.Tasks;

namespace MvcLaptop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderManageController : Controller
    {
        private readonly MvcLaptopContext _context;

        public OrderManageController(MvcLaptopContext context)
        {
            _context = context;
        }
        [ClaimRequirement(FunctionCode.SYSTEM_USER, CommandCode.VIEW)]
        public IActionResult Index()
        {
            var order = _context.Orders!.ToList();
            return View(order);
        }
        [ClaimRequirement(FunctionCode.SYSTEM_USER, CommandCode.VIEW)]
        // Trang chi tiết Category (GET)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders!
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // Trang tạo Category mới (GET)
        // public IActionResult Create()
        // {
        //     return View();
        // }

        // // Trang tạo Category mới (POST)
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // [ClaimRequirement(FunctionCode.SYSTEM_USER, CommandCode.CREATE)]
        // public async Task<IActionResult> Create([Bind("CategoryId, Name_Category, Description")] Category category)
        // {
        //     if (ModelState.IsValid)
        //     {
        //         _context.Add(category);
        //         await _context.SaveChangesAsync();
        //         return RedirectToAction(nameof(Index));
        //     }
        //     return View(category);
        // }

        // Trang sửa Category (GET)
        [ClaimRequirement(FunctionCode.SYSTEM_USER, CommandCode.UPDATE)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders!.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // Trang sửa Category (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, FullName, PhoneNumber,Address,PaymentMethod, OrderDate, Status, TotalPrice,  UserName")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingOrder = await _context.Orders!.FindAsync(id);
                    if (existingOrder == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật các trường
                    existingOrder.Status = order.Status;

                    _context.Update(existingOrder);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // Trang xóa Category (GET)
        [ClaimRequirement(FunctionCode.SYSTEM_USER, CommandCode.DELETE)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders!
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // Trang xóa Category (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders!.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders!.Any(e => e.Id == id);
        }
    }
}
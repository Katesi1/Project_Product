using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcLaptop.Data;
using MvcLaptop.Models;
using AutoMapper;
using MvcLaptop.Services;
using Microsoft.AspNetCore.Authorization;

namespace MvcLaptop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Administrator,Manager")]
    public class LaptopsController : Controller
    {
        private readonly ILaptopService _laptopService;
        public LaptopsController(ILaptopService laptopService)
        {
            _laptopService = laptopService;
        }

        // GET: Laptops
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            var userName = HttpContext.Session.GetString("UserName");
            ViewData["UserName"] = userName;
            ViewData["SearchString"] = searchString;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["PriceSortParm"] = sortOrder == "Price" ? "price_desc" : "Price";
            ViewData["QuantitySortParm"] = sortOrder == "Quantity" ? "Quantity_desc" : "Quantity";
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;
            // Lấy danh sách laptop
            return View(await _laptopService.GetLaptops(sortOrder, currentFilter, searchString, pageNumber));
        }

        // GET: Laptops/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var userName = HttpContext.Session.GetString("UserName");
            ViewData["UserName"] = userName;
            if (id == null)
            {
                return NotFound();
            }

            var laptop = await _laptopService.GetLaptopById(id.Value);
            if (laptop == null)
            {
                return NotFound();
            }
            return View(laptop);
        }

        // GET: Laptops/Create
        public async Task<IActionResult> Create()
        {
            var userName = HttpContext.Session.GetString("UserName");
            ViewData["UserName"] = userName;
            ViewBag.SuccessMessage = null; 
            ViewBag.Categories = new SelectList(await _laptopService.GetCategories(), "CategoryId", "Name_Category");
            return View(new LaptopRequest());
            // ViewBag.Categories = new SelectList(await _laptopService.GetCategories(), "CategoryId", "Name_Category");
            // return View();
        }

        // POST: Laptops/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LaptopRequest request, IFormFile MainImage)
        {
            if (ModelState.IsValid)
            {
                var result = await _laptopService.Create(request, MainImage);
                if (result != null) {
                    ViewBag.SuccessMessage = "Sản phẩm đã được tạo thành công!";
                }
                    return RedirectToAction(nameof(Index));
                    
            }
             foreach (var modelStateKey in ModelState.Keys)
            {
                var modelStateVal = ModelState[modelStateKey];
                foreach (var error in modelStateVal!.Errors)
                {
                    Console.WriteLine($"Key: {modelStateKey}, Error: {error.ErrorMessage}");
                }
            }
            ViewBag.Categories = new SelectList(await _laptopService.GetCategories(), "CategoryId", "Name_Category", request.CategoryId);
            return View(request);

        }

        // GET: Laptops/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var laptop = await _laptopService.GetLaptopById(id!.Value);
            if (laptop == null)
            {
                return NotFound();
            }
            ViewBag.Categories = new SelectList(await _laptopService.GetCategories(), "CategoryId", "Name_Category", laptop.CategoryId);
            return View(laptop);
        }

        // POST: Laptops/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LaptopViewModel laptop, IFormFile? MainImage)
        {
            if (!ModelState.IsValid)
            {
                // Nạp danh sách danh mục nếu ModelState không hợp lệ
                ViewBag.Categories = new SelectList(await _laptopService.GetCategories(), "CategoryId", "Name_Category", laptop.CategoryId);
                return View(laptop);
            }
            // Gọi service để cập nhật sản phẩm và xử lý ảnh
            var result = await _laptopService.Update(id, laptop, MainImage);
            if (!result)
            {
                ModelState.AddModelError("", "Unable to update product. Please try again.");
                ViewBag.Categories = new SelectList(await _laptopService.GetCategories(), "CategoryId", "Name_Category", laptop.CategoryId);
                return View(laptop);
            }

            // Lấy lại sản phẩm để hiển thị ảnh mới (nếu đã được cập nhật)
            var updatedProduct = await _laptopService.GetLaptopById(id);

            ViewBag.Categories = new SelectList(await _laptopService.GetCategories(), "CategoryId", "Name_Category", updatedProduct.CategoryId);
            return View(updatedProduct); // Hiển thị lại trang Edit với dữ liệu cập nhật
        }

        // GET: Laptops/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var laptop = await _laptopService.GetLaptopById(id.Value);
            if (laptop == null)
            {
                return NotFound();
            }

            return View(laptop);
        }

        // POST: Laptops/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _laptopService.Delete(id);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Category(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            var userName = HttpContext.Session.GetString("UserName");
            ViewData["UserName"] = userName;
            ViewData["SearchString"] = searchString;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;
            // Lấy danh sách laptop
            return View(await _laptopService.GetCategory(sortOrder, currentFilter, searchString, pageNumber));
        }
        public IActionResult Create_cate()
        {
            var userName = HttpContext.Session.GetString("UserName");
            ViewData["UserName"] = userName;
            ViewBag.SuccessMessage = null;
            return View(new LaptopRequest());
            // ViewBag.Categories = new SelectList(await _laptopService.GetCategories(), "CategoryId", "Name_Category");
            // return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create_cate(Category request)
        {
            if (ModelState.IsValid)
            {
                var result = await _laptopService.Create_cate(request);
                if (result != null)
                {
                    ViewBag.SuccessMessage = "Danh mục đã được tạo thành công.";
                    return RedirectToAction(nameof(Category));
                }

                ViewBag.ErrorMessage = "Đã xảy ra lỗi khi tạo danh mục. Vui lòng thử lại.";
            }

            // Ghi log lỗi để kiểm tra ModelState
            foreach (var modelStateKey in ModelState.Keys)
            {
                var modelStateVal = ModelState[modelStateKey];
                foreach (var error in modelStateVal!.Errors)
                {
                    Console.WriteLine($"Key: {modelStateKey}, Error: {error.ErrorMessage}");
                }
            }

            ViewBag.ErrorMessage = "Dữ liệu nhập không hợp lệ.";
            return View(request);
        }
    }
}

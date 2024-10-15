using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Elixir.Models;
 using X.PagedList;
using Microsoft.Extensions.Hosting;
namespace Elixir.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ADProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ADProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/ADProducts
       

    public async Task<IActionResult> Index(int? page)
    {
        var pageNumber = page ?? 1;
        pageNumber = pageNumber < 1 ? 1 : pageNumber; // Đảm bảo pageNumber luôn là một số dương hợp lệ
        var pageSize = 10; // Số mục trên mỗi trang
        var products = await _context.Products.Include(p => p.Category).ToPagedListAsync(pageNumber, pageSize);

        return View(products);
    }


    // GET: Admin/ADProducts/Details/5
    public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Discounts)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/ADProducts/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["DiscountId"] = new SelectList(_context.Discount, "Id", "Name");
            return View();
        }

        // POST: Admin/ADProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile productImage)
        {
            if (ModelState.IsValid)
            {
                if (productImage != null && productImage.Length > 0)
                {
                    var fileName = Path.GetFileName(productImage.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/product", fileName);

                    // Đảm bảo thư mục tồn tại
                    if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    }

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await productImage.CopyToAsync(stream);
                    }

                    product.ImageUrl = "/img/product/" + fileName; // Cập nhật đường dẫn hình ảnh
                }

                try
                {
                    _context.Add(product);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Ghi log lỗi
                    ModelState.AddModelError("", $"Không thể lưu sản phẩm. Chi tiết lỗi: {ex.Message}");
                    // Ghi chi tiết lỗi ra log (ví dụ: sử dụng logger)
                    // _logger.LogError(ex, "Error creating product");
                }
            }

            // Ghi lại các lỗi trong ModelState
            var errorMessages = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            foreach (var errorMessage in errorMessages)
            {
                ModelState.AddModelError("", errorMessage);
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }



        // GET: Admin/ADProducts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            ViewData["DiscountId"] = new SelectList(_context.Discount, "Id", "Name", product.DiscountId);

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Description,ShortDescription,CategoryId,QuantityInStock,Status,DiscountId,HomeFlag,ImageUrl")] Product product, IFormFile newImage)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (newImage != null && newImage.Length > 0)
                    {
                        var fileName = Path.GetFileName(newImage.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/product", fileName);

                        // Đảm bảo thư mục tồn tại
                        if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                        }

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await newImage.CopyToAsync(stream);
                        }

                        product.ImageUrl = "/img/product/" + fileName; // Cập nhật đường dẫn hình ảnh mới
                    }
                    else
                    {
                        // Giữ nguyên đường dẫn hình ảnh cũ
                        var existingProduct = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                        if (existingProduct != null)
                        {
                            product.ImageUrl = existingProduct.ImageUrl;
                        }
                    }

                    _context.Update(product); // Cập nhật thông tin sản phẩm
                    await _context.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // Nếu ModelState không hợp lệ, hiển thị lại form với thông báo lỗi
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            ViewData["DiscountId"] = new SelectList(_context.Discount, "Id", "Name", product.DiscountId);
            return View(product);
        }





        // GET: Admin/ADProducts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Discounts)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/ADProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}

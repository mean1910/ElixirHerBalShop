using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Elixir.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace Elixir.Controllers
{
    [Area("Admin")]

    [Authorize(Roles = SD.Role_Admin)] // Chỉ cho phép người dùng có vai trò "Admin" truy cập
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index(int? page)
        {
            var pageNumber = page ?? 1;
            pageNumber = pageNumber < 1 ? 1 : pageNumber; // Đảm bảo pageNumber luôn là một số dương hợp lệ
            var pageSize = 10; // Số mục trên mỗi trang
            var user = _userManager.Users.ToPagedList(pageNumber, pageSize);
            
            return View(user);
        }

        // Action để khóa tài khoản người dùng
        public IActionResult Lock(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;
            if (user != null)
            {
                user.LockoutEnd = DateTimeOffset.MaxValue; // Khóa tài khoản vĩnh viễn
                var result = _userManager.UpdateAsync(user).Result;
                if (result.Succeeded)
                {
                    // Xử lý thành công
                }
                else
                {
                    // Xử lý khi không thành công
                }
            }
            return RedirectToAction("Index");
        }

        // Action để mở khóa tài khoản người dùng
        public IActionResult Unlock(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;
            if (user != null)
            {
                user.LockoutEnd = null; // Mở khóa tài khoản
                var result = _userManager.UpdateAsync(user).Result;
                if (result.Succeeded)
                {
                    // Xử lý thành công
                }
                else
                {
                    // Xử lý khi không thành công
                }
            }
            return RedirectToAction("Index");
        }

        // Action để xem thông tin sơ bộ của người dùng
        public IActionResult Details(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;
            return View(user);
        }
    }
}

// Controllers/ShoppingCartController.cs
using Microsoft.AspNetCore.Mvc;
using Elixir.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Linq;
using System;
using Elixir.Extensions;
using Elixir.ViewModels;

public class ShoppingCartController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ShoppingCartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> AddToCart(int productId, int quantity)
    {
        var product = await _context.Products.FindAsync(productId);

        if (product == null)
        {
            return NotFound();
        }

        // Adjust quantity based on the unit
        var adjustedQuantity = quantity * product.unit;

        var cartItem = new CartItem
        {
            ProductId = productId,
            Name = product.Name,
            Quantity = adjustedQuantity,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            Unit = product.unit
        };

        var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
        cart.AddItem(cartItem);

        HttpContext.Session.SetObjectAsJson("Cart", cart);

        return RedirectToAction("Index");
    }


    [HttpPost]
    public IActionResult UpdateQuantity(int productId, int quantity)
    {
        var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
        if (cart != null)
        {
            cart.UpdateQuantity(productId, quantity);
            HttpContext.Session.SetObjectAsJson("Cart", cart);
        }

        return RedirectToAction("Index");
    }

    public IActionResult RemoveFromCart(int productId)
    {
        var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
        if (cart != null)
        {
            cart.RemoveItem(productId);
            HttpContext.Session.SetObjectAsJson("Cart", cart);
        }
        return RedirectToAction("Index");
    }

    public IActionResult Index()
    {
        var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
        ViewBag.TotalQuantity = cart.GetTotalQuantity();
        ViewBag.TotalPrice = cart.GetTotalPrice();
        return View(cart);
    }




    public async Task<IActionResult> Checkout()
    {
        // Lấy giỏ hàng từ Session
        var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");

        // Kiểm tra xem giỏ hàng có null hoặc không có sản phẩm nào không
        if (cart == null || !cart.Items.Any())
        {
            return RedirectToAction("Index");
        }

        // Lấy thông tin người dùng đang đăng nhập
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            // Nếu không có người dùng đăng nhập, chuyển hướng đến trang đăng nhập
            return RedirectToAction("Login", "Account");
        }

        // Tạo CheckoutViewModel để chứa dữ liệu cho view
        var viewModel = new CheckoutViewModel
        {
            Order = new Order
            {
                // Gán thông tin từ ApplicationUser vào Order
                UserId = user.Id,
                ApplicationUser = user,
                OrderDate = DateTime.UtcNow,
                TotalPrice = cart.Items.Sum(i => i.TotalPrice)
            },
            ShippingMethods = _context.ShippingMethods.ToList(),
            Vouchers = _context.Vouchers.Where(v => v.IsActive && v.ExpiryDate > DateTime.UtcNow).ToList()
        };

        // Gán thông tin người dùng vào view model
        viewModel.Order.ApplicationUser = user;

        return View(viewModel);
    }



    [HttpPost]
    public async Task<IActionResult> Checkout(CheckoutViewModel viewModel)
    {
        var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
        if (cart == null || !cart.Items.Any())
        {
            return RedirectToAction("Index");
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        viewModel.Order.UserId = user.Id;
        viewModel.Order.OrderDate = DateTime.UtcNow;
        viewModel.Order.TotalPrice = cart.Items.Sum(i => i.TotalPrice);

        viewModel.Order.OrderDetails = cart.Items.Select(i => new OrderDetail
        {
            ProductId = i.ProductId,
            Quantity = i.Quantity,
            Price = i.Price
        }).ToList();

        viewModel.Order.ShippingMethod = _context.ShippingMethods.Find(viewModel.Order.ShippingMethodId);
        if (viewModel.Order.VoucherId.HasValue)
        {
            viewModel.Order.Voucher = _context.Vouchers.Find(viewModel.Order.VoucherId.Value);
        }

        _context.Orders.Add(viewModel.Order);
        await _context.SaveChangesAsync();

        HttpContext.Session.Remove("Cart");

        var orderCompletedViewModel = new OrderCompletedViewModel
        {
            OrderId = viewModel.Order.Id,
            OrderDate = viewModel.Order.OrderDate,
            TotalPrice = viewModel.Order.TotalPrice,
            ShippingMethodName = viewModel.Order.ShippingMethod.Name,
            VoucherCode = viewModel.Order.Voucher?.Code ?? "Không có",
            Notes = viewModel.Order.Notes
        };

        return View("OrderCompleted", orderCompletedViewModel);
    }

}


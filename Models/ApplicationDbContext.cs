using Microsoft.EntityFrameworkCore;
using Elixir.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Elixir.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>

    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>
       options) : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostImage> PostImages { get; set; }
        public DbSet<Elixir.Models.Discount> Discount { get; set; } = default!;
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<ShippingMethod> ShippingMethods { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }

    }
}

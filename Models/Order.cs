using Elixir.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

public class Order
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalPrice { get; set; }
    public string? Notes { get; set; }

    // Liên kết với ApplicationUser
    [ForeignKey("UserId")]
    [ValidateNever]
    public ApplicationUser ApplicationUser { get; set; }

    // Liên kết với ShippingMethod
    public int ShippingMethodId { get; set; }
    [ForeignKey("ShippingMethodId")]
    [ValidateNever]
    public ShippingMethod ShippingMethod { get; set; }

    // Liên kết với Voucher (có thể nullable nếu không sử dụng voucher)
    public int? VoucherId { get; set; }
    [ForeignKey("VoucherId")]
    [ValidateNever]
    public Voucher Voucher { get; set; }

    public List<OrderDetail> OrderDetails { get; set; }
}

using Elixir.Models;

namespace Elixir.ViewModels
{
    public class CheckoutViewModel
    {
        public Order Order { get; set; }
        public List<ShippingMethod> ShippingMethods { get; set; }
        public List<Voucher> Vouchers { get; set; }
    }

}

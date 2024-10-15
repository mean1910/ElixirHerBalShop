namespace Elixir.ViewModels
{
    public class OrderCompletedViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string ShippingMethodName { get; set; }
        public string VoucherCode { get; set; }
        public string Notes { get; set; }
    }
}

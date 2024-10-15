
public class CartItem
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; } // Quantity in terms of units (e.g., grams)
    public decimal Price { get; set; }
    public string ImageUrl { get; set; }
    public int Unit { get; set; } // The unit of the product (e.g., 100 grams)

    public decimal TotalPrice => Price * Quantity / Unit; // Calculate the total price based on the unit
}

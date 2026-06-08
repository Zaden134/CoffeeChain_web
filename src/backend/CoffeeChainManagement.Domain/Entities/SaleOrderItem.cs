namespace CoffeeChainManagement.Domain.Entities;

// SaleOrderItem ghi chi tiet tung mon trong hoa don de tinh tong tien va dong bo POS.
public sealed class SaleOrderItem
{
    public required Guid ProductId { get; set; }
    public required string ProductName { get; set; }
    public required int Quantity { get; set; }
    public required decimal UnitPrice { get; set; }
    public decimal LineTotal => Quantity * UnitPrice;
}

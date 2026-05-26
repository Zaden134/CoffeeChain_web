namespace CoffeeChainManagement.Domain.Entities;

// SaleOrderItem ghi chi tiet tung mon trong hoa don de tinh tong tien va dong bo POS.
public sealed class SaleOrderItem
{
    public required Guid ProductId { get; init; }
    public required string ProductName { get; init; }
    public required int Quantity { get; init; }
    public required decimal UnitPrice { get; init; }
    public decimal LineTotal => Quantity * UnitPrice;
}

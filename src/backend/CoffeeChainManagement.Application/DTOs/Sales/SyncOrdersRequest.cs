namespace CoffeeChainManagement.Application.DTOs.Sales;

public class SaleOrderItemDto
{
    public required Guid ProductId { get; set; }
    public required string ProductName { get; set; }
    public required int Quantity { get; set; }
    public required decimal UnitPrice { get; set; }
}

public class SaleOrderDto
{
    public required Guid Id { get; set; }
    public required Guid BranchId { get; set; }
    public required Guid EmployeeId { get; set; }
    public required string PaymentMethod { get; set; }
    public required string Status { get; set; }
    public string? PromotionCode { get; set; }
    public decimal DiscountAmount { get; set; }
    public required DateTime CreatedAtUtc { get; set; }
    public List<SaleOrderItemDto> Items { get; set; } = [];
}

public class SyncOrdersRequest
{
    public List<SaleOrderDto> Orders { get; set; } = [];
}

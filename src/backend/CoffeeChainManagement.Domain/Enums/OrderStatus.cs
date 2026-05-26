namespace CoffeeChainManagement.Domain.Enums;

// OrderStatus mo ta vong doi don hang de sau nay map thang sang POS va dashboard.
public enum OrderStatus
{
    Draft = 1,
    Paid = 2,
    Cancelled = 3,
    Refunded = 4
}

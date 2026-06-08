namespace CoffeeChainManagement.Domain.Common;

// BaseEntity gom cac thuoc tinh dung chung de tat ca entity sau nay mo rong dong nhat.
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
}

using CoffeeChainManagement.Domain.Common;

namespace CoffeeChainManagement.Domain.Entities;

// Promotion dat san cac chuong trinh khuyen mai cho giai doan phat trien tiep theo.
public sealed class Promotion : BaseEntity
{
    public required string Name { get; init; }
    public required decimal DiscountPercent { get; init; }
    public required DateOnly StartDate { get; init; }
    public required DateOnly EndDate { get; init; }
    public bool IsActive { get; init; } = true;
}

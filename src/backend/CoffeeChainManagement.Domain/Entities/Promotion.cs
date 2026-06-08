using CoffeeChainManagement.Domain.Common;

namespace CoffeeChainManagement.Domain.Entities;

// Promotion dat san cac chuong trinh khuyen mai cho giai doan phat trien tiep theo.
public sealed class Promotion : BaseEntity
{
    public required string Name { get; set; }
    public required decimal DiscountPercent { get; set; }
    public required DateOnly StartDate { get; set; }
    public required DateOnly EndDate { get; set; }
    public bool IsActive { get; set; } = true;
}

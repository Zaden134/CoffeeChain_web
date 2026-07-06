using CoffeeChainManagement.Domain.Common;

namespace CoffeeChainManagement.Domain.Entities;

// Promotion dat san cac chuong trinh khuyen mai cho giai doan phat trien tiep theo.
public sealed class Promotion : BaseEntity
{
    public required string Code { get; set; }
    public required string Name { get; set; }
    public decimal? DiscountPercent { get; set; }
    public decimal? DiscountAmount { get; set; }
    public required DateOnly StartDate { get; set; }
    public required DateOnly EndDate { get; set; }
    public Guid? BranchId { get; set; }
    public string? CustomerSegment { get; set; }
    public string? CustomerPhone { get; set; }
    public bool IsActive { get; set; } = true;
}

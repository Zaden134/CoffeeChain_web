using CoffeeChainManagement.Domain.Common;
using CoffeeChainManagement.Domain.Enums;

namespace CoffeeChainManagement.Domain.Entities;

// RecruitmentRequest luu de xuat tuyen nguoi tu quan ly chi nhanh gui admin phe duyet.
public sealed class RecruitmentRequest : BaseEntity
{
    public required Guid BranchId { get; set; }
    public required Guid RequestedByEmployeeId { get; set; }
    public required string PositionTitle { get; set; }
    public required int Quantity { get; set; }
    public required string Reason { get; set; }
    public RecruitmentRequestStatus Status { get; set; } = RecruitmentRequestStatus.Pending;
    public string? AdminNote { get; set; }
    public Guid? ReviewedByEmployeeId { get; set; }
    public DateTime? ReviewedAtUtc { get; set; }
}

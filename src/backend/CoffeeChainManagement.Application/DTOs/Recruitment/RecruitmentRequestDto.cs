namespace CoffeeChainManagement.Application.DTOs.Recruitment;

// RecruitmentRequestDto la du lieu danh sach yeu cau tuyen dung cho manager va admin.
public sealed record RecruitmentRequestDto(
    Guid Id,
    Guid BranchId,
    string BranchName,
    Guid RequestedByEmployeeId,
    string RequestedByName,
    string PositionTitle,
    int Quantity,
    string Reason,
    string Status,
    string? AdminNote,
    DateTime CreatedAtUtc,
    DateTime? ReviewedAtUtc);

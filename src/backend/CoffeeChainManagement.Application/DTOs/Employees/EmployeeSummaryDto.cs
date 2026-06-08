namespace CoffeeChainManagement.Application.DTOs.Employees;

// EmployeeSummaryDto la view model danh sach nhan vien cho man hinh quan tri.
public sealed record EmployeeSummaryDto(
    Guid Id,
    string Username,
    string FullName,
    string Email,
    string Role,
    Guid? BranchId,
    string BranchName,
    bool IsActive);

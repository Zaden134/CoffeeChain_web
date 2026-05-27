using CoffeeChainManagement.Application.DTOs.Employees;

namespace CoffeeChainManagement.Application.Interfaces;

// IEmployeeService dinh nghia CRUD nhan vien voi rule theo vai tro va chi nhanh.
public interface IEmployeeService
{
    Task<IReadOnlyCollection<EmployeeSummaryDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<EmployeeSummaryDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<EmployeeSummaryDto> CreateAsync(UpsertEmployeeRequestDto request, CancellationToken cancellationToken = default);
    Task<EmployeeSummaryDto> UpdateAsync(Guid id, UpsertEmployeeRequestDto request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

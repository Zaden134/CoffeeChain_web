using CoffeeChainManagement.Application.DTOs.Employees;
using CoffeeChainManagement.Application.Interfaces;
using CoffeeChainManagement.Domain.Entities;
using CoffeeChainManagement.Domain.Enums;
using CoffeeChainManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CoffeeChainManagement.Infrastructure.Services;

// PostgresEmployeeService xu ly CRUD nhan vien va ap dung rule theo role/branch.
internal sealed class PostgresEmployeeService(
    CoffeeChainDbContext dbContext,
    ICurrentUserContext currentUser,
    IPasswordHasher<Employee> passwordHasher,
    IAuditLogService auditLogService) : IEmployeeService
{
    public async Task<IReadOnlyCollection<EmployeeSummaryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        EnsureAuthenticated();

        IQueryable<Employee> query = dbContext.Employees.AsNoTracking();
        if (currentUser.Role == UserRole.BranchManager)
        {
            query = query.Where(employee => employee.BranchId == currentUser.BranchId);
        }

        var branches = await dbContext.Branches.AsNoTracking().ToDictionaryAsync(branch => branch.Id, branch => branch.Name, cancellationToken);
        var employees = await query.OrderBy(employee => employee.FullName).ToListAsync(cancellationToken);

        return employees.Select(employee => MapEmployee(employee, branches)).ToArray();
    }

    public async Task<EmployeeSummaryDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        EnsureAuthenticated();

        var employee = await dbContext.Employees.AsNoTracking().SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (employee is null || !CanAccessEmployee(employee))
        {
            return null;
        }

        var branches = await dbContext.Branches.AsNoTracking().ToDictionaryAsync(branch => branch.Id, branch => branch.Name, cancellationToken);
        return MapEmployee(employee, branches);
    }

    public async Task<EmployeeSummaryDto> CreateAsync(UpsertEmployeeRequestDto request, CancellationToken cancellationToken = default)
    {
        EnsureAuthenticated();
        var role = ParseRole(request.Role);
        EnsureCanManageEmployeeRole(role, request.BranchId, null);

        if (await dbContext.Employees.AnyAsync(employee => employee.Username == request.Username.Trim(), cancellationToken))
        {
            throw new InvalidOperationException("Username already exists.");
        }

        if (await dbContext.Employees.AnyAsync(employee => employee.Email == request.Email.Trim(), cancellationToken))
        {
            throw new InvalidOperationException("Email already exists.");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            throw new InvalidOperationException("Password is required when creating an employee.");
        }

        var employee = new Employee
        {
            Username = request.Username.Trim(),
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim(),
            PasswordHash = string.Empty,
            Role = role,
            BranchId = request.BranchId,
            IsActive = request.IsActive
        };
        employee.PasswordHash = passwordHasher.HashPassword(employee, request.Password);

        dbContext.Employees.Add(employee);
        await dbContext.SaveChangesAsync(cancellationToken);
        await auditLogService.WriteAsync(
            "EMPLOYEE_CREATE",
            nameof(Employee),
            $"Created employee {employee.Username}",
            true,
            employee.Id,
            employee.Username,
            employee.BranchId,
            employee.Id,
            cancellationToken);

        var branches = await dbContext.Branches.AsNoTracking().ToDictionaryAsync(branch => branch.Id, branch => branch.Name, cancellationToken);
        return MapEmployee(employee, branches);
    }

    public async Task<EmployeeSummaryDto> UpdateAsync(Guid id, UpsertEmployeeRequestDto request, CancellationToken cancellationToken = default)
    {
        EnsureAuthenticated();

        var employee = await dbContext.Employees.SingleOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Employee not found.");

        var role = ParseRole(request.Role);
        EnsureCanManageEmployeeRole(role, request.BranchId, employee);
        if (employee.Id == currentUser.UserId && !request.IsActive)
        {
            throw new InvalidOperationException("You cannot deactivate your own account.");
        }

        if (await dbContext.Employees.AnyAsync(item => item.Id != id && item.Username == request.Username.Trim(), cancellationToken))
        {
            throw new InvalidOperationException("Username already exists.");
        }

        if (await dbContext.Employees.AnyAsync(item => item.Id != id && item.Email == request.Email.Trim(), cancellationToken))
        {
            throw new InvalidOperationException("Email already exists.");
        }

        employee.Username = request.Username.Trim();
        employee.FullName = request.FullName.Trim();
        employee.Email = request.Email.Trim();
        employee.Role = role;
        employee.BranchId = request.BranchId;
        employee.IsActive = request.IsActive;
        employee.UpdatedAtUtc = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            employee.PasswordHash = passwordHasher.HashPassword(employee, request.Password);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        await auditLogService.WriteAsync(
            "EMPLOYEE_UPDATE",
            nameof(Employee),
            $"Updated employee {employee.Username}",
            true,
            employee.Id,
            employee.Username,
            employee.BranchId,
            employee.Id,
            cancellationToken);

        var branches = await dbContext.Branches.AsNoTracking().ToDictionaryAsync(branch => branch.Id, branch => branch.Name, cancellationToken);
        return MapEmployee(employee, branches);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        EnsureAuthenticated();

        var employee = await dbContext.Employees.SingleOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Employee not found.");

        if (employee.Id == currentUser.UserId)
        {
            throw new InvalidOperationException("You cannot deactivate your own account.");
        }

        EnsureCanManageEmployeeRole(employee.Role, employee.BranchId, employee);
        employee.IsActive = false;
        employee.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        await auditLogService.WriteAsync(
            "EMPLOYEE_DELETE",
            nameof(Employee),
            $"Deactivated employee {employee.Username}",
            true,
            employee.Id,
            employee.Username,
            employee.BranchId,
            employee.Id,
            cancellationToken);
    }

    private void EnsureAuthenticated()
    {
        if (!currentUser.IsAuthenticated || (currentUser.Role != UserRole.Administrator && currentUser.Role != UserRole.BranchManager))
        {
            throw new UnauthorizedAccessException("You do not have permission to access employees.");
        }
    }

    private bool CanAccessEmployee(Employee employee)
        => currentUser.Role == UserRole.Administrator
            || (currentUser.Role == UserRole.BranchManager && employee.BranchId == currentUser.BranchId);

    private void EnsureCanManageEmployeeRole(UserRole targetRole, Guid? branchId, Employee? existingEmployee)
    {
        if (currentUser.Role == UserRole.Administrator)
        {
            return;
        }

        if (currentUser.Role != UserRole.BranchManager || branchId != currentUser.BranchId)
        {
            throw new UnauthorizedAccessException("You do not have permission to manage this employee.");
        }

        if (targetRole is UserRole.Administrator or UserRole.BranchManager)
        {
            throw new UnauthorizedAccessException("Branch managers can only manage cashier or warehouse staff.");
        }

        if (existingEmployee is not null && existingEmployee.Role is UserRole.Administrator or UserRole.BranchManager)
        {
            throw new UnauthorizedAccessException("Branch managers cannot edit admin or manager accounts.");
        }
    }

    private static UserRole ParseRole(string value)
        => Enum.TryParse<UserRole>(value, out var role)
            ? role
            : throw new InvalidOperationException("Invalid employee role.");

    private static EmployeeSummaryDto MapEmployee(Employee employee, IReadOnlyDictionary<Guid, string> branches)
        => new(
            employee.Id,
            employee.Username,
            employee.FullName,
            employee.Email,
            employee.Role.ToString(),
            employee.BranchId,
            employee.BranchId.HasValue && branches.TryGetValue(employee.BranchId.Value, out var branchName)
                ? branchName
                : "Toan he thong",
            employee.IsActive);
}

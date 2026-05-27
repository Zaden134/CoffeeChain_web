using CoffeeChainManagement.Application.DTOs.Employees;
using CoffeeChainManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeChainManagement.Api.Controllers;

// EmployeesController cung cap CRUD nhan vien voi rule theo action o service.
[ApiController]
[Authorize(Roles = "Administrator,BranchManager")]
[Route("api/employees")]
public sealed class EmployeesController(IEmployeeService employeeService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        => await ExecuteAsync(() => employeeService.GetAllAsync(cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        => await ExecuteAsync(async () =>
        {
            var employee = await employeeService.GetByIdAsync(id, cancellationToken);
            return employee is null ? NotFound() : Ok(employee);
        });

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UpsertEmployeeRequestDto request, CancellationToken cancellationToken)
        => await ExecuteAsync(async () => Ok(await employeeService.CreateAsync(request, cancellationToken)));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertEmployeeRequestDto request, CancellationToken cancellationToken)
        => await ExecuteAsync(async () => Ok(await employeeService.UpdateAsync(id, request, cancellationToken)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => await ExecuteAsync(async () =>
        {
            await employeeService.DeleteAsync(id, cancellationToken);
            return NoContent();
        });

    private async Task<IActionResult> ExecuteAsync<T>(Func<Task<T>> action)
    {
        try
        {
            return Ok(await action());
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    private async Task<IActionResult> ExecuteAsync(Func<Task<IActionResult>> action)
    {
        try
        {
            return await action();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }
}

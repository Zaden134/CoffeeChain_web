using System.ComponentModel.DataAnnotations;

namespace CoffeeChainManagement.Application.DTOs.Branches;

public sealed record UpsertBranchRequestDto(
    [property: Required, MaxLength(30)] string Code,
    [property: Required, MaxLength(150)] string Name,
    [property: Required, MaxLength(250)] string Address,
    [property: Required, MaxLength(120)] string ManagerName,
    bool IsActive);

namespace CoffeeChainManagement.Application.DTOs.Auth;

public sealed record UserSessionQueryDto(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    bool ActiveOnly = false);

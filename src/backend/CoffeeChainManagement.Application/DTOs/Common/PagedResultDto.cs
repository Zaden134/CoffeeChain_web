namespace CoffeeChainManagement.Application.DTOs.Common;

// PagedResultDto chuan hoa response danh sach lon co phan trang.
public sealed record PagedResultDto<T>(
    IReadOnlyCollection<T> Items,
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages);

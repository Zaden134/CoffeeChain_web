using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CoffeeChainManagement.Application.DTOs.Inventory;

namespace CoffeeChainManagement.Application.Interfaces;

public interface IInventoryTransactionService
{
    Task<List<InventoryTransactionDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<InventoryTransactionDto> CreateTransactionAsync(CreateInventoryTransactionRequestDto request, Guid employeeId, CancellationToken cancellationToken = default);
}

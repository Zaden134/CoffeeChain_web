using CoffeeChainManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeChainManagement.Api.Controllers;

// ProductsController phuc vu menu, mon ban chay va du lieu hien thi danh muc.
[ApiController]
[Route("api/products")]
public sealed class ProductsController(IProductService productService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var data = await productService.GetAllAsync(cancellationToken);
        return Ok(data);
    }
}

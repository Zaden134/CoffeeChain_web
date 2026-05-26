namespace CoffeeChainManagement.Application.DTOs.Auth;

// LoginRequestDto nhan thong tin dang nhap tu frontend.
public sealed record LoginRequestDto(string Username, string Password);

# backend.Dockerfile build va chay ASP.NET API tren .NET 8 de dong nhat local va server.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/backend/CoffeeChainManagement.Api/CoffeeChainManagement.Api.csproj", "src/backend/CoffeeChainManagement.Api/"]
COPY ["src/backend/CoffeeChainManagement.Application/CoffeeChainManagement.Application.csproj", "src/backend/CoffeeChainManagement.Application/"]
COPY ["src/backend/CoffeeChainManagement.Domain/CoffeeChainManagement.Domain.csproj", "src/backend/CoffeeChainManagement.Domain/"]
COPY ["src/backend/CoffeeChainManagement.Infrastructure/CoffeeChainManagement.Infrastructure.csproj", "src/backend/CoffeeChainManagement.Infrastructure/"]
RUN dotnet restore "src/backend/CoffeeChainManagement.Api/CoffeeChainManagement.Api.csproj"

COPY . .
RUN dotnet publish "src/backend/CoffeeChainManagement.Api/CoffeeChainManagement.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "CoffeeChainManagement.Api.dll"]

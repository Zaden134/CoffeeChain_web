# Coffee Chain Management

Monorepo cho do an tot nghiep quan ly va ban ca phe chuoi cua hang.

## Stack

- Backend: ASP.NET Core Web API `.NET 8`
- Frontend: `Angular 21`
- Database: `PostgreSQL`
- ORM: `EF Core + Npgsql`
- Auth: `JWT Bearer`
- Container: `Docker` + `docker compose`
- Source control: `Git` + `GitHub`

## Cau truc thu muc

```text
.
|-- src
|   |-- backend
|   |   |-- CoffeeChainManagement.Api
|   |   |-- CoffeeChainManagement.Application
|   |   |-- CoffeeChainManagement.Domain
|   |   `-- CoffeeChainManagement.Infrastructure
|   `-- frontend
|       `-- coffee-chain-admin
|-- docker
|   |-- backend.Dockerfile
|   `-- frontend.Dockerfile
|-- docs
|-- dotnet-tools.json
|-- CoffeeChainManagement.slnx
`-- docker-compose.yml
```

## Y nghia nhanh tung layer backend

- `Domain`: entity va enum cua nghiep vu quan ly chi nhanh, menu, POS, kho.
- `Application`: DTO va interface/use case de frontend/API goi vao.
- `Infrastructure`: `EF Core + Npgsql`, migrations, seed data, JWT auth implementation.
- `Api`: endpoint REST, swagger, CORS, auth middleware, startup migration/seed.

## Chay local khong dung Docker

### Backend

```powershell
dotnet tool restore
dotnet run --project .\src\backend\CoffeeChainManagement.Api
```

App startup se tu dong:

- Apply migration EF Core vao PostgreSQL
- Seed du lieu dev neu database dang rong

### Frontend

```powershell
cd .\src\frontend\coffee-chain-admin
npm.cmd install
npm.cmd start
```

Frontend mac dinh: `http://localhost:4200`

## EF Core migrations

Tao migration moi:

```powershell
dotnet tool restore
dotnet dotnet-ef migrations add <MigrationName> --project .\src\backend\CoffeeChainManagement.Infrastructure --startup-project .\src\backend\CoffeeChainManagement.Api --output-dir Persistence\Migrations
```

Apply migration thu cong:

```powershell
dotnet tool restore
dotnet dotnet-ef database update --project .\src\backend\CoffeeChainManagement.Infrastructure --startup-project .\src\backend\CoffeeChainManagement.Api
```

## Chay bang Docker

```powershell
docker compose up --build
```

## API mau de frontend dung ngay

- `GET /health`
- `POST /api/auth/login`
- `GET /api/auth/me`
- `GET /api/dashboard/overview`
- `GET /api/branches`
- `GET /api/products`

## Tai khoan seed mac dinh

- `admin / Admin@123` -> role `Administrator`
- `manager.q1 / Manager@123` -> role `BranchManager`
- `cashier.q1 / Cashier@123` -> role `Cashier`

## Phan quyen hien tai

- `Administrator`: login, xem dashboard, chi nhanh, san pham
- `BranchManager`: login, xem dashboard, chi nhanh, san pham
- `Cashier`: login, xem dashboard, san pham

## Frontend hien tai

- Login page goi `POST /api/auth/login`
- `AuthStore` luu JWT + profile vao `localStorage`
- `authGuard` chan route noi bo
- `guestGuard` chan quay lai trang login khi da dang nhap
- `authInterceptor` tu dong gan bearer token
- Shell va sitemap frontend duoc phat trien theo mau tham chieu trong file zip:
- `Dashboard`
- `Chi nhanh`
- `San pham`
- `Kho hang`
- `Nhan vien`
- `Bao cao`
- `Khuyen mai`

## Ghi chu cho ban va team

- Minh da them comment ngan o dau nhieu file de nhin vao biet file do phuc vu gi.
- Hien tai `Infrastructure` da dung `EF Core + Npgsql`, migration files va seed dev data vao PostgreSQL luc app startup.
- Auth dang dung JWT bearer token va role authorization tren cac endpoint chinh.
- App startup da tach ro `MigrateAsync()` va `SeedAsync()`.
- Frontend da co login flow, guard, interceptor va bo cuc quan tri theo mau tham chieu.

## GitHub va Docker Hub

### GitHub

```powershell
git add .
git commit -m "Add EF Core migrations and Angular auth shell"
git push -u origin main
```

### Docker Hub

```powershell
docker login
docker tag coffee-chain-api zaden134/coffee-chain-api:latest
docker tag coffee-chain-admin zaden134/coffee-chain-admin:latest
docker push zaden134/coffee-chain-api:latest
docker push zaden134/coffee-chain-admin:latest
```

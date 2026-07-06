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

## E2E tests

Playwright tests nam trong `src/frontend/coffee-chain-admin/e2e`.

Chay backend, frontend va PostgreSQL truoc, sau do:

```powershell
cd .\src\frontend\coffee-chain-admin
npm.cmd run e2e
```

Mac dinh test dung `http://localhost:4200`. Neu can doi URL:

```powershell
$env:E2E_BASE_URL="http://localhost:4200"
npm.cmd run e2e
```

Tai khoan seed dung trong E2E:

- `admin / Admin@123`
- `manager.q1 / Manager@123`

## Health va backup

Health endpoints:

- `GET /health`
- `GET /health/db`
- `GET /health/info`

Backup PostgreSQL:

```powershell
.\scripts\backup-postgres.ps1
```

Restore PostgreSQL:

```powershell
.\scripts\restore-postgres.ps1 -BackupFile .\backups\coffee-chain-yyyyMMdd-HHmmss.dump -Clean
```

## Chay bang Docker

```powershell
docker compose up --build
```

## API mau de frontend dung ngay

- `GET /health`
- `POST /api/auth/login`
- `GET /api/auth/me`
- `POST /api/auth/refresh`
- `POST /api/auth/logout`
- `GET /api/dashboard/overview`
- `GET /api/reports/sales`
- `GET /api/reports/sales/export`
- `GET /api/branches`
- `GET /api/products`
- `GET /api/employees`
- `GET /api/inventory`
- `GET /api/inventory/ingredients`
- `GET /api/promotions`
- `GET /api/recruitment-requests`

## Tai khoan seed mac dinh

- `admin / Admin@123` -> role `Administrator`
- `manager.q1 / Manager@123` -> role `BranchManager`
- `cashier.q1 / Cashier@123` -> role `Cashier`
- `warehouse.q1 / Warehouse@123` -> role `WarehouseStaff`

## Phan quyen hien tai

- `Administrator`: toan quyen he thong, CRUD nhan vien/kho/khuyen mai, review recruitment, xem bao cao, export report
- `BranchManager`: xem dashboard, chi nhanh, san pham, kho, nhan vien, khuyen mai, tao recruitment request cho chi nhanh cua minh
- `Cashier`: login, xem dashboard, san pham, xem khuyen mai dang hoat dong
- `WarehouseStaff`: dang nhap va duoc mo rong quyen ve sau cho cac tac vu kho

## Frontend hien tai

- Login page goi `POST /api/auth/login`
- `AuthStore` luu access token, refresh token va profile vao `localStorage`
- `authGuard` chan route noi bo
- `guestGuard` chan quay lai trang login khi da dang nhap
- `authInterceptor` tu dong gan bearer token va refresh token neu access token het han
- Shell va sitemap frontend duoc phat trien theo mau tham chieu trong file zip:
- `Dashboard`
- `Chi nhanh`
- `San pham`
- `Kho hang`
- `Nhan vien`
- `Bao cao`
- `Khuyen mai`
- `Yeu cau tuyen dung`

## Phan tich bao cao

- `GET /api/reports/sales` tra tong hop doanh thu theo ngay, chi nhanh va mon ban chay.
- `GET /api/reports/sales/export?format=xlsx|pdf` xuat bao cao ra Excel hoac PDF.
- Frontend `Bao cao` co bo loc ngay/chi nhanh va nut export.

## Hardening

- JWT access token co refresh token xoay vong.
- `audit_logs` ghi lai thao tac quan trong.
- `Employees`, `Inventory`, `Promotions`, `Recruitment Requests` co search + phan trang tren UI.
- Docker frontend build su dung output Angular moi nhat.

## Test

```powershell
dotnet test .\CoffeeChainManagement.slnx
```

## CI/CD

- `.github/workflows/ci.yml` chay `dotnet test` va `npm run build`.
- `.github/workflows/docker-publish.yml` build va push image neu cau hinh Docker Hub secrets.

## Ghi chu cho ban va team

- Minh da them comment ngan o dau nhieu file de nhin vao biet file do phuc vu gi.
- Hien tai `Infrastructure` da dung `EF Core + Npgsql`, migration files va seed dev data vao PostgreSQL luc app startup.
- Khuyen mai (Promotions) moi duoc bo sung them truong `Code` (nhu `HAPPY15`) va `DiscountAmount` giup chi nhanh co the tao khuyen mai theo ca % lan so tien.
- Auth dang dung JWT bearer token va role authorization tren cac endpoint chinh.
- App startup da tach ro `MigrateAsync()` va `SeedAsync()`.
- Frontend da co login flow, guard, interceptor va bo cuc quan tri theo mau tham chieu.
- Ung dung WPF POS rieng biet (nam o `../app/CoffeeChainPOS`) da duoc tich hop giao dien Material Design, Background Sync (tu dong cap nhat Menu & Khuyen mai luc 1:00 AM) va ban hang Offline.

## GitHub va Docker Hub

- Push len GitHub bang branch `main`.
- Docker Hub co the cau hinh qua secrets `DOCKERHUB_USERNAME` va `DOCKERHUB_TOKEN`.

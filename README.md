# Hệ thống quản lý chuỗi cửa hàng cà phê

Dự án khóa luận tốt nghiệp xây dựng hệ thống quản trị cho chuỗi cửa hàng cà phê. Hệ thống gồm backend ASP.NET Core, frontend Angular, PostgreSQL, Docker và các luồng quản trị phục vụ vận hành chuỗi.

## Tình Trạng Hiện Tại

Dự án đã có đầy đủ các phần chính để chạy demo và kiểm thử end-to-end:

- Đăng nhập JWT, refresh token, đăng xuất và đổi mật khẩu.
- Phân quyền theo vai trò: `Administrator`, `BranchManager`, `Cashier`, `WarehouseStaff`.
- Quản lý chi nhánh, sản phẩm, công thức, kho, giao dịch kho, nhân viên, khuyến mãi, tuyển dụng, audit log và bảo mật.
- Trang Home hiển thị KPI doanh thu toàn chuỗi, doanh thu ròng và sản phẩm bán chạy.
- Báo cáo doanh thu có lọc theo thời gian/chi nhánh, tính cả chi phí nhập kho âm, xuất Excel/PDF.
- Khuyến mãi hỗ trợ giảm theo phần trăm hoặc số tiền cố định, áp dụng theo toàn hệ thống, chi nhánh, nhóm khách hàng hoặc số điện thoại.
- Quản lý chi nhánh có nút chi tiết để chỉnh sửa; quản lý chi nhánh chỉ sửa được chi nhánh của mình, admin sửa được tất cả.
- Frontend đã có icon/logo và ảnh fallback cho đồ uống để không bị vỡ ảnh khi thiếu URL ảnh.
- Docker compose chạy PostgreSQL, backend và frontend.
- Có Playwright E2E cơ bản cho luồng admin và tuyển dụng.

## Công Nghệ Sử Dụng

- Backend: `.NET 8`, ASP.NET Core Web API
- Frontend: `Angular 21`
- Database: `PostgreSQL 17`
- ORM: `Entity Framework Core + Npgsql`
- Authentication: `JWT Bearer + Refresh Token`
- Export báo cáo: `ClosedXML` cho Excel, `QuestPDF` cho PDF
- E2E: `Playwright`
- Container: `Docker`, `Docker Compose`
- CI: GitHub Actions

## Cấu Trúc Thư Mục

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
|-- scripts
|   |-- backup-postgres.ps1
|   `-- restore-postgres.ps1
|-- tests
|-- docker-compose.yml
|-- CoffeeChainManagement.slnx
`-- README.md
```

## Ý Nghĩa Các Layer Backend

- `Domain`: entity, enum và mô hình nghiệp vụ cốt lõi.
- `Application`: DTO, interface và contract use case.
- `Infrastructure`: EF Core, PostgreSQL, migrations, seed data, JWT, audit log và service implementation.
- `Api`: controller REST, Swagger, CORS, middleware auth, health check, migrate/seed khi startup.

## Chạy Dự Án Bằng Docker

Yêu cầu: Docker Desktop đang chạy.

```powershell
docker compose up --build
```

Sau khi chạy:

- Frontend: `http://localhost:4200`
- Backend API trong Docker: `http://localhost:8080`
- PostgreSQL: `localhost:5432`

## Chạy Local Khi Phát Triển

### 1. Bật PostgreSQL

Có thể bật riêng database bằng Docker:

```powershell
docker compose up -d postgres
```

Thông tin mặc định:

- Database: `coffee_chain_management`
- Username: `postgres`
- Password: `postgres`
- Port: `5432`

### 2. Chạy Backend

```powershell
dotnet tool restore
dotnet run --project .\src\backend\CoffeeChainManagement.Api
```

Khi backend khởi động, hệ thống tự động:

- Apply EF Core migrations.
- Seed tài khoản và dữ liệu demo.
- Tạo dữ liệu sản phẩm, chi nhánh, kho, giao dịch kho, khuyến mãi, tuyển dụng và đơn bán mẫu.

Backend local thường chạy theo profile trong `launchSettings.json`. Nếu cần ép URL:

```powershell
dotnet run --project .\src\backend\CoffeeChainManagement.Api --urls http://localhost:5000
```

### 3. Chạy Frontend

```powershell
cd .\src\frontend\coffee-chain-admin
npm.cmd install
npm.cmd start
```

Frontend chạy tại:

```text
http://localhost:4200
```

Proxy frontend đang trỏ API local theo `proxy.conf.json`.

## Tài Khoản Seed Mặc Định

| Vai trò | Tên đăng nhập | Mật khẩu |
|---|---|---|
| Administrator | `admin` | `Admin@123` |
| BranchManager | `manager.q1` | `Manager@123` |
| Cashier | `cashier.q1` | `Cashier@123` |
| WarehouseStaff | `warehouse.q1` | `Warehouse@123` |

## Chức Năng Chính

### Xác Thực Và Bảo Mật

- Đăng nhập bằng JWT.
- Refresh token xoay vòng.
- Đổi mật khẩu.
- Admin reset mật khẩu nhân viên.
- Xem và thu hồi phiên đăng nhập.
- Audit log các thao tác quan trọng.

### Home

- KPI doanh thu hôm nay.
- Chi phí nhập kho hôm nay.
- Doanh thu ròng hôm nay/tháng.
- Biểu đồ doanh thu theo chi nhánh.
- Sản phẩm bán chạy.

### Chi Nhánh

- Xem danh sách chi nhánh.
- Xem doanh thu hôm nay, cảnh báo tồn kho, trạng thái.
- Bấm `Chi tiết` để sửa thông tin chi nhánh.
- Admin sửa tất cả chi nhánh.
- BranchManager chỉ sửa được chi nhánh được phân công.
- BranchManager không được đổi trạng thái hoạt động của chi nhánh.

### Sản Phẩm Và Công Thức

- Danh sách sản phẩm có phân trang/tìm kiếm.
- Tạo, sửa, ẩn sản phẩm.
- Ảnh sản phẩm có fallback theo nhóm đồ uống.
- Quản lý công thức/định mức nguyên liệu cho từng sản phẩm.

### Kho Và Giao Dịch Kho

- Quản lý tồn kho theo chi nhánh.
- Tạo/sửa/xóa mục tồn kho.
- Tạo giao dịch nhập/xuất kho.
- Giao dịch nhập kho ghi nhận số tiền âm để tính vào doanh thu ròng.
- WarehouseStaff chỉ thao tác trong chi nhánh của mình.

### Nhân Viên

- Tạo/sửa/vô hiệu hóa nhân viên.
- Không cho người dùng tự khóa chính mình.
- Có thể kích hoạt lại tài khoản đã vô hiệu hóa.
- Phân quyền thao tác theo role và chi nhánh.

### Khuyến Mãi

- Tạo/sửa/xóa khuyến mãi.
- Giảm theo `%` hoặc số tiền cố định `VND`.
- Áp dụng theo toàn hệ thống, từng cửa hàng, nhóm khách hàng hoặc số điện thoại khách hàng.
- Manager chỉ quản lý khuyến mãi trong phạm vi phù hợp.

### Tuyển Dụng

- Quản lý chi nhánh gửi yêu cầu tuyển người cho chi nhánh của mình.
- Admin xem xét, duyệt hoặc từ chối yêu cầu.
- Có ghi chú admin khi xử lý.

### Báo Cáo

- Báo cáo doanh thu theo ngày.
- Báo cáo theo chi nhánh.
- Sản phẩm bán chạy.
- Bộ lọc thời gian và chi nhánh.
- Tính chi phí nhập kho âm vào doanh thu ròng.
- Export Excel/PDF với tên file có ý nghĩa.

## API Chính

- `POST /api/auth/login`
- `POST /api/auth/refresh`
- `POST /api/auth/logout`
- `GET /api/auth/me`
- `POST /api/auth/change-password`
- `GET /api/dashboard/overview`
- `GET /api/branches`
- `PUT /api/branches/{id}`
- `GET /api/products`
- `GET /api/products/paged`
- `POST /api/products`
- `PUT /api/products/{id}`
- `DELETE /api/products/{id}`
- `GET /api/inventory`
- `POST /api/inventory`
- `GET /api/inventory-transactions`
- `POST /api/inventory-transactions`
- `GET /api/employees`
- `POST /api/employees`
- `PUT /api/employees/{id}`
- `DELETE /api/employees/{id}`
- `GET /api/promotions`
- `POST /api/promotions`
- `PUT /api/promotions/{id}`
- `DELETE /api/promotions/{id}`
- `GET /api/recruitment-requests`
- `POST /api/recruitment-requests`
- `POST /api/recruitment-requests/{id}/approve`
- `POST /api/recruitment-requests/{id}/reject`
- `GET /api/reports/sales`
- `GET /api/reports/sales/export?format=xlsx|pdf`
- `GET /api/audit-logs`
- `POST /api/support/requests`
- `POST /api/sales/sync`

## Health Check

- `GET /health`
- `GET /health/db`
- `GET /health/info`

## EF Core Migration

Tạo migration mới:

```powershell
dotnet tool restore
dotnet dotnet-ef migrations add <MigrationName> --project .\src\backend\CoffeeChainManagement.Infrastructure --startup-project .\src\backend\CoffeeChainManagement.Api --output-dir Persistence\Migrations
```

Apply migration thủ công:

```powershell
dotnet tool restore
dotnet dotnet-ef database update --project .\src\backend\CoffeeChainManagement.Infrastructure --startup-project .\src\backend\CoffeeChainManagement.Api
```

Thông thường không cần apply thủ công vì backend đã tự migrate khi startup.

## Kiểm Thử

### Backend

```powershell
dotnet build .\src\backend\CoffeeChainManagement.Api\CoffeeChainManagement.Api.csproj /p:PublishAot=false
dotnet test .\CoffeeChainManagement.slnx
```

### Frontend

```powershell
cd .\src\frontend\coffee-chain-admin
npm.cmd run build -- --configuration development
```

Build production có thể cần mạng để inline Google Fonts. Nếu máy không có mạng, nên dùng development build để kiểm tra compile.

### Playwright E2E

Chạy PostgreSQL, backend và frontend trước, sau đó:

```powershell
cd .\src\frontend\coffee-chain-admin
npm.cmd run e2e
```

Nếu cần đổi base URL:

```powershell
$env:E2E_BASE_URL="http://localhost:4200"
npm.cmd run e2e
```

## Backup Và Restore PostgreSQL

Backup:

```powershell
.\scripts\backup-postgres.ps1
```

Restore:

```powershell
.\scripts\restore-postgres.ps1 -BackupFile .\backups\coffee-chain-yyyyMMdd-HHmmss.dump -Clean
```

## CI/CD

Workflow hiện có:

- `.github/workflows/ci.yml`

CI kiểm tra backend và frontend build theo cấu hình trong workflow.

## Ghi Chú Demo

- POS/bán hàng có app riêng, frontend admin không cần màn POS.
- Endpoint `POST /api/sales/sync` dùng để đồng bộ đơn từ POS/app bán hàng vào hệ thống quản trị.
- Dữ liệu nhập kho là chi phí nên hiển thị số âm và được tính vào doanh thu ròng.
- Các file ảnh đồ uống fallback nằm trong `src/frontend/coffee-chain-admin/public/assets/drinks`.
- README này được cập nhật theo trạng thái dự án ngày `13/07/2026`.

## GitHub

Repository:

```text
https://github.com/Zaden134/CoffeeChain_web.git
```

Nhánh chính:

```text
main
```

# Coffee Chain Management

Monorepo khoi tao cho do an tot nghiep quan ly va ban ca phe chuoi cua hang.

## Stack

- Backend: ASP.NET Core Web API `.NET 8`
- Frontend: `Angular 21`
- Database: `PostgreSQL`
- Container: `Docker` + `docker-compose`
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
|-- CoffeeChainManagement.slnx
`-- docker-compose.yml
```

## Y nghia nhanh tung layer backend

- `Domain`: entity va enum cua nghiep vu quan ly chi nhanh, menu, POS, kho.
- `Application`: DTO va interface/use case de frontend/API goi vao.
- `Infrastructure`: implementation tam bang in-memory, sau nay doi sang PostgreSQL/EF Core.
- `Api`: endpoint REST, swagger, CORS, health check.

## Chay local khong dung Docker

### Backend

```powershell
dotnet run --project .\src\backend\CoffeeChainManagement.Api
```

API mac dinh: `http://localhost:8080` neu chay bang Docker, hoac theo port do ASP.NET cap neu chay thu cong.

### Frontend

```powershell
cd .\src\frontend\coffee-chain-admin
npm.cmd install
npm.cmd start
```

Frontend mac dinh: `http://localhost:4200`

## Chay bang Docker

```powershell
docker compose up --build
```

## API mau de frontend dung ngay

- `GET /health`
- `GET /api/dashboard/overview`
- `GET /api/branches`
- `GET /api/products`

## Ghi chu cho ban va team

- Mình da them comment ngan o dau nhieu file de nhin vao biet file do phuc vu gi.
- Hien tai `Infrastructure` dang dung du lieu mau in-memory de setup nhanh va tranh phu thuoc package/database ngay buoc dau.
- Khi ban muon, buoc tiep theo hop ly la them `Entity Framework Core + Npgsql + JWT Auth + role-based authorization`.

## GitHub va Docker Hub

### GitHub

```powershell
git init
git add .
git commit -m "Initial coffee chain management setup"
git branch -M main
git remote add origin https://github.com/zaden134/<ten-repo>.git
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

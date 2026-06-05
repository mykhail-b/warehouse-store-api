# Warehouse Store API

[![.NET 10](https://img.shields.io/badge/.NET-10-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)

REST API for an online store with warehouse management functionality.

---

## Tech Stack

- **ASP.NET Core 10** — framework
- **Entity Framework Core** — ORM, Code-First
- **SQL Server** — database
- **Redis** — caching for catalog and cart
- **JWT Bearer** — authentication
- **Docker / Docker Compose** — containerization
- **k6** — load testing

---

## Project Structure

```
SomeProject/
├── Backend/                 # Main API application
│   ├── Controllers/
│   ├── Services/
│   ├── Data/
│   ├── Migrations/
│   └── Dockerfile
├── ClassLibrary/            # Shared entities and DTOs
│   ├── Entity/
│   └── Dto/
├── Backend.Tests/           # xUnit Tests
│   └── k6-scripts/
└── docker-compose.yml
```

---

## Getting Started

### Requirements
- .NET 10 SDK
- SQL Server or SQL Server Express LocalDB
- Redis
- Docker

### Installation

1. Clone the repository
```bash
git clone https://github.com/mykhail-b/warehouse-store-api.git
cd SomeProject
```

2. Configure `appsettings.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=WarehouseDb;Integrated Security=true;"
  },
  "Jwt": {
    "Key": "your-very-long-secret-key-min-32-chars",
    "Issuer": "warehouse-api",
    "Audience": "warehouse-app"
  },
  "Redis": {
    "Connection": "localhost:6379"
  }
}
```

3. Apply migrations
```bash
cd Backend
dotnet ef database update
```

4. Run the application
```bash
dotnet run
```

API: `https://localhost:7133`  
Swagger: `https://localhost:7133/swagger`

### Docker
```bash
docker-compose up -d
```

---

## API Endpoints

### Authentication
```
POST   /api/auth/register
POST   /api/auth/login
```

### Orders
```
GET    /api/order
GET    /api/order/{id}
POST   /api/order
PUT    /api/order
DELETE /api/order/{id}
```

### Warehouse Items (Admin)
```
GET    /api/warehouseitem
GET    /api/warehouseitem/{id}
POST   /api/warehouseitem
PUT    /api/warehouseitem
DELETE /api/warehouseitem/{id}
```

### Deliveries (Admin)
```
GET    /api/warehousedelivery/outbound
GET    /api/warehousedelivery/outbound/{id}
```

---

## Testing

### Unit tests
```bash
dotnet test
```

### Load testing (k6)
```bash
k6 run Backend.Tests/k6-scripts/load-test.ts
```

---

## Architecture

```
Controllers → Services → Data Access (EF Core) → SQL Server
```

Key decisions:
- Database transactions on order creation — atomicity guarantee
- Soft delete for warehouse items
- Paginated product catalog
- DTOs to separate external and internal models

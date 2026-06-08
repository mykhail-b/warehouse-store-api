# Warehouse Store API
[![.NET 10](https://img.shields.io/badge/.NET-10-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)

REST API for an online store with warehouse management functionality.

---

## Tech Stack

- **ASP.NET Core 10**
- **Entity Framework Core**
- **Microsoft SQL Server**
- **Redis** — caching
- **Grafana k6** — load testing
- **MailKit** — sending emails
- **MailHog** — local email testing
- **Stripe** — payment processing API

---

## Project Structure

```
├── Backend/                 # Main API application
├── ClassLibrary/            # Shared entities and DTOs
├── Backend.Tests/           # xUnit Tests
└── Frontend                 # Next.Js frontend in the future

```

---

## Getting Started

### Requirements

- .NET 10 SDK
- SQL Server or SQL Server Express LocalDB
- Docker (for Redis and MailHog)

### Installation

1. Clone the repository

```bash
git clone https://github.com/mykhail-b/warehouse-store-api.git
cd SomeProject
```

2. Start Redis and MailHog

Run Redis:
```bash
docker run -d -p 6379:6379 --name redis redis
```

Run MailHog:
```bash
docker run -d -p 1025:1025 -p 8025:8025 --name mailhog mailhog/mailhog
```

- MailHog UI: `http://localhost:8025`

3. Configure `appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=WarehouseDb;Integrated Security=true;"
  },

  "Jwt": {
    "Key": "your-secret-key-min-32-chars",
    "Issuer": "warehouse-api",
    "Audience": "warehouse-app"
  },

  "Redis": {
    "Connection": "localhost:6379"
  },

  "EmailSettings": {
    "SmtpServer": "localhost",
    "Port": 1025,
    "SenderName": "Computer Store",
    "SenderEmail": "no-reply@computerstore.com"
  },

  "Stripe": {
    "SecretKey": "YOUR_STRIPE_SECRET_KEY",
    "PublishableKey": "YOUR_STRIPE_PUBLISHABLE_KEY"
  }
}
```

4. Apply migrations

```bash
cd Backend
dotnet ef database update
```

5. Run the application

```bash
dotnet run
```

API: `http://localhost:5111`  
Swagger: `http://localhost:5111/swagger`

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
DELETE /api/order/{id}
```

### Products

```
GET    /api/product
GET    /api/product/{id}
POST   /api/product
PUT    /api/product
DELETE /api/product/{id}
```

### Delivery

For the needs of the admin panel or something similar

```
GET    /api/delivery
GET    /api/delivery{id}
```

### Stripe Webhooks

Handles incoming events from Stripe to automatically create an order and send a confirmation email after successful payment.

```
POST    /api/webhook
```
---
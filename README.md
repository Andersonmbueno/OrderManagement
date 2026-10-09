	# Order Management API

Technical challenge implementation for a simple e-commerce order management backend.

The project intentionally favors **clear architecture and testability over unnecessary complexity**.

## Stack

- .NET 10
- ASP.NET Core Web API / Controllers
- Clean Architecture: Domain, Application, Infrastructure, API
- CQRS with MediatR
- Entity Framework Core + SQLite
- JWT authentication
- FluentValidation with a MediatR pipeline behavior
- xUnit + Moq + FluentAssertions
- Docker / Docker Compose

## Architecture

```text
OrderManagement.Api
                
OrderManagement.Application
                
OrderManagement.Domain

OrderManagement.Infrastructure ---> Application + Domain
```

- **Domain** contains entities and business rules. It has no dependency on EF Core or ASP.NET.
- **Application** contains use cases (commands/queries), handlers, validation and repository abstractions.
- **Infrastructure** implements persistence using EF Core/SQLite.
- **API** handles HTTP, authentication, dependency injection and presentation concerns.

This keeps business rules independent from frameworks and makes the handlers easy to unit test.

## Domain rules

1. An order must contain at least one item.
2. Quantity must be greater than zero.
3. UnitPrice must be greater than zero.
4. Only Pending orders can be cancelled.
5. TotalAmount is calculated by the `Order` domain entity, not by the Application or API layer.

## Endpoints

| Method | Route | Auth | Purpose |
|---|---|---|---|
| POST | `/auth/login` | No | Returns a JWT |
| POST | `/api/orders` | Yes | Creates an order |
| GET | `/api/orders?page=1&pageSize=10` | Yes | Lists orders with pagination |
| GET | `/api/orders/{id}` | Yes | Gets one order |
| PATCH | `/api/orders/{id}/cancel` | Yes | Cancels a pending order |

## Fixed login required by the challenge

```text
Username: dev@martech.com
Password: Senha@123
```

This is intentionally simple because the challenge explicitly allows a fixed in-memory/configured user. It is **not** intended as production authentication.

## Run locally

Prerequisite: .NET 10 SDK.

```bash
dotnet restore
dotnet build
dotnet test

dotnet run --project src/OrderManagement.Api
```

The API starts using SQLite. On startup, EF Core applies the included migration automatically.

Swagger is available in Development mode.

## Run with Docker

```bash
docker compose up --build
```

Then open:

```text
http://localhost:8080/swagger
```

The SQLite database is stored in a Docker named volume.

## Example API usage

### 1. Login

```http
POST /auth/login
Content-Type: application/json

{
  "username": "dev@martech.com",
  "password": "Senha@123"
}
```

Copy the `accessToken` from the response and use it as:

```text
Authorization: Bearer <token>
```

### 2. Create an order

```http
POST /api/orders
Authorization: Bearer <token>
Content-Type: application/json

{
  "customerId": "11111111-1111-1111-1111-111111111111",
  "items": [
    {
      "productName": "Mechanical Keyboard",
      "quantity": 2,
      "unitPrice": 250.00
    },
    {
      "productName": "Mouse",
      "quantity": 1,
      "unitPrice": 100.00
    }
  ]
}
```

The domain calculates `TotalAmount = 600.00`.

### 3. List orders

```http
GET /api/orders?page=1&pageSize=10
Authorization: Bearer <token>
```

### 4. Cancel

```http
PATCH /api/orders/{id}/cancel
Authorization: Bearer <token>
```

## Why no generic IRepository<T>?

The challenge explicitly asks not to introduce a generic repository without a real reason. The project therefore exposes `IOrderRepository`, focused on the operations the order use cases actually need.

## Why is TotalAmount in the Domain?

`TotalAmount` is a business concept. If the calculation lived in a controller or handler, another entry point could calculate it differently. Keeping it inside `Order` makes the rule part of the aggregate and gives us one source of truth.

## Why CQRS here?

The exercise explicitly requires commands and queries to be separated. Commands such as `CreateOrderCommand` and `CancelOrderCommand` change state. Queries such as `GetOrderQuery` and `GetOrdersQuery` read state.

CQRS is kept intentionally small: there is no event bus, message broker or separate read database because the challenge does not require them.

## Why FluentValidation + pipeline behavior?

Validation should happen before a handler executes. The MediatR pipeline behavior applies validators consistently to requests without duplicating validation code in controllers or handlers.

## Deliberate scope

The challenge marks Serilog logging, OpenTelemetry, SonarQube/dotnet-sonarscanner and integration tests as desirable rather than mandatory. A basic WebApplicationFactory integration test is included. Logging uses the standard ASP.NET Core logging abstraction to avoid adding infrastructure that does not materially improve this small exercise.

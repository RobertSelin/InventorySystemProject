# Inventory Management System

An Inventory Management System built with ASP.NET Core 8 Web API, Entity Framework Core, and an ASP.NET Core MVC management client. The project demonstrates clean architecture, secure authentication, client resilience, and automated testing.

---

## Architecture and Design Principles

The application is structured following Clean Architecture and separation of concerns:

- **Domain and Persistence**: Rich entity models (`Product`, `Category`, `InventoryLog`, `User`) mapped via EF Core Fluent API with SQL Server.
- **Data Access Layer**: Generic Repository and Unit of Work patterns encapsulating data operations, query composition, and atomic transactions.
- **Business Layer**: Isolated service layer managing domain business rules, AutoMapper DTO projections, server-side pagination, search filtering, and sorting.
- **Stock Audit and Transactions**: Stock adjustments are processed within explicit database transactions to guarantee data consistency and record immutable audit trail entries (`InventoryLog`).
- **Error Handling**: Centralized global exception handling middleware mapping domain exceptions to RFC 7807 Problem Details responses.
- **Security**: JWT Bearer token authentication with PBKDF2 password hashing (`PasswordHasher<User>`) and role-based endpoint authorization (`Admin`, `Viewer`).
- **Rate Limiting**: Native ASP.NET Core 8 rate limiting middleware partitioned per client IP address on sensitive authentication endpoints to prevent brute-force abuse.
- **Client Resilience**: Polly resilience pipeline configured on the typed HTTP client with exponential backoff, jitter, timeouts, and circuit breaking.
- **Testing**: Comprehensive test suite using xUnit, Moq for unit testing business rules and repository side effects, and `WebApplicationFactory` for in-memory integration testing.

---

## Solution Structure

```
InventorySystem/
├── InventorySystem.API/       # RESTful Web API (ASP.NET Core 8)
│   ├── Controllers/          # API Controllers (Auth, Products, Categories)
│   ├── Data/                 # AppDbContext, entity configurations, migrations
│   ├── DTOs/                 # Data transfer objects (Auth, Products)
│   ├── Exceptions/           # Domain exceptions and global exception handler
│   ├── Models/               # Domain entities (Product, Category, User, InventoryLog)
│   ├── Repositories/         # Generic and specific repository implementations
│   └── Services/             # Business logic layer (ProductService, AuthService)
│
├── InventorySystem.Web/       # Frontend client (ASP.NET Core MVC)
│   ├── Controllers/          # MVC Controllers consuming the API
│   ├── Services/             # Typed HttpClient with Polly resilience
│   └── Views/                # Razor views for product and inventory management
│
└── InventorySystem.Tests/     # Automated testing suite
    ├── ProductServiceTests.cs                 # Unit tests (xUnit + Moq)
    └── ProductsControllerIntegrationTests.cs  # Integration tests (WebApplicationFactory)
```

---

## Technology Stack

- **Runtime**: .NET 8 (C# 12)
- **Database**: Microsoft SQL Server / LocalDB with Entity Framework Core 8
- **Documentation**: Swashbuckle Swagger / OpenAPI with XML code documentation and JWT Bearer authorization support
- **Resilience**: Microsoft.Extensions.Http.Resilience (Polly v8)
- **Testing**: xUnit, Moq, Microsoft.AspNetCore.Mvc.Testing

---

## API Endpoints

### Authentication (`/api/auth`)

- `POST /api/auth/register` - Register a new user account (Rate limited per IP).
- `POST /api/auth/login` - Authenticate and retrieve a signed JWT Bearer token (Rate limited per IP).

### Products (`/api/products`)

- `GET /api/products` - Retrieve paginated, searchable, and sortable products.
- `GET /api/products/{id}` - Retrieve product details by ID.
- `POST /api/products` - Create a new product (Requires `Admin` role).
- `PUT /api/products/{id}` - Update product details (Requires `Admin` role).
- `DELETE /api/products/{id}` - Delete a product (Requires `Admin` role).
- `POST /api/products/{id}/adjust-stock` - Atomically adjust inventory with transaction and audit log (Requires `Admin` role).

---

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server (or LocalDB / SQL Server Express)
- Visual Studio 2022 or VS Code with C# Dev Kit

### Configuration

1. In `InventorySystem.API`, locate `appsettings.Example.json`.
2. Create a copy named `appsettings.Development.json` (or `appsettings.json`) in the same directory:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=inventory_test;Trusted_Connection=True;TrustServerCertificate=True;"
     },
     "JWT": {
       "Key": "YOUR_STRONG_SECRET_KEY_AT_LEAST_32_CHARACTERS",
       "Issuer": "InventorySystem.API",
       "Audience": "InventorySystem.Web",
       "ExpiryMinutes": 60
     }
   }
   ```

### Database Migration

Apply EF Core migrations to create the database schema and seed default users:

```bash
cd InventorySystem.API
dotnet ef database update
```

Default seeded credentials:

- **Admin**: Username: `admin` | Password: `111111`
- **Viewer**: Username: `viewer` | Password: `111111`

### Running the Application

To run the Web API:

```bash
cd InventorySystem.API
dotnet run
```

Access the Swagger documentation at: `https://localhost:7265/swagger`

To run the MVC client:

```bash
cd InventorySystem.Web
dotnet run
```

Access the MVC interface at: `https://localhost:7266`

### Running Tests

Execute the automated test suite across unit and integration tests:

```bash
cd InventorySystem.Tests
dotnet test
```

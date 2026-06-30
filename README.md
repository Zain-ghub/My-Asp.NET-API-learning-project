[README.md](https://github.com/user-attachments/files/25934255/README.md)
![Tests](https://github.com/Zain-ghub/My-Asp.NET-API-learning-project/actions/workflows/dotnet-tests.yml/badge.svg)
# RepoApi — ASP.NET Core REST API

A RESTful Web API built with ASP.NET Core and Entity Framework Core, demonstrating core backend development concepts including relational data modeling, middleware pipelines, and DTO patterns.

## Tech Stack

- Docker & Docker Compose
- ASP.NET Core 9
- Entity Framework Core (Code First)
- SQL Server
- C#
- xUnit + FluentAssertions (unit testing)
- Moq (mocking)
- RabbitMQ (event-driven messaging)
- GitHub Actions (CI)
- SignalR (real-time communication)


## Features

- Full CRUD endpoints for Products, Orders, Customers, Brands, and Categories
- Relational data model with foreign keys and navigation properties
- DTO layer to control API response shape and avoid circular reference issues
- Custom middleware pipeline — global exception handling and request logging
- Order pricing logic validated server-side against the database
  
## Architecture
Controllers are intentionally split into two groups:
- **Refactored** (`OrderController`, `CustomerController`) — thin controllers depending on service interfaces, with business logic in dedicated service classes
- **Legacy** (`ProductsController`, `BrandController`, `CategoryController`) — direct DbContext access, kept as-is for comparison
  
## Event-Driven Order Processing
When an order is created, the API publishes an `order_created` event to RabbitMQ. A separate console application (`RepoApi.OrderConsumer`) listens for these events and independently deducts stock from the database — demonstrating decoupled, event-driven communication between services.

- **Producer**: `OrderService` publishes order details (Id, items, quantities) after successfully saving an order
- **Consumer**: `RepoApi.OrderConsumer` listens continuously, validates stock availability, and updates `Product.StockQuantity`
- Stock validation also happens synchronously in the API before order creation, returning a clear error if insufficient stock is available

## Real-Time Stock Updates
The Order Consumer broadcasts live stock changes via SignalR after processing each order. A minimal test page (`wwwroot/stocktest.html`) demonstrates this — connecting to the `/stockhub` endpoint and displaying stock updates in real time as orders are processed, with no page refresh required.

## Running with Docker
The full stack — API, SQL Server, RabbitMQ, and the Order Consumer — can be run together in containers with no local SQL Server or RabbitMQ installation required.

1. Make sure Docker Desktop is installed and running
2. From the solution root, run: "docker-compose up --build"
3. The API will be available at `http://localhost:8080`
4. Database migrations are applied automatically on startup
5. To reset the database completely: "docker-compose down -v" "docker-compose up --build"
  
## Running Tests
1. Open the solution in Visual Studio
2. Go to Test → Run All Tests
3. All 14 tests should pass
   
## Continuous Integration
Tests run automatically on every push via GitHub Actions. See the badge above for current build status.
   
## Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/products | Get all products |
| GET | /api/products/{id} | Get product by ID |
| POST | /api/products | Create a product |
| PUT | /api/products/{id} | Update a product |
| DELETE | /api/products/{id} | Delete a product |
| GET | /api/order | Get all orders |
| GET | /api/order/{id} | Get order by ID |
| POST | /api/order | Create an order |
| PUT | /api/order/{id} | Update an order |
| DELETE | /api/order/{id} | Delete an order |
| GET | /api/customer | Get all customers |
| GET | /api/customer/{id} | Get customer with order history |
| POST | /api/customer | Create a customer |
| PUT | /api/customer/{id} | Update a customer |
| DELETE | /api/customer/{id} | Delete a customer |

## Getting Started

1. Clone the repository
2. Update the connection string in `appsettings.json` under `RepoConnection`
3. Run `dotnet ef database update` to apply migrations
4. Run `dotnet run` to start the API

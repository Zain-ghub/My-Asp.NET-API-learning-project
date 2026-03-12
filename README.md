[README.md](https://github.com/user-attachments/files/25934255/README.md)
# RepoApi — ASP.NET Core REST API

A RESTful Web API built with ASP.NET Core and Entity Framework Core, demonstrating core backend development concepts including relational data modeling, middleware pipelines, and DTO patterns.

## Tech Stack

- ASP.NET Core 9
- Entity Framework Core (Code First)
- SQL Server
- C#

## Features

- Full CRUD endpoints for Products, Orders, Customers, Brands, and Categories
- Relational data model with foreign keys and navigation properties
- DTO layer to control API response shape and avoid circular reference issues
- Custom middleware pipeline — global exception handling and request logging
- Order pricing logic validated server-side against the database

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

# Book Management API

A RESTful API built with ASP.NET Core that provides functionality for managing books.

## Features

- CRUD operations for books (single and bulk operations)
- JWT Authentication
- Book popularity tracking
- Pagination support
- Swagger documentation
- Input validation
- Soft delete functionality

## Technical Stack

- .NET 8
- Entity Framework Core
- SQL Server
- ASP.NET Core Web API
- JWT Authentication
- Swagger/OpenAPI

## Project Structure

The solution follows a clean 3-layer architecture:

- **BookManagement.API** - REST API layer with controllers and DTOs
- **BookManagement.Domain** - Domain models
- **BookManagement.Infrastructure** - Data access layer and implementations

## Getting Started

1. Clone the repository
2. Update the connection string in `appsettings.json`
3. Run database migrations:
   ```
   dotnet ef database update
   ```
4. Run the application:
   ```
   dotnet run
   ```

## API Documentation

Once the application is running, you can access the Swagger documentation at:
`https://localhost:7106/swagger`

## Authentication

All endpoints are secured with JWT authentication. To access the API:
1. Register a new user
2. Get JWT token using login endpoint
3. Include the token in Authorization header for subsequent requests
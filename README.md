# LibraryApi

A RESTful API for managing a library's book collection, built with ASP.NET Core Minimal API, Entity Framework Core, and JWT Authentication.

## Features

- Full CRUD operations for books
- JWT-based authentication for protected endpoints
- SQLite database with Entity Framework Core
- Interactive API documentation via Swagger

## Tech Stack

- .NET 10 (Minimal API)
- Entity Framework Core + SQLite
- JWT Bearer Authentication
- Swagger / OpenAPI

## Getting Started

```bash
dotnet restore
dotnet ef database update
dotnet run
```

Navigate to `/swagger` to explore the API.

## Endpoints

| Method | Route         | Auth Required | Description         |
|--------|---------------|----------------|----------------------|
| GET    | /books        | No             | List all books       |
| GET    | /books/{id}   | No             | Get a single book    |
| POST   | /books        | Yes            | Create a new book    |
| PUT    | /books/{id}   | Yes            | Update a book        |
| DELETE | /books/{id}   | Yes            | Delete a book        |
| POST   | /login        | No             | Get a JWT token      |

## Test Credentials

- Username: `admin`
- Password: `1234`
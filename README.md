# Expense Tracker API

A Clean Architecture–based Expense Tracker backend API built with ASP.NET Core.
This project is designed as a learning-focused but real-world–oriented backend system, emphasizing proper architecture, maintainability, and backend best practices.

The goal of this project is not just CRUD functionality, but to deeply understand enterprise-grade API design, including auditing, soft deletion, filtering, validation, and authorization patterns.

<hr>

## 🚀 Features

### Core Features
- Expense management (CRUD)
- Category and budget support
- User-based expense ownership
- Advanced filtering and pagination

### Architecture & Design
- Clean Architecture
- CQRS with MediatR
- FluentValidation
- DTO-based request/response models

### Auditing & Data Safety

  #### Automatic audit logging
  - Entity name
  - Entity ID
  - Action (Created, Updated, Deleted)
  - User ID
  - Timestamp
  - Correlation ID
  #### Soft delete
  - Deleted data is never removed from the database
  - Global query filters applied automatically

### Filtering & Querying
- Date range filtering
- Min/Max amount filtering
- Category-based filtering
- User-based filtering
- Sorting and pagination support

### Security
- JWT-based authentication
- Role-based authorization
- User context abstraction
- Secure user access enforcement

<hr>

## 🔦 Validation & Error Handling

- FluentValidation for:
  - Commands
  - Queries
  - Filter objects
- Centralized exception handling
- Clear validation and domain error messages

<hr>

## 📁 Technologies Used
- ASP.NET Core Web API
- Entity Framework Core
- MediatR
- FluentValidation
- JWT Authentication
- SQL Server
- Clean Architecture
- CQRS Pattern

<hr>

## ⚙️ Getting Started

### Prerequisites
- .NET SDK (latest LTS)
- SQL Server
- Visual Studio / VS Code

### Setup
```
git clone <repository-url>
cd ExpenseTracker
dotnet restore
dotnet build
```

### Database
- Update appsettings.json with your connection string
- Apply migrations:
```
  dotnet ef database update
```
### Run the API
```
dotnet run --project ExpenseTracker.API
```

<hr>

## 📌 Project Intent & Honesty Note
- Some ideas were inspired by industry patterns and learning resources, but all architecture decisions, implementations, and extensions were written and adapted manually to deepen understanding of real-world backend systems.

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

### Authentication and Security
- JWT-based authentication
- policy based authorization
- Email confirmation for new users
- Phone OTP verification
- User context abstraction
- Secure user access enforcement

### Auditing & Data Safety
- Automatic audit logging
  - Entity name
  - Entity ID
  - Action (Created, Updated, Deleted)
  - User ID
  - Timestamp
  - Correlation ID
  - Soft delete
- Deleted data is never removed from the database
- Restore deleted entities
- User's timeline and Entity's timeline tracking
- Audit logs retention: automatically cleaned after 90 days
- Global query filters applied automatically

### Data Export
- Export expenses and audit logs in CSV/Excel/pdf format
  
### Filtering & Querying
- Expenses filtering
  - Date range filtering
  - Min/Max amount filtering
  - Category-based filtering
  - User-based filtering
  - Sorting and pagination support
- Auditlogs filtering
  - Date range filtering
  - Entity-based filtering
  - User-based filtering
  - Action-based filtering

<hr>

## 🧩 Solution Structure
```
ExpenseTracker
├── ExpenseTracker.API            # Controllers, Middleware
├── ExpenseTracker.Application    # CQRS, Validators, DTOs, Exceptions, Service Interface, Cross-cutting concerns
├── ExpenseTracker.Domain         # Entities, Enums, Base Models, Repository Interface
├── ExpenseTracker.Persistence    # EF Core, DbContext, Identity
└── ExpenseTracker.Infrastructure # Services, Repositories
```

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
- .NET 8.0
- ASP.NET Core
- Entity Framework Core
- MediatR
- FluentValidation
- JWT Authentication
- SQL Server
- Clean Architecture
- CQRS Pattern
- smtp4dev
- SMSGateway

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
- navigate to solution directory
```
  cd backend
```
- Apply migrations:
```
  dotnet ef database update
```
### Run the API
first navigate to solution directory
```
  cd backend
```
and then run the solution
```
dotnet run --project ExpenseTracker.API
```

<hr>

## 📌 Project Intent & Honesty Note
- Built manually (not scaffold-only)
- Incrementally designed
- Focused on backend engineering concepts
- Not a tutorial copy
- Not production-ready
- Some ideas were inspired by industry patterns and learning resources, but all architecture decisions, implementations, and extensions were written and adapted manually to deepen understanding of real-world backend systems.

# Expense Tracker API

This is an ongoing project, designed as a learning-focused yet real-world–oriented backend system, with continuous improvements aimed at proper architecture, maintainability, observability, and backend best practices.

The goal of this project is not just CRUD functionality, but to deeply understand enterprise-grade API design, including auditing, soft deletion, filtering, validation, and authorization patterns and other production-inspired backend concerns.

<hr>

## 🚀 Features

### Core Features
- Expense management (CRUD and many more)
- Category and budget support
- User-based expense ownership
- Advanced filtering and pagination
- Real-time budget threshold notifications when limits are exceeded

### Architecture & Design
- Clean Architecture
- CQRS with MediatR
- FluentValidation
- DTO-based request/response models
- Middleware-driven cross-cutting concerns

### Authentication and Security
- JWT-based authentication
- policy based authorization
- Email confirmation for new users
- Phone OTP verification
- User context abstraction
- Secure user-scoped data access enforcement

### Observability & Diagnostics
- Correlation ID–based request tracking across:
  - Logs
  - API responses
  - Audit logs
- Structured logging with enriched context:
  - Correlation ID
  - User ID
- Request timing and slow-request detection
- Centralized exception handling with consistent error contracts
- Client-visible trace and correlation identifiers for easier debugging
  
### Auditing & Data Safety
- Automatic audit logging
  - Entity name
  - Entity ID
  - Action (Created, Updated, Deleted)
  - User ID
  - Timestamp
  - Correlation ID
- Soft delete implementation
- Restore deleted entities
- User timeline and Entity timeline tracking
- Audit logs retention policy
-   automatically cleaned after 90 days
- Global query filters applied automatically

### Data Export
- Export expenses and audit logs in
  - CSV
  - Excel
  - PDF format
  
### Filtering & Querying
- Expenses filtering
  - Date range filtering
  - Min/Max amount filtering
  - Category-based filtering
  - User-based filtering
  - Sorting and pagination support
- Audit log filtering
  - Date range
  - Entity-based
  - User-based
  - Action-based

<hr>

## 🧩 Solution Structure
```
backend
├── ExpenseTracker.API            # Controllers, Middleware
├── ExpenseTracker.Application    # CQRS, Validators, DTOs, Exceptions, Service Interface, Cross-cutting concerns
├── ExpenseTracker.Domain         # Entities, Enums, Base Models, Repository Interface
├── ExpenseTracker.Persistence    # EF Core, DbContext, Identity
└── ExpenseTracker.Infrastructure # Services, Repositories, External integrations
```

<hr>

## 🔦 Validation & Error Handling
- FluentValidation for:
  - Commands
  - Queries
  - Filter objects
- Centralized global exception handling
- Consistent and structured error responses
- Validation, domain, and infrastructure exceptions handled separately
  
<hr>

## 🗄️ Tech Stack
- .NET 8.0
- ASP.NET Core
- Entity Framework Core
- MediatR
- FluentValidation
- JWT Authentication
- SQL Server
- Clean Architecture
- SignalR
- CQRS Pattern
- smtp4dev
- SMSGateway
- Serilog (structured logging)

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
- Navigate to solution directory>
```
  cd backend
```
- Apply migrations:
```
  dotnet ef database update --project ExpenseTracker.Persistence --startup-project ExpenseTracker.API
```
### Run the API
- Navigate to solution directory:
```
  cd backend
```
- Run the solution:
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

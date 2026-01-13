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
- Clear separation of application, domain, and infrastructure concerns

### Authentication and Security
- JWT-based authentication
- policy based authorization
- Email confirmation for new users
- Phone OTP verification
- User context abstraction
- Secure user-scoped data access enforcement

### Observability & Diagnostics
This project treats observability as a first-class concern, with clear separation between logging, metrics, and audit trails.

#### Request Tracing & Correlation
- Correlation ID–based request tracking across:
  - Application Logs
  - API responses
  - Audit logs
- Correlation ID propagated via middleware and exposed to clients
- Enables full request lifecycle tracing across layers

#### Structured Logging
- Structured logging using Serilog
- Logs enriched with:
  - Correlation ID
  - User ID
  - Request metadata
- Context-aware logging inside handlers, middleware, and infrastructure services
- Proper log level usage:
  - Information for business flow
  - Warning for expected failures
  - Error for unexpected or system-level faults
  
#### Metrics(OpenTelemetry)
- OpenTelemetry-based metrics instrumentation
- Clear distinction between system metrics and business metrics
- System Metrics:
  - HTTP request duration
  - Active requests and server health signals
  - Infrastructure-level observability
- Business Metrics:
  - Business operation latency (opt-in per use case)
  - Business operation success counters
  - Business operation failure counters (recorded centrally in exception middleware)
  - Domain-specific metrics
  Metrics are intentionally low-cardinality and aggregate-focused, designed for dashboards and alerting rather than per-request tracing.

#### Error Handling & Diagnostics
- Centralized global exception handling middleware
- Consistent error response contracts
- Domain, validation, and infrastructure exceptions handled explicitly
- Client-visible correlation identifiers for easier debugging and support


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
├── ExpenseTracker.Application    # CQRS, Validators, DTOs, Exceptions, Observability, Service Interface, Cross-cutting concerns
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
- Centralized exception handling
- Consistent and structured error responses
- Clear separation between validation, domain, and infrastructure errors
  
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
- Serilog (structured logging)
- OpenTelemetry (metrics & observability)
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

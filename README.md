# Expense Tracker API

A production-inspired, enterprise-grade backend system built with a strong focus on architecture, security, observability, and maintainability.

This project goes far beyond basic CRUD operations. It is intentionally designed to explore and apply real-world backend engineering concerns such as auditing, security event tracking, soft deletion, filtering, validation, policy-based authorization, and operational observability.

While not intended for production deployment, the system is engineered with production patterns and discipline, serving both as a deep learning platform and a realistic backend reference.

<hr>
 
## 🧠 Engineering Intent & Portfolio Positioning
This project is intentionally designed as a production-inspired backend system rather than a simple demo or tutorial application.

The primary focus is on:
- Architectural clarity
- Security-first thinking
- Observability and diagnostics
- Maintainability and long-term evolution
Rather than optimizing for feature quantity, this codebase prioritizes how and why systems are built, mirroring real-world backend decision-making.

This repository serves as:
- A backend engineering portfolio project
- A reference implementation for Clean Architecture–based APIs
- A learning-driven system built with production discipline
- 
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
- Clear separation of application, domain, infrastructure concerns, and API layer

### Key Engineering Decisions
#### Clean Architecture + CQRS
- Chosen to enforce:
  - Clear dependency direction
  - Testability
  - Long-term maintainability
- CQRS used selectively to avoid unnecessary complexity
- 
#### Explicit Security Telemetry
- Security events are modeled as first-class domain concepts
- Security logs are separated from:
  - Application logs
  - Audit logs
- Enables forensic analysis and security auditing

#### Observability as a Core Concern
- Logging, metrics, and audit trails are intentionally separated
- Avoided high-cardinality metrics to ensure dashboard usability
- Business metrics are treated differently from infrastructure metrics

#### Middleware-Driven Cross-Cutting Concerns
- Correlation IDs
- Security event logging
- Global exception handling
- Centralized policy enforcement
This approach reduces duplication and enforces consistency across the system.

### Authentication and Security
- JWT-based authentication
- policy based authorization
- Email confirmation for new users
- Phone OTP verification
- User context abstraction
- Secure user-scoped data access enforcement

### Security Event Logging
Security is treated as a first-class concern with explicit security telemetry, not just application logs.
#### Tracked Security Events
The system records security-related activities using structured, queryable security event logs:
```
public enum SecurityEventTypes
{
    LoginSuccess = 1,
    LoginFailed = 2,
    TokenIssued = 3,
    Logout = 4,
    AccessDenied = 5,        // 403 Forbidden
    UnauthorizedAccess = 6  // 401 Unauthorized
}
```
#### Each security event captures:
- Event type
- User ID (nullable where applicable)
- User email
- Timestamp
- Correlation ID
- Request context metadata

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
Metrics are intentionally low-cardinality and aggregate-focused, designed for dashboards and alerting rather than per-request tracing.
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
- Export expenses, audit logs, and security event logs in
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
## 📌 Project Intent & Positioning
- Designed with production-grade backend patterns
- Incrementally designed
- Focused on backend engineering concepts
- Not production-ready by design
- Serves as:
  - A learning-driven engineering project
  - A reference architecture for real-world backend systems
  - A demonstration of backend maturity beyond CRUD

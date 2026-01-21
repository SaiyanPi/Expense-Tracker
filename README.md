# Expense Tracker API

A production-inspired, enterprise-grade backend system built with a strong focus on architecture, security, observability, and maintainability.

This project is intentionally designed to explore and apply real-world backend engineering concerns such as auditing, security event tracking, soft deletion, filtering, validation, policy-based authorization, and operational observability.

While not intended for production deployment, the system is engineered with production patterns and discipline, serving both as a deep learning platform and a realistic backend reference.

<hr>
 
## 🧠 Engineering Intent & Portfolio Positioning
This project is intentionally designed as a production-inspired backend system rather than a simple demo or tutorial application.

The primary focus is on:
- Architectural clarity
- Security-first thinking
- Observability and diagnostics
- Maintainability and long-term evolution
  
This repository serves as:
- A backend engineering portfolio project
- A reference implementation for Clean Architecture–based APIs
- A learning-driven system built with production discipline

<hr> 

## 🚀 Product Capabilities

### Core Features
- Expense management (CRUD and advanced operations)
- Category and budget management
- User-based expense ownership
- Advanced filtering and pagination
- Real-time budget threshold notifications when limits are exceeded
- Soft delete and restore functionality
- User and entity activity timelines
- Export expenses, audit logs, and security event logs data in:
   - CSV
   - Excel
   - PDF
  
 <hr>

## 🔁 API Versioning
This API is designed to support safe, long-term evolution using explicit versioning.
- URL-based API versioning (`/api/v1`, `/api/v2`)
- Versioned request/response contracts
- Version-specific controllers to prevent breaking changes
- Version-aware Swagger documentation for clear API discovery

<hr>

 ## 🔐 Authentication & Access Control
- JWT-based authentication
- Policy-based authorization
- Email confirmation for new users
- Phone OTP verification
- User context abstraction
- Secure, user-scoped data access enforcement

<hr>

## 🛡️ Security & Compliance
### Security Event Logging
Security is treated as a first-class concern with explicit security telemetry, not just application logs.
#### Tracked Security Events
The system records security-related activities using structured, queryable security event logs:
```csharp
public enum SecurityEventTypes
{
    LoginSuccess = 1,
    LoginFailed = 2,
    TokenIssued = 3,
    Logout = 4,
    AccessDenied = 5,       
    UnauthorizedAccess = 6  
}
```
#### Each security event captures:
- Event type
- User ID (nullable where applicable)
- User email
- Timestamp
- Correlation ID
- Request context metadata
#### Security Event Log Capabilities
- Get all security event logs with advanced filtering
- Get security event log by ID
- Export security event logs by filters
- Access restricted to privileged/admin users only

<hr>

## 📊 Observability & Operations
### Request Tracing & Correlation
- Correlation ID–based request tracking across:
 - Application logs
 - API responses
 - Audit logs
 - Security event logs
- Correlation ID propagated via middleware and exposed to clients
- Enables full request lifecycle tracing across layers
### Structured Logging
- Structured logging using Serilog
- Logs enriched with:
 - Correlation ID
 - User ID
 - Request metadata
- Context-aware logging across handlers, middleware, and infrastructure services
- Proper log level usage:
 - Information → business flow
 - Warning → expected failures
 - Error → unexpected or system-level faults
### Metrics (OpenTelemetry)
Metrics are intentionally low-cardinality and aggregate-focused, designed for dashboards and alerting.
- OpenTelemetry-based metrics instrumentation
- Clear distinction between system metrics and business metrics
#### System Metrics
- HTTP request duration
- Active requests
- Infrastructure health signals
- Rate limiting metrics
- Cache metrics
- Security event metric
#### Business Metrics
- Business operation latency (opt-in per use case)
- Business operation success counters
- Business operation failure counters (recorded centrally in exception middleware)
- Domain-specific metrics
### Error Handling & Diagnostics
- Centralized global exception handling middleware
- Consistent error response contracts
- Explicit handling of:
 - Domain exceptions
 - Validation exceptions
 - Infrastructure exceptions
- Client-visible correlation identifiers for easier debugging and support
### Health Monitoring
- Application health checks exposed via standard health endpoints
- Used for basic liveness and readiness signaling
- Designed to support containerized or orchestrated deployments
<hr>

## 📃 Auditing & Data Safety
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

<hr>

## 🔍 Filtering & Querying
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
 
## 🏗️ Architecture & Engineering
- Clean Architecture
- CQRS with MediatR
- FluentValidation
- DTO-based request/response models
- Explicit API versioning and contract evolution
- Middleware-driven cross-cutting concerns
- Clear separation of:
 - Domain
 - Application
 - Infrastructure
 - API layers

<hr>

## ✔️ Key Engineering Decisions
#### Clean Architecture + CQRS
- Enforces clear dependency direction
- Improves testability
- Supports long-term maintainability
- CQRS applied selectively to avoid unnecessary complexity
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
- Rate limiting, cache, and Security event metrics are included
#### Middleware-Driven Cross-Cutting Concerns
- Correlation IDs
- Security event logging
- Global exception handling
- Centralized policy enforcement

<hr>

## 🧩 Solution Structure
```
backend
├── ExpenseTracker.API            # Controllers, Middleware, Contracts
├── ExpenseTracker.Application    # CQRS, Validators, DTOs, Exceptions, Observability, Service Interface, Cross-cutting concerns
├── ExpenseTracker.Domain         # Entities, Enums, Base Models, Repository Interface
├── ExpenseTracker.Persistence    # EF Core, DbContext, Identity
└── ExpenseTracker.Infrastructure # Services, Repositories, External integrations
```

<hr>

## 🗄️ Tech Stack
- .NET 8
- ASP.NET Core
- Entity Framework Core
- MediatR
- FluentValidation
- JWT Authentication
- SQL Server
- Clean Architecture
- SignalR
- CQRS Pattern
- Serilog 
- OpenTelemetry 
- smtp4dev
- SMSGateway integration
- In-memory caching
- Rate limiting


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
- Navigate to solution directory
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

## 🚦 Project Intent & Positioning
- Designed with production-grade backend patterns
- Incrementally designed
- Focused on backend engineering concepts
- Not production-ready by design
- Serves as:
  - A reference architecture for real-world backend systems
  - A demonstration of backend maturity beyond CRUD

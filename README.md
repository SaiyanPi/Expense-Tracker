# Expense Tracker API

A Clean Architecture–based Expense Tracker backend API built with ASP.NET Core.
This project is designed as a learning-focused but real-world–oriented backend system, emphasizing proper architecture, maintainability, and backend best practices.

The goal of this project is not just CRUD functionality, but to deeply understand enterprise-grade API design, including auditing, soft deletion, filtering, validation, and authorization patterns.


## Architecture

This project follows Clean Architecture principles with strict separation of concerns.

ExpenseTracker
│
├── ExpenseTracker.API           → Presentation layer (Controllers, Middleware)
├── ExpenseTracker.Application   → Application logic (CQRS, Validators, DTOs)
├── ExpenseTracker.Domain        → Core domain (Entities, Enums, Base classes)
├── ExpenseTracker.Persistence  → Data access (EF Core, DbContext, Repositories)
└── ExpenseTracker.Infrastructure→ Cross-cutting concerns (Auth, Services)

## Features

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
- 
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


## Validation & Error Handling

- FluentValidation for:
  - Commands
  - Queries
  - Filter objects
- Centralized exception handling
- Clear validation and domain error messages


## Technologies Used
- ASP.NET Core Web API
- Entity Framework Core
- MediatR
- FluentValidation
- JWT Authentication
- SQL Server
- Clean Architecture
- CQRS Pattern

# Origin.Identity

A modern, reusable identity and authentication platform built for personal projects and applications.

`Origin.Identity` is a centralized authentication server designed to power multiple applications from a single source of truth. It provides user management, authentication, authorization, token issuance, and client management for web, mobile, desktop, and API-based applications.

Built with:

* ASP.NET Core
* ASP.NET Core Identity
* OpenIddict
* PostgreSQL

---

## Features

* Centralized authentication for all personal projects
* User registration and login
* JWT access token issuance
* Refresh token support
* Role and claim management
* OAuth 2.0 and OpenID Connect support
* Authorization Code + PKCE flow for web and mobile apps
* Support for multiple client applications
* ASP.NET Identity-based user management
* Email confirmation and password reset support
* Extensible architecture for future providers and integrations

---

## Planned Client Applications

`Origin.Identity` is intended to be shared across multiple applications, including:

* Web applications
* Mobile applications
* Desktop tools
* Games
* Internal dashboards
* Future personal projects

Example clients:

```text
origin-web
origin-mobile
music-app
game-launcher
hrms-app
```

---

## Solution Structure

```text
Origin.Identity.sln

src/
├── Origin.Identity.Api
├── Origin.Identity.Application
├── Origin.Identity.Domain
├── Origin.Identity.Infrastructure
└── Origin.Identity.Contracts

tests/
├── Origin.Identity.UnitTests
└── Origin.Identity.IntegrationTests
```

### Project Responsibilities

| Project                          | Responsibility                                                         |
| -------------------------------- | ---------------------------------------------------------------------- |
| `Origin.Identity.Api`            | Controllers, middleware, dependency injection, application startup     |
| `Origin.Identity.Application`    | Business logic, services, interfaces, validation, result handling      |
| `Origin.Identity.Domain`         | Core domain entities, enums, constants, domain rules                   |
| `Origin.Identity.Infrastructure` | Database, ASP.NET Identity, OpenIddict, persistence, external services |
| `Origin.Identity.Contracts`      | Request and response DTOs shared across the API boundary               |

---

## Architecture

```text
Client Application
        ↓
Origin.Identity.Api
        ↓
Application Layer
        ↓
Domain Layer
        ↓
Infrastructure Layer
        ↓
Database
```

Dependency flow:

```text
Api            → Application, Contracts
Application    → Domain, Contracts
Infrastructure → Application, Domain
Contracts      → none
Domain         → none
```

---

## Authentication Flows

| Scenario         | Flow                      |
| ---------------- | ------------------------- |
| Angular / SPA    | Authorization Code + PKCE |
| Flutter / Mobile | Authorization Code + PKCE |
| Server-to-Server | Client Credentials        |
| Internal APIs    | JWT Bearer Tokens         |

---

## Core Endpoints

| Endpoint                         | Description                      |
| -------------------------------- | -------------------------------- |
| `POST /api/auth/register`        | Register a new user              |
| `POST /api/auth/login`           | Authenticate a user              |
| `POST /api/auth/refresh`         | Refresh an access token          |
| `POST /api/auth/logout`          | Revoke the current session       |
| `POST /api/auth/forgot-password` | Send password reset instructions |
| `POST /api/auth/reset-password`  | Reset a user's password          |
| `GET /api/users/{id}`            | Get a user by ID                 |
| `GET /api/users`                 | Get all users                    |
| `PUT /api/users/{id}`            | Update a user                    |
| `DELETE /api/users/{id}`         | Delete a user                    |

---

## Technology Stack

* ASP.NET Core 10
* Entity Framework Core
* OpenIddict
* ASP.NET Core Identity
* PostgreSQL

---

## Getting Started

### Prerequisites

* .NET SDK 10
* PostgreSQL
* Optional: Docker

### Clone the Repository

```bash
git clone <repository-url>
cd Origin.Identity
```

### Configure Environment

Create an `appsettings.Development.json` file inside `Origin.Identity.Api`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=origin_identity;Username=postgres;Password=your_password"
  }
}
```

### Apply Database Migrations

```bash
dotnet ef database update \
  --project src/Origin.Identity.Infrastructure \
  --startup-project src/Origin.Identity.Api
```

### Run the Application

```bash
dotnet run --project src/Origin.Identity.Api
```

By default, the API will be available at:

```text
https://localhost:5001
http://localhost:5000
```

---

## Future Roadmap

* External login providers

  * Google
  * GitHub
  * Microsoft
* Multi-factor authentication
* Device management
* Session tracking
* Email verification
* Audit logs
* Multi-tenant support
* Admin dashboard
* API key management
* Fine-grained permissions
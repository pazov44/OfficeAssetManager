# Office Asset Manager

A modern, robust, and secure full-stack web application designed to streamline the tracking, allocation, and lifecycle management of organizational office assets. Built with a scalable decoupled architecture using an **ASP.NET Core Web API** backend and a reactive **React (Vite)** frontend.

🔗 **Live Production Demo:** [https://asset-manager-alpha-mauve.vercel.app](https://asset-manager-alpha-mauve.vercel.app)

---

## Architecture Overview

The system utilizes a split-hosting strategy engineered for high efficiency and seamless scalability:

* **Frontend Client:** Deployed on **Vercel** for lightning-fast Edge distribution.
* **Web API Backend:** Deployed on **Render** utilizing a Kestrel container architecture.
* **Database Cluster:** Serverless PostgreSQL cluster hosted securely on **Neon Tech**.

---

## Tech Stack & Key Features

### Web API
* **Framework:** ASP.NET Core 8.0 Web API with C# clean architecture principles.
* **Identity & Security:** ASP.NET Core Identity integrated with stateless **JWT Bearer Token Authentication**.
* **Data Tier:** Entity Framework Core (EF Core) using the **Repository Pattern** for strict data isolation.
* **Validation Rules:** Comprehensive backend validation mapping including global authorization filters.
* **Documentation:** Automated OpenAPI documentation generation via Swagger in Development Environment.

### Client
* **Core Framework:** React (Vite) with explicit state management hooks.
* **Routing Architecture:** Client-side path management handled by `react-router-dom`.
* **Forms & Validation:** Real-time, contextual UI input validators (alphanumeric constraints, dynamic password policy meters).
* **Asynchronous I/O:** Promise-based HTTP communications mapped via unified API services.

---

## Security & System Requirements

### Password Complexity Controls
To satisfy enterprise constraints, the platform enforces strict security validation constraints across both the frontend validation engine and backend Core identity services.

### Role-Based Access Control (RBAC)
* **Automatic Admin Bootstrap:** The system checks database state upon user registration. The **very first user** to register automatically receives the explicit `Admin` role to securely set up the platform layout.
* **Standard Profiles:** All subsequent registrants default strictly to the `User` role.

---

### Environment Configuration

To ensure continuous integration and eliminate environment alignment mismatches, the application requires specific variables configured across local and cloud environments.

#### Backend Configurations (`Render / Local Docker-Compose`)
When configuring backend service layers, variables mapping to C# configuration providers must utilize the standard hierarchical PascalCase delimiter (`__` double underscore):

```env
# Database Connection
DB_CONNECTION_STRING="Host=your-database-host;Database=your-database-name;Username=your-database-username;Password=your-database-password"

# JWT Access Configuration
Jwt__Key="your-secret-key-min-256-bits-long"
Jwt__Issuer="your-api-issuer"
Jwt__Audience="your-api-audience"

# CORS Access Policies
CorsSettings__AllowedOrigins__0="your-allowed-origin"
```

#### Frontend Build-Time Configurations (`Vercel / Local .env`)
Because the React client compiles directly into static browser assets, its environment configurations must be injected during the build process. Vite requires all client-facing variables to be explicitly prefixed with `VITE_`:

```env
# API Base URL Used By Axios Client to Communicate with the C# Backend
VITE_API_BASE_URL="your-api-base-url"
```
---

### Containerized Execution
The most efficient way to spin up the entire multi-container ecosystem (the C# Web API backend and the React Nginx frontend working together) is by using Docker Compose. This replicates the production architecture right on your local machine.

#### Spin up all containers and connect the network bridge
```bash
docker compose up
```

#### Rebuild images from scratch (run this after changing code, dependencies, or configs)
```bash
docker compose up --build
```

#### Stop running containers safely
```bash
docker compose down
```

#### Stop containers and completely wipe local database persistent volumes
```bash
docker compose down -v
```

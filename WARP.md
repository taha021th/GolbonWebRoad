# WARP.md

This file provides guidance to WARP (warp.dev) when working with code in this repository.

## Project Overview

This is a .NET 8.0 e-commerce solution called GolbonWebRoad that implements Clean Architecture principles. The solution contains multiple projects organized in a layered architecture with both API and Web frontend components.

## Architecture

This solution follows Clean Architecture patterns with the following projects:

- **GolbonWebRoad.Domain** - Core business entities and interfaces
- **GolbonWebRoad.Application** - Application services, DTOs, CQRS handlers using MediatR
- **GolbonWebRoad.Infrastructure** - Data access, repositories, external services, Entity Framework Core
- **GolbonWebRoad.Api** - REST API with JWT authentication, Swagger documentation
- **GolbonWebRoad.Web** - MVC web application with Razor views and Tailwind CSS
- **GolbonWebRoad.Admin.Web** - Admin panel (referenced in solution but files not present)

Key architectural patterns:
- **Repository Pattern** with Unit of Work
- **CQRS** with MediatR for command/query separation
- **AutoMapper** for DTO mappings
- **Identity Framework** for authentication/authorization
- **Entity Framework Core** with PostgreSQL

## Common Development Commands

### Building the Solution
```powershell
# Build entire solution
dotnet build

# Build specific project
dotnet build GolbonWebRoad.Api
dotnet build GolbonWebRoad.Web

# Build in Release mode
dotnet build --configuration Release
```

### Running Applications
```powershell
# Run API project
dotnet run --project GolbonWebRoad.Api

# Run Web project  
dotnet run --project GolbonWebRoad.Web

# Run with specific environment
dotnet run --project GolbonWebRoad.Api --environment Development
```

### Database Operations
```powershell
# Add new migration
dotnet ef migrations add "MigrationName" --project GolbonWebRoad.Infrastructure --startup-project GolbonWebRoad.Api

# Update database
dotnet ef database update --project GolbonWebRoad.Infrastructure --startup-project GolbonWebRoad.Api

# Drop database (careful!)
dotnet ef database drop --project GolbonWebRoad.Infrastructure --startup-project GolbonWebRoad.Api
```

### Frontend Development (Tailwind CSS)
```powershell
# Navigate to Web project
cd GolbonWebRoad.Web

# Build CSS (production)
npm run build

# Watch CSS changes (development)
npm run watch

# Install/update npm packages
npm install
```

### Package Management
```powershell
# Add NuGet package to specific project
dotnet add GolbonWebRoad.Application package PackageName

# Remove package
dotnet remove GolbonWebRoad.Application package PackageName

# Restore packages
dotnet restore
```

## Project Configuration

### Database Configuration
- **API Project**: Uses PostgreSQL (connection string: "GolbonWebRoadShopDbConnection")
- **Web Project**: Uses SQL Server (connection string: "GolbonWebRoadDbContextConnection")
- Both projects have separate database configurations in their respective appsettings.json

### Key Technologies
- **.NET 8.0** - Target framework
- **Entity Framework Core 8.0** - ORM
- **ASP.NET Core Identity** - Authentication/Authorization  
- **MediatR** - CQRS pattern implementation
- **AutoMapper** - Object mapping
- **Serilog** - Structured logging with Seq integration
- **FluentValidation** - Input validation
- **Swagger/OpenAPI** - API documentation (API project only)
- **Tailwind CSS** - Utility-first CSS framework (Web project)

### Authentication & Authorization
- **JWT Bearer Token** authentication for API
- **Cookie-based** authentication for Web application
- Identity seeding configured in both applications
- Custom authentication error messages in Persian

### Logging Configuration
- **Serilog** configured with multiple sinks:
  - Console output (development)
  - File logging
  - Seq integration for structured logging
- Custom logging configuration in `GolbonWebRoad.Api.DependecyInjection.cs`
- Environment-specific log levels

## Important File Locations

### Configuration Files
- Solution file: `GolbonWebRoad.sln`
- API settings: `GolbonWebRoad.Api/appsettings.json`
- Web settings: `GolbonWebRoad.Web/appsettings.json`
- Tailwind config: `GolbonWebRoad.Web/tailwind.config.js`
- Node.js config: `GolbonWebRoad.Web/package.json`

### Key Source Files
- DbContext: `GolbonWebRoad.Infrastructure/Persistence/GolbonWebRoadDbContext.cs`
- Dependency injection: `*/DependencyInjection.cs` in each project
- Mapping profiles: `*/Mapping/MappingProfile.cs` in Application and Web projects
- Domain entities: `GolbonWebRoad.Domain/Entities/`
- API controllers: `GolbonWebRoad.Api/Controllers/`
- Web controllers: `GolbonWebRoad.Web/Controllers/`

## Development Notes

### Entity Framework Considerations
- Two different database providers are configured (PostgreSQL for API, SQL Server for Web)
- Migration files are located in `GolbonWebRoad.Infrastructure/Migrations/`
- Database seeding is configured for Identity users/roles

### CQRS Pattern Implementation
- Commands and queries are organized under `GolbonWebRoad.Application/Features/`
- Each feature has separate folders for Commands and Queries
- MediatR handles request/response pipeline

### Payment Integration
- Payment gateway integration (AqayepardakhtSandboxGateway) is configured in the Web project
- Custom payment service implementations in `GolbonWebRoad.Web/Services/Payments/`

### Multilingual Support
- Some error messages and UI text are in Persian
- Consider this when making changes to user-facing text
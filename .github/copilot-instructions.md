# GitHub Copilot Instructions for QMSServer

## Technology Stack

This is a Queue Management System (QMS) server built with:

- **.NET 9.0** - Target framework
- **ASP.NET Core** - Web API framework
- **Entity Framework Core 9.0** - ORM for database access
- **MySQL 8** - Database server
- **SignalR** - Real-time communication
- **MediatR** - Mediator pattern implementation
- **AutoMapper** - Object-object mapping
- **Scalar** - API documentation (OpenAPI)
- **Docker** - Containerization

## Project Structure

```
QMSServer/
├── QMS/                          # Main application project
│   ├── Controllers/              # API Controllers
│   ├── DAL/                      # Data Access Layer (Repositories)
│   ├── DTO/                      # Data Transfer Objects
│   ├── Db/                       # Database context
│   ├── Hubs/                     # SignalR hubs
│   ├── Mediator/                 # MediatR events and handlers
│   ├── Migrations/               # EF Core migrations
│   ├── Program.cs                # Application entry point
│   └── QMS.csproj                # Project file
├── QMSServer.sln                 # Solution file
└── docker-compose.local.yml      # Local development Docker setup
```

## Development Setup

### Prerequisites

- .NET 9.0 SDK
- MySQL 8 or Docker Desktop
- Visual Studio 2022 or JetBrains Rider (optional)

### Database Setup

1. Start MySQL using Docker Compose:
   ```bash
   docker-compose -f docker-compose.local.yml up -d
   ```

2. Apply database migrations:
   ```bash
   cd QMS
   dotnet ef database update
   ```

### Running the Application

```bash
cd QMS
dotnet run
```

The API will be available at `https://localhost:7182` (or the configured HTTPS port).

## Coding Conventions

### General Guidelines

- Use C# 12 features with nullable reference types enabled
- Follow standard C# naming conventions (PascalCase for classes/methods, camelCase for parameters)
- Keep implicit usings enabled as per project configuration
- Use dependency injection for all services and repositories

### Architecture Patterns

- **Repository Pattern**: Data access is abstracted through repositories (see `DAL/` folder)
- **CQRS with MediatR**: Use MediatR for command/query separation and event handling
- **DTO Pattern**: Use DTOs for API request/response models (see `DTO/` folder)
- **SignalR Hubs**: Real-time notifications through SignalR hubs (see `Hubs/` folder)

### Database

- Use Entity Framework Core for all database operations
- Create migrations for schema changes:
  ```bash
  dotnet ef migrations add <MigrationName>
  ```
- Repository interfaces should be defined in the `DAL/` folder
- Database context is `QmsDbContext` located in `Db/` folder

### API Development

- All controllers should inherit from `ControllerBase`
- Use proper HTTP status codes
- Use DTOs for request/response models, not entity models directly
- OpenAPI documentation is automatically generated and viewable via Scalar at `/scalar/v1` in development

### SignalR

- Hubs are located in the `Hubs/` folder
- Current hub: `QueueHub` at endpoint `/hubs/queue`
- Use MediatR notification handlers to send SignalR messages (see `Mediator/` folder)

## Configuration

### Connection Strings

- Development: Check `appsettings.Development.json`
- Default connection string format: `Server=localhost;Port=3306;Database=appdb;User=appuser;Password=apppass;`
- Connection strings are configured in `appsettings.json`

### CORS

- Configured to allow origins starting with `https://localhost:7182`
- Modify CORS policy in `Program.cs` if needed for additional origins

## Testing

- No test projects currently exist in the solution
- When adding tests, follow .NET testing best practices with xUnit or NUnit

## Docker

- Dockerfile is available in the `QMS/` folder
- Local development uses `docker-compose.local.yml` for MySQL
- Docker configuration uses Linux as target OS

## Key Dependencies

- **AutoMapper**: Mapping configuration in `DAL/MappingProfile.cs`
- **MediatR**: Events and handlers in `Mediator/` folder
- **SignalR**: Hubs in `Hubs/` folder
- **EF Core Tools**: For migrations and database updates

## Common Tasks

### Adding a New Entity

1. Create the entity class in `Db/` folder
2. Add DbSet to `QmsDbContext`
3. Create and apply migration
4. Create repository interface and implementation in `DAL/`
5. Register repository in `Program.cs`
6. Create corresponding DTOs in `DTO/` folder
7. Update AutoMapper profile in `DAL/MappingProfile.cs`

### Adding a New API Endpoint

1. Create or update controller in `Controllers/`
2. Create DTOs if needed in `DTO/`
3. Implement repository methods if data access is needed
4. Use MediatR for complex business logic or notifications

### Adding SignalR Notifications

1. Create event class in `Mediator/` (inherit from `INotification`)
2. Create handler in `Mediator/` (implement `INotificationHandler<TEvent>`)
3. Inject `IHubContext<QueueHub>` in handler
4. Publish event using `IMediator.Publish()`

## Build and Deployment

### Build

```bash
dotnet build QMSServer.sln
```

### Restore Packages

```bash
dotnet restore
```

### Clean Build

```bash
dotnet clean && dotnet build
```

## Important Notes

- User secrets are configured with ID: `df39ff31-f78f-4c76-8c94-a41b62bafbfa`
- Always test database migrations before deploying to production
- Ensure CORS settings are properly configured for production environments
- API documentation is only available in development mode

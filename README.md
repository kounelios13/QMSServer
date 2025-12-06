# QMSServer

A Queue Management System (QMS) server built with ASP.NET Core 9.0, providing REST API and SignalR real-time communication for managing customer queues and front desk terminals.

## Features

- Ticket creation and management
- Front desk device registration
- Real-time queue updates via SignalR
- RESTful API endpoints
- MySQL database support

## Configuration

### CORS Settings

CORS origins are now configurable through `appsettings.json`:

```json
{
    "Cors": {
        "AllowedOrigins": [
            "https://localhost:7182",
            "https://yourdomain.com"
        ]
    }
}
```

You can also set CORS origins via environment variables:
```bash
Cors__AllowedOrigins__0=https://localhost:7182
Cors__AllowedOrigins__1=https://yourdomain.com
```

### Database Connection

**Important**: Never commit database credentials to source control!

#### Development (User Secrets)
```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Port=3306;Database=<database>;User=<username>;Password=<password>"
```

#### Production (Environment Variables)
```bash
# Linux/Mac
export ConnectionStrings__DefaultConnection="Server=prod-server;Port=3306;Database=<database>;User=<username>;Password=<password>"

# Windows
set ConnectionStrings__DefaultConnection=Server=prod-server;Port=3306;Database=<database>;User=<username>;Password=<password>
```

See `.env.example` for more configuration options.

## Security

This application includes several security improvements:
- Configurable CORS origins
- No hardcoded credentials in source code
- Proper null reference handling

For a complete security analysis and recommendations, see [SECURITY_ANALYSIS.md](SECURITY_ANALYSIS.md).

### Security Recommendations for Production

1. **Add Authentication/Authorization**: Implement JWT or ASP.NET Core Identity
2. **Enable HTTPS**: Ensure all traffic is encrypted
3. **Use Secret Management**: Azure Key Vault, AWS Secrets Manager, or similar
4. **Implement Rate Limiting**: Prevent API abuse
5. **Add Input Validation**: Use FluentValidation or data annotations
6. **Regular Security Audits**: Keep dependencies updated

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- MySQL Server 8.0+

### Installation

1. Clone the repository
```bash
git clone https://github.com/kounelios13/QMSServer.git
cd QMSServer
```

2. Configure database connection (use user secrets for development)
```bash
cd QMS
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Port=3306;Database=<database>;User=<username>;Password=<password>"
```

3. Run database migrations
```bash
dotnet ef database update
```

4. Run the application
```bash
dotnet run
```

### API Documentation

When running in development mode, API documentation is available via Scalar:
- Navigate to `/scalar/v1` for interactive API documentation

## Development

### Build
```bash
dotnet build
```

### Run
```bash
dotnet run
```

### Docker Support

Docker configuration is available in the repository. See `docker-compose.local.yml` for local development setup.

## License

[License information to be added]

## Contributing

[Contributing guidelines to be added]

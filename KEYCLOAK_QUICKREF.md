# Keycloak Quick Reference Card

## Quick Start Commands

### Start Keycloak Locally
```bash
# Linux/macOS
./setup-keycloak.sh

# Windows
setup-keycloak.bat

# Or manually with Docker Compose
docker-compose -f docker-compose.keycloak.yml up -d
```

### Configure Application
```bash
# Using User Secrets (Development)
cd QMS
dotnet user-secrets set "Keycloak:Authority" "http://localhost:8080"
dotnet user-secrets set "Keycloak:Realm" "qms-realm"
dotnet user-secrets set "Keycloak:ClientId" "qms-api"

# Using Environment Variables
export Keycloak__Authority="http://localhost:8080"
export Keycloak__Realm="qms-realm"
export Keycloak__ClientId="qms-api"
```

### Get Access Token
```bash
curl -X POST "http://localhost:8080/realms/qms-realm/protocol/openid-connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password" \
  -d "client_id=qms-api" \
  -d "username=admin" \
  -d "password=admin123"
```

### Call Protected API
```bash
curl -X GET "https://localhost:7182/api/Tickets" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
```

## Keycloak Setup Checklist

- [ ] 1. Start Keycloak server
- [ ] 2. Access Admin Console at http://localhost:8080/admin
- [ ] 3. Create realm: `qms-realm`
- [ ] 4. Create client: `qms-api`
- [ ] 5. Create roles: `qms-admin`, `qms-frontdesk`, `qms-user`
- [ ] 6. Create users and assign roles
- [ ] 7. Configure QMSServer with Keycloak settings
- [ ] 8. Test authentication

## Roles & Permissions

| Role | Permissions |
|------|-------------|
| `qms-admin` | Full access to all endpoints |
| `qms-frontdesk` | Register devices, acquire tickets, view tickets |
| `qms-user` | View tickets, connect to SignalR hub |

## Endpoint Authorization

### Public (No Auth)
- `POST /api/Tickets/Create` - Create ticket from kiosk
- `WS /hubs/queue` - SignalR connection for real-time updates

### Authenticated (Any User)
- `GET /api/Tickets` - List all tickets
- `GET /api/Tickets/{id}` - Get ticket details
- `GET /api/Tickets/Status/{status}` - Get tickets by status

### Front Desk Staff
- `POST /api/FrontDeskDevice/Register` - Register device
- `POST /api/Tickets/AcquireTicket` - Acquire next ticket
- `PUT /api/Tickets/{id}/Status` - Update ticket status (complete tickets)

### Admin Only
- `GET /api/FrontDeskDevice/Devices` - List all devices
- `DELETE /api/Tickets/{id}` - Delete ticket
- `POST /api/Tickets/Reset` - Reset all tickets

## Configuration Options

### Minimal (Development)
```json
{
  "Keycloak": {
    "Authority": "http://localhost:8080",
    "Realm": "qms-realm",
    "ClientId": "qms-api",
    "RequireHttpsMetadata": false
  }
}
```

### Production
```json
{
  "Keycloak": {
    "Authority": "https://keycloak.your-domain.com",
    "Realm": "qms-realm",
    "ClientId": "qms-api",
    "ClientSecret": "USE_SECRETS_MANAGER",
    "RequireHttpsMetadata": true,
    "ValidateIssuer": true,
    "ValidateAudience": true,
    "ValidateLifetime": true,
    "ClockSkewSeconds": 300
  }
}
```

## Common Issues

### 401 Unauthorized
- ✓ Check token is valid (not expired)
- ✓ Verify Authorization header format: `Bearer TOKEN`
- ✓ Confirm Keycloak configuration is correct

### 403 Forbidden
- ✓ Verify user has required role in Keycloak
- ✓ Check role claim mapping in client scopes
- ✓ Ensure `UseRealmRoles` setting matches your setup

### Cannot connect to Keycloak
- ✓ Verify Keycloak server is running
- ✓ Check Authority URL is correct
- ✓ Test well-known endpoint: `GET {Authority}/realms/{Realm}/.well-known/openid-configuration`

### SignalR Connection Issues
- ✓ Verify the hub URL is correct: `/hubs/queue`
- ✓ Check CORS settings allow your client origin
- ✓ Ensure WebSocket support is enabled
- ✓ Note: SignalR hub allows anonymous connections for public kiosks

## Useful Commands

```bash
# Stop Keycloak
docker-compose -f docker-compose.keycloak.yml down

# View Keycloak logs
docker logs qms-keycloak

# Decode JWT token (at jwt.io)
echo "YOUR_TOKEN" | base64 -d

# Test API endpoint
curl -v -X GET "https://localhost:7182/api/Tickets" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -k  # Skip SSL verification for localhost
```

## Resources

📖 **Full Documentation**: [KEYCLOAK_INTEGRATION.md](KEYCLOAK_INTEGRATION.md)
📋 **Implementation Details**: [KEYCLOAK_IMPLEMENTATION.md](KEYCLOAK_IMPLEMENTATION.md)
🔒 **Security Analysis**: [SECURITY_ANALYSIS.md](SECURITY_ANALYSIS.md)
📘 **README**: [README.md](README.md)

## Support

- GitHub Issues: Report bugs and request features
- Keycloak Docs: https://www.keycloak.org/documentation
- ASP.NET Core Security: https://learn.microsoft.com/en-us/aspnet/core/security/

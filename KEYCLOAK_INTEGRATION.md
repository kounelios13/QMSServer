# Keycloak Integration Guide

This guide provides step-by-step instructions for integrating Keycloak authentication with the QMSServer application.

## Table of Contents
1. [Prerequisites](#prerequisites)
2. [Keycloak Setup](#keycloak-setup)
3. [Application Configuration](#application-configuration)
4. [Testing the Integration](#testing-the-integration)
5. [Authorization Roles and Policies](#authorization-roles-and-policies)
6. [Best Practices](#best-practices)
7. [Troubleshooting](#troubleshooting)

## Prerequisites

- Keycloak server instance (version 20+ recommended)
- QMSServer application
- .NET 9.0 SDK

## Keycloak Setup

### Step 1: Create a Realm

1. Log in to your Keycloak Admin Console (usually at `http://localhost:8080/admin` or `https://your-keycloak-server/admin`)
2. Click on the realm dropdown (top left) and select **"Create Realm"**
3. Enter realm name: `qms-realm`
4. Click **"Create"**

### Step 2: Create a Client

1. In the left menu, click **"Clients"**
2. Click **"Create client"**
3. Configure the client:
   - **Client type**: OpenID Connect
   - **Client ID**: `qms-api`
   - Click **"Next"**

4. Client authentication settings:
   - **Client authentication**: ON (for confidential client) or OFF (for public client)
   - **Authorization**: OFF (not needed for API)
   - **Authentication flow**: 
     - ✅ Standard flow
     - ✅ Direct access grants
     - ✅ Service accounts roles (if using confidential client)
   - Click **"Next"**

5. Login settings:
   - **Root URL**: `https://localhost:7182` (your API URL)
   - **Valid redirect URIs**: 
     - `https://localhost:7182/*`
     - `http://localhost:3000/*` (if you have a frontend)
   - **Valid post logout redirect URIs**: `+` (same as redirect URIs)
   - **Web origins**: 
     - `https://localhost:7182`
     - `http://localhost:3000` (if you have a frontend)
   - Click **"Save"**

6. If you enabled **Client authentication**, go to the **"Credentials"** tab and copy the **Client Secret**. You'll need this later.

### Step 3: Create Roles

1. In the left menu, click **"Realm roles"**
2. Click **"Create role"** and create the following roles:

   **Admin Role:**
   - Role name: `qms-admin`
   - Description: `Administrator role with full access`
   - Click **"Save"**

   **Front Desk Role:**
   - Role name: `qms-frontdesk`
   - Description: `Front desk staff role for managing queues`
   - Click **"Save"**

   **User Role:**
   - Role name: `qms-user`
   - Description: `Standard user role for viewing queues`
   - Click **"Save"**

### Step 4: Create Users

1. In the left menu, click **"Users"**
2. Click **"Add user"**
3. Fill in user details:
   - **Username**: `admin` (or your preferred username)
   - **Email**: `admin@example.com`
   - **First name**: `Admin`
   - **Last name**: `User`
   - **Email verified**: ON
   - Click **"Create"**

4. Set user password:
   - Go to the **"Credentials"** tab
   - Click **"Set password"**
   - Enter password (e.g., `admin123`)
   - Set **Temporary**: OFF (so user doesn't need to change password on first login)
   - Click **"Save"**

5. Assign roles to user:
   - Go to the **"Role mapping"** tab
   - Click **"Assign role"**
   - Filter by **"Realm roles"**
   - Select `qms-admin` role
   - Click **"Assign"**

6. Repeat steps 2-5 to create additional users with different roles:
   - Create a front desk user with `qms-frontdesk` role
   - Create a regular user with `qms-user` role

### Step 5: Configure Client Scopes (Optional but Recommended)

For proper role mapping in JWT tokens:

1. In the left menu, click **"Client scopes"**
2. Find and click on the `roles` client scope
3. Go to the **"Mappers"** tab
4. Click on `realm roles` mapper
5. Ensure the following settings:
   - **Token Claim Name**: `realm_access.roles`
   - **Add to ID token**: ON
   - **Add to access token**: ON
   - **Add to userinfo**: ON
6. Click **"Save"**

## Application Configuration

### Option 1: Using User Secrets (Recommended for Development)

```bash
cd QMS
dotnet user-secrets set "Keycloak:Authority" "http://localhost:8080"
dotnet user-secrets set "Keycloak:Realm" "qms-realm"
dotnet user-secrets set "Keycloak:ClientId" "qms-api"
# Only if using confidential client:
dotnet user-secrets set "Keycloak:ClientSecret" "your-client-secret-here"
```

### Option 2: Using Environment Variables

**Linux/macOS:**
```bash
export Keycloak__Authority="http://localhost:8080"
export Keycloak__Realm="qms-realm"
export Keycloak__ClientId="qms-api"
# Only if using confidential client:
export Keycloak__ClientSecret="your-client-secret-here"
```

**Windows (CMD):**
```cmd
set Keycloak__Authority=http://localhost:8080
set Keycloak__Realm=qms-realm
set Keycloak__ClientId=qms-api
set Keycloak__ClientSecret=your-client-secret-here
```

**Windows (PowerShell):**
```powershell
$env:Keycloak__Authority="http://localhost:8080"
$env:Keycloak__Realm="qms-realm"
$env:Keycloak__ClientId="qms-api"
$env:Keycloak__ClientSecret="your-client-secret-here"
```

### Option 3: Using appsettings.Production.json

Create `appsettings.Production.json` (and add it to `.gitignore`):

```json
{
  "Keycloak": {
    "Authority": "https://keycloak.your-domain.com",
    "Realm": "qms-realm",
    "ClientId": "qms-api",
    "ClientSecret": "your-client-secret-here",
    "RequireHttpsMetadata": true,
    "ValidateIssuer": true,
    "ValidateAudience": true,
    "ValidateLifetime": true,
    "ClockSkewSeconds": 300
  }
}
```

### Configuration Options Explained

| Option | Description | Default | Production Value |
|--------|-------------|---------|------------------|
| `Authority` | Keycloak server base URL | - | `https://keycloak.your-domain.com` |
| `Realm` | Keycloak realm name | - | `qms-realm` |
| `ClientId` | Client ID registered in Keycloak | - | `qms-api` |
| `ClientSecret` | Client secret (confidential clients only) | - | Your secret |
| `RequireHttpsMetadata` | Require HTTPS for OpenID metadata | `true` | `true` (always in production) |
| `ValidateIssuer` | Validate the token issuer | `true` | `true` |
| `ValidateAudience` | Validate the token audience | `true` | `true` |
| `ValidateLifetime` | Validate token expiration | `true` | `true` |
| `ClockSkewSeconds` | Clock skew tolerance for expiration | `300` | `300` (5 minutes) |

## Testing the Integration

### 1. Get an Access Token

Use the Keycloak token endpoint to get an access token:

```bash
curl -X POST "http://localhost:8080/realms/qms-realm/protocol/openid-connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password" \
  -d "client_id=qms-api" \
  -d "username=admin" \
  -d "password=admin123"
```

If using a confidential client (client authentication enabled), add the client secret:

```bash
curl -X POST "http://localhost:8080/realms/qms-realm/protocol/openid-connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password" \
  -d "client_id=qms-api" \
  -d "client_secret=your-client-secret-here" \
  -d "username=admin" \
  -d "password=admin123"
```

The response will include an `access_token`:

```json
{
  "access_token": "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expires_in": 300,
  "refresh_expires_in": 1800,
  "refresh_token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "token_type": "Bearer"
}
```

### 2. Test API Endpoints

**Public Endpoint (No Auth Required):**
```bash
curl -X POST "https://localhost:7182/api/Tickets/Create"
```

**Authenticated Endpoint:**
```bash
curl -X GET "https://localhost:7182/api/Tickets" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
```

**Admin-Only Endpoint:**
```bash
curl -X DELETE "https://localhost:7182/api/Tickets/{ticket-id}" \
  -H "Authorization: Bearer YOUR_ADMIN_ACCESS_TOKEN"
```

### 3. Test SignalR Connection

For SignalR connections, pass the access token as a query parameter:

```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7182/hubs/queue?access_token=YOUR_ACCESS_TOKEN")
    .build();

await connection.start();
```

## Authorization Roles and Policies

### Policies Defined

The application defines three authorization policies:

| Policy | Required Roles | Description |
|--------|---------------|-------------|
| `AdminPolicy` | `qms-admin` | Full access to all endpoints |
| `FrontDeskPolicy` | `qms-frontdesk`, `qms-admin` | Access to front desk operations |
| `PublicPolicy` | Any authenticated user | Basic authenticated access |

### Endpoint Authorization Matrix

| Endpoint | Method | Required Policy | Description |
|----------|--------|----------------|-------------|
| `/api/Tickets/Create` | POST | Anonymous | Create a ticket (public kiosk) |
| `/api/Tickets` | GET | PublicPolicy | Get all tickets |
| `/api/Tickets/{id}` | GET | PublicPolicy | Get specific ticket |
| `/api/Tickets/Status/{status}` | GET | PublicPolicy | Get tickets by status |
| `/api/Tickets/{id}/Status` | PUT | PublicPolicy | Update ticket status |
| `/api/Tickets/{id}` | DELETE | AdminPolicy | Delete ticket (admin only) |
| `/api/Tickets/AcquireTicket` | POST | FrontDeskPolicy | Acquire next ticket |
| `/api/Tickets/Reset` | POST | AdminPolicy | Reset all tickets (admin only) |
| `/api/FrontDeskDevice/Register` | POST | FrontDeskPolicy | Register front desk device |
| `/api/FrontDeskDevice/Devices` | GET | AdminPolicy | Get all devices (admin only) |
| `/hubs/queue` | WebSocket | PublicPolicy | SignalR real-time updates |

### Customizing Roles

To customize roles, edit `/QMS/Configuration/AuthorizationPolicies.cs`:

```csharp
public static class AuthorizationPolicies
{
    // Policy Names
    public const string AdminPolicy = "AdminPolicy";
    public const string FrontDeskPolicy = "FrontDeskPolicy";
    public const string PublicPolicy = "PublicPolicy";

    // Keycloak Role Names - Change these to match your Keycloak roles
    public const string AdminRole = "qms-admin";
    public const string FrontDeskRole = "qms-frontdesk";
    public const string UserRole = "qms-user";

    // Set to true for realm roles, false for client roles
    public const bool UseRealmRoles = true;
}
```

#### Using Client Roles Instead of Realm Roles

If you prefer to use client-specific roles:

1. In Keycloak:
   - Go to **Clients** → `qms-api` → **Roles** tab
   - Create roles: `admin`, `frontdesk`, `user`
   - Assign these client roles to users via **Users** → Select User → **Role mapping** → **Assign role** → Filter by client

2. In your application, update `AuthorizationPolicies.cs`:
   ```csharp
   public const bool UseRealmRoles = false;
   ```

## Best Practices

### 1. Use HTTPS in Production

Always use HTTPS for your Keycloak server and API in production:

```json
{
  "Keycloak": {
    "Authority": "https://keycloak.your-domain.com",
    "RequireHttpsMetadata": true
  }
}
```

### 2. Secure Your Client Secret

- **Never** commit client secrets to source control
- Use environment variables, user secrets, or secure vaults (Azure Key Vault, AWS Secrets Manager)
- Rotate secrets regularly

### 3. Configure CORS Properly

Update `appsettings.json` to include your frontend origin:

```json
{
  "Cors": {
    "AllowedOrigins": [
      "https://your-frontend.com",
      "https://admin.your-frontend.com"
    ]
  }
}
```

### 4. Token Expiration

Configure appropriate token lifetimes in Keycloak:

1. Go to **Realm settings** → **Tokens** tab
2. Set reasonable values:
   - **Access Token Lifespan**: 5-15 minutes
   - **Client Session Idle**: 30 minutes
   - **Client Session Max**: 10 hours

### 5. Use Refresh Tokens

Implement refresh token logic in your frontend to maintain user sessions:

```javascript
async function refreshAccessToken(refreshToken) {
  const response = await fetch('http://localhost:8080/realms/qms-realm/protocol/openid-connect/token', {
    method: 'POST',
    headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
    body: new URLSearchParams({
      grant_type: 'refresh_token',
      client_id: 'qms-api',
      refresh_token: refreshToken
    })
  });
  return await response.json();
}
```

### 6. Implement Logout

Call the Keycloak logout endpoint when users sign out:

```bash
curl -X POST "http://localhost:8080/realms/qms-realm/protocol/openid-connect/logout" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "client_id=qms-api" \
  -d "refresh_token=YOUR_REFRESH_TOKEN"
```

### 7. Monitor and Log

Enable logging for authentication issues:

```json
{
  "Logging": {
    "LogLevel": {
      "Microsoft.AspNetCore.Authentication": "Debug",
      "Microsoft.AspNetCore.Authorization": "Debug"
    }
  }
}
```

### 8. Use Service Accounts for Backend Communication

For service-to-service communication:

1. In Keycloak, create a client with **Service accounts roles** enabled
2. Get a token using client credentials grant:

```bash
curl -X POST "http://localhost:8080/realms/qms-realm/protocol/openid-connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=client_credentials" \
  -d "client_id=qms-api" \
  -d "client_secret=your-client-secret-here"
```

## Troubleshooting

### Issue: "401 Unauthorized" on all endpoints

**Possible causes:**
1. Token is expired
2. Token is not included in the Authorization header
3. Keycloak configuration is incorrect

**Solutions:**
- Get a fresh token
- Ensure the header is: `Authorization: Bearer YOUR_TOKEN`
- Verify Keycloak settings in appsettings.json

### Issue: "403 Forbidden" with valid token

**Possible causes:**
1. User doesn't have required role
2. Role claim mapping is incorrect

**Solutions:**
- Check user roles in Keycloak
- Verify role mapping in client scopes
- Check `UseRealmRoles` setting matches your setup

### Issue: "Unable to obtain configuration from..."

**Possible causes:**
1. Keycloak server is not running
2. Authority URL is incorrect
3. Realm name is incorrect

**Solutions:**
- Verify Keycloak is accessible at the configured URL
- Check the well-known endpoint manually:
  ```bash
  curl http://localhost:8080/realms/qms-realm/.well-known/openid-configuration
  ```

### Issue: SignalR connection fails with 401

**Possible causes:**
1. Token not passed in query string
2. Token is expired

**Solutions:**
- Pass token as query parameter: `?access_token=YOUR_TOKEN`
- Ensure token is valid and not expired

### Issue: CORS errors in browser

**Possible causes:**
1. Frontend origin not in CORS allowed origins
2. Keycloak Web Origins not configured

**Solutions:**
- Add frontend URL to `appsettings.json` CORS settings
- Add frontend URL to Keycloak client **Web origins**

### Enable Debug Logging

Add to `appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore.Authentication": "Trace",
      "Microsoft.AspNetCore.Authorization": "Trace"
    }
  }
}
```

### Verify JWT Token

Decode your JWT token at [jwt.io](https://jwt.io) to verify:
- `iss` (issuer) matches your Keycloak realm URL
- `aud` (audience) includes your client ID
- `exp` (expiration) is in the future
- Roles are present in the expected claim path

## Additional Resources

- [Keycloak Documentation](https://www.keycloak.org/documentation)
- [ASP.NET Core Authentication](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/)
- [JWT Best Practices](https://datatracker.ietf.org/doc/html/rfc8725)
- [SignalR Authentication](https://learn.microsoft.com/en-us/aspnet/core/signalr/authn-and-authz)

## Support

For issues specific to this integration, please open an issue on the GitHub repository.

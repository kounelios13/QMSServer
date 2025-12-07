#!/bin/bash

# Keycloak Quick Setup Script for QMSServer
# This script helps you quickly set up Keycloak for local development

set -e

KEYCLOAK_URL="http://localhost:8080"
REALM="qms-realm"
CLIENT_ID="qms-api"
ADMIN_USER="admin"
ADMIN_PASSWORD="admin"

echo "========================================"
echo "QMSServer Keycloak Quick Setup"
echo "========================================"
echo ""

# Check if docker is running
if ! docker info > /dev/null 2>&1; then
    echo "❌ Error: Docker is not running. Please start Docker and try again."
    exit 1
fi

echo "✅ Docker is running"
echo ""

# Start Keycloak with docker-compose
echo "🚀 Starting Keycloak..."
docker-compose -f docker-compose.keycloak.yml up -d keycloak

echo "⏳ Waiting for Keycloak to be ready (this may take 30-60 seconds)..."
sleep 30

# Check if Keycloak is ready
until curl -s -f -o /dev/null "$KEYCLOAK_URL"; do
    echo "   Still waiting for Keycloak..."
    sleep 5
done

echo "✅ Keycloak is ready!"
echo ""

echo "========================================"
echo "Keycloak Setup Information"
echo "========================================"
echo ""
echo "Keycloak Admin Console: $KEYCLOAK_URL/admin"
echo "Username: $ADMIN_USER"
echo "Password: $ADMIN_PASSWORD"
echo ""
echo "========================================"
echo "Next Steps"
echo "========================================"
echo ""
echo "1. Open Keycloak Admin Console: $KEYCLOAK_URL/admin"
echo "2. Log in with username '$ADMIN_USER' and password '$ADMIN_PASSWORD'"
echo "3. Follow the setup guide in KEYCLOAK_INTEGRATION.md to:"
echo "   - Create realm: $REALM"
echo "   - Create client: $CLIENT_ID"
echo "   - Create roles: qms-admin, qms-frontdesk, qms-user"
echo "   - Create users and assign roles"
echo ""
echo "4. Configure the QMSServer application:"
echo ""
echo "   Using User Secrets (recommended for development):"
echo "   cd QMS"
echo "   dotnet user-secrets set \"Keycloak:Authority\" \"$KEYCLOAK_URL\""
echo "   dotnet user-secrets set \"Keycloak:Realm\" \"$REALM\""
echo "   dotnet user-secrets set \"Keycloak:ClientId\" \"$CLIENT_ID\""
echo ""
echo "   Or using environment variables:"
echo "   export Keycloak__Authority=\"$KEYCLOAK_URL\""
echo "   export Keycloak__Realm=\"$REALM\""
echo "   export Keycloak__ClientId=\"$CLIENT_ID\""
echo ""
echo "5. Run the QMSServer application:"
echo "   cd QMS"
echo "   dotnet run"
echo ""
echo "For detailed instructions, see KEYCLOAK_INTEGRATION.md"
echo ""
echo "To stop Keycloak:"
echo "docker-compose -f docker-compose.keycloak.yml down"
echo ""

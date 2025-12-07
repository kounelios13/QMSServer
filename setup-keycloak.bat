@echo off
REM Keycloak Quick Setup Script for QMSServer (Windows)
REM This script helps you quickly set up Keycloak for local development

setlocal

set KEYCLOAK_URL=http://localhost:8080
set REALM=qms-realm
set CLIENT_ID=qms-api
set ADMIN_USER=admin
set ADMIN_PASSWORD=admin

echo ========================================
echo QMSServer Keycloak Quick Setup
echo ========================================
echo.

REM Check if docker is running
docker info >nul 2>&1
if %errorlevel% neq 0 (
    echo Error: Docker is not running. Please start Docker and try again.
    exit /b 1
)

echo ✓ Docker is running
echo.

REM Start Keycloak with docker-compose
echo Starting Keycloak...
docker-compose -f docker-compose.keycloak.yml up -d keycloak

echo Waiting for Keycloak to be ready (this may take 30-60 seconds)...
timeout /t 30 /nobreak >nul

echo Waiting for Keycloak to respond...
:wait_loop
curl -s -f -o nul %KEYCLOAK_URL%
if %errorlevel% neq 0 (
    echo    Still waiting for Keycloak...
    timeout /t 5 /nobreak >nul
    goto wait_loop
)

echo ✓ Keycloak is ready!
echo.

echo ========================================
echo Keycloak Setup Information
echo ========================================
echo.
echo Keycloak Admin Console: %KEYCLOAK_URL%/admin
echo Username: %ADMIN_USER%
echo Password: %ADMIN_PASSWORD%
echo.
echo ========================================
echo Next Steps
echo ========================================
echo.
echo 1. Open Keycloak Admin Console: %KEYCLOAK_URL%/admin
echo 2. Log in with username '%ADMIN_USER%' and password '%ADMIN_PASSWORD%'
echo 3. Follow the setup guide in KEYCLOAK_INTEGRATION.md to:
echo    - Create realm: %REALM%
echo    - Create client: %CLIENT_ID%
echo    - Create roles: qms-admin, qms-frontdesk, qms-user
echo    - Create users and assign roles
echo.
echo 4. Configure the QMSServer application:
echo.
echo    Using User Secrets (recommended for development):
echo    cd QMS
echo    dotnet user-secrets set "Keycloak:Authority" "%KEYCLOAK_URL%"
echo    dotnet user-secrets set "Keycloak:Realm" "%REALM%"
echo    dotnet user-secrets set "Keycloak:ClientId" "%CLIENT_ID%"
echo.
echo    Or using environment variables (CMD):
echo    set Keycloak__Authority=%KEYCLOAK_URL%
echo    set Keycloak__Realm=%REALM%
echo    set Keycloak__ClientId=%CLIENT_ID%
echo.
echo    Or using environment variables (PowerShell):
echo    $env:Keycloak__Authority="%KEYCLOAK_URL%"
echo    $env:Keycloak__Realm="%REALM%"
echo    $env:Keycloak__ClientId="%CLIENT_ID%"
echo.
echo 5. Run the QMSServer application:
echo    cd QMS
echo    dotnet run
echo.
echo For detailed instructions, see KEYCLOAK_INTEGRATION.md
echo.
echo To stop Keycloak:
echo docker-compose -f docker-compose.keycloak.yml down
echo.

endlocal

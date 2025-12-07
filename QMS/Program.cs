using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using MySql.EntityFrameworkCore.Extensions;
using QMS.DAL;
using QMS.Db;
using QMS.Hubs;
using QMS.Configuration;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((doc , _ , _) =>
    {
        doc.Servers = [];
        return Task.CompletedTask;
    });
});

builder.Services.AddScoped<IFrontDeskRepository, FrontDeskRepository>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddDbContext<QmsDbContext>( opts =>
{
    var devLocal = builder.Configuration.GetValue<string>("ConnectionStrings:DefaultConnection");

    if (string.IsNullOrEmpty(devLocal))
    {
        throw new InvalidOperationException("Connection string 'DevLocal' is not configured.");
    }
    _ = opts.UseMySQL(devLocal!);
});
builder.Services.AddAutoMapper(cfg => {
    cfg.AddProfile<MappingProfile>();
});
builder.Services.AddSignalR();
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<Program>();
});

// Configure Keycloak authentication
var keycloakSettings = builder.Configuration.GetSection("Keycloak").Get<KeycloakSettings>();

if (keycloakSettings != null && !string.IsNullOrEmpty(keycloakSettings.Authority) && 
    !string.IsNullOrEmpty(keycloakSettings.Realm) && !string.IsNullOrEmpty(keycloakSettings.ClientId))
{
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            var authority = keycloakSettings.GetAuthorityUrl();
            
            options.Authority = authority;
            options.MetadataAddress = $"{authority}/.well-known/openid-configuration";
            options.RequireHttpsMetadata = keycloakSettings.RequireHttpsMetadata;
            options.Audience = keycloakSettings.ClientId;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = keycloakSettings.ValidateIssuer,
                ValidIssuer = authority,
                ValidateAudience = keycloakSettings.ValidateAudience,
                ValidAudiences = keycloakSettings.ValidAudiences ?? new[] { keycloakSettings.ClientId },
                ValidateLifetime = keycloakSettings.ValidateLifetime,
                ClockSkew = TimeSpan.FromSeconds(keycloakSettings.ClockSkewSeconds),
                ValidateIssuerSigningKey = true,
                // Map the 'resource_access' claim to roles
                RoleClaimType = AuthorizationPolicies.UseRealmRoles ? "realm_access/roles" : $"resource_access/{keycloakSettings.ClientId}/roles"
            };

            // Handle SignalR connections with JWT
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;

                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                    {
                        context.Token = accessToken;
                    }

                    return Task.CompletedTask;
                }
            };
        });

    // Configure authorization policies
    builder.Services.AddAuthorization(options =>
    {
        // Admin policy - requires admin role
        options.AddPolicy(AuthorizationPolicies.AdminPolicy, policy =>
            policy.RequireRole(AuthorizationPolicies.AdminRole));

        // FrontDesk policy - requires front desk or admin role
        options.AddPolicy(AuthorizationPolicies.FrontDeskPolicy, policy =>
            policy.RequireRole(AuthorizationPolicies.FrontDeskRole, AuthorizationPolicies.AdminRole));

        // Public policy - requires any authenticated user
        options.AddPolicy(AuthorizationPolicies.PublicPolicy, policy =>
            policy.RequireAuthenticatedUser());
    });
}
else
{
    // If Keycloak is not configured, add a warning log but allow the app to run without authentication
    builder.Services.AddAuthentication();
    builder.Services.AddAuthorization();
    
    var logger = LoggerFactory.Create(config => config.AddConsole()).CreateLogger("Program");
    logger.LogWarning("Keycloak authentication is not configured. API endpoints will not be secured. " +
                     "Please configure Keycloak settings in appsettings.json or environment variables.");
}

builder.Services.AddCors(options => {
    var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
    
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
        policy.WithOrigins(allowedOrigins);
        policy.AllowCredentials();
        policy.Build();
    });
});


var app = builder.Build();
app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseWebSockets();
app.MapHub<QueueHub>("/hubs/queue");

app.Run();

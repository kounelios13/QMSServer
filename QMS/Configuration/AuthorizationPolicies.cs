namespace QMS.Configuration;

/// <summary>
/// Authorization policy names and role definitions
/// </summary>
public static class AuthorizationPolicies
{
    // Policy Names
    public const string AdminPolicy = "AdminPolicy";
    public const string FrontDeskPolicy = "FrontDeskPolicy";
    public const string PublicPolicy = "PublicPolicy";

    // Keycloak Role Names
    public const string AdminRole = "qms-admin";
    public const string FrontDeskRole = "qms-frontdesk";
    public const string UserRole = "qms-user";

    // Realm roles vs Client roles
    // Set to true if using realm roles, false for client roles
    public const bool UseRealmRoles = true;
}

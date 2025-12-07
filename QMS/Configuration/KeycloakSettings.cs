namespace QMS.Configuration;

/// <summary>
/// Configuration settings for Keycloak authentication
/// </summary>
public class KeycloakSettings
{
    /// <summary>
    /// Keycloak server URL (e.g., https://keycloak.example.com)
    /// </summary>
    public string Authority { get; set; } = string.Empty;

    /// <summary>
    /// The realm name in Keycloak
    /// </summary>
    public string Realm { get; set; } = string.Empty;

    /// <summary>
    /// Client ID registered in Keycloak
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Client secret (optional, required for confidential clients)
    /// </summary>
    public string? ClientSecret { get; set; }

    /// <summary>
    /// Whether to require HTTPS for metadata address
    /// </summary>
    public bool RequireHttpsMetadata { get; set; } = true;

    /// <summary>
    /// Whether to validate the issuer
    /// </summary>
    public bool ValidateIssuer { get; set; } = true;

    /// <summary>
    /// Whether to validate the audience
    /// </summary>
    public bool ValidateAudience { get; set; } = true;

    /// <summary>
    /// Expected audience values (defaults to ClientId if not specified)
    /// </summary>
    public string[]? ValidAudiences { get; set; }

    /// <summary>
    /// Token validation parameters - lifetime validation
    /// </summary>
    public bool ValidateLifetime { get; set; } = true;

    /// <summary>
    /// Clock skew for token expiration validation (in seconds)
    /// </summary>
    public int ClockSkewSeconds { get; set; } = 300;

    /// <summary>
    /// Gets the complete authority URL including realm
    /// </summary>
    public string GetAuthorityUrl()
    {
        if (string.IsNullOrEmpty(Authority) || string.IsNullOrEmpty(Realm))
        {
            return string.Empty;
        }
        return $"{Authority.TrimEnd('/')}/realms/{Realm}";
    }
}

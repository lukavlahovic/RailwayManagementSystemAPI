namespace RailwayManagementSystemAPI.Configuration
{
    // mirror of JwtSettings  in appsetting for safer and cleaner use
    public class JwtSettings
    {
        public string Secret { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpiryHours { get; set; }
    }
}

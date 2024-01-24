namespace Boiler.Server;

public static class Constants
{
    public static class Database
    {
        public const string DefaultDbSchema = "dbo";
        public const string PublicDbSchema = "public";
        public const string DefaultConnection_StringConfig = "Data:DefaultConnection:ConnectionString";
        public const string Data_CommandTimeout = "Data:CommandTimeout";
    }

    public static class Auth
    {
        public const string Token = "Authentication:Token:Key";
        public const string AuthTokenCookieName = "AccessToken";
    }
}
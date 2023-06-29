using Microsoft.Extensions.Configuration;

namespace PlaywrightUtils.CommonHelpers
{
    /// <summary>
    /// Environment and envrionment variable configuration
    /// </summary>
    public static class BaseConfig
    {
        private static readonly IConfiguration Config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        public static string BaseUrl => Config["BASE_URL"];
        public static string BaseApiUrl => Config["BASE_API_URL"];
        public static string Email => Config["EMAIL"];
        public static string Password => Config["PASSWORD"];
    }
}

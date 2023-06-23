using Microsoft.Extensions.Configuration;

namespace PlaywrightUI.Helpers
{
    public static class BaseConfig
    {
        private static readonly IConfiguration Config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        public static string BaseUrl => Config["BASE_URL"];
    }
}

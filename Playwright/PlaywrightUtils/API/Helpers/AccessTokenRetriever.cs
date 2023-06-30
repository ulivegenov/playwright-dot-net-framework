using Microsoft.Playwright;
using PlaywrightUtils.API.Models;
using PlaywrightUtils.CommonHelpers;
using System.Text.Json;

namespace PlaywrightUtils.API.Helpers
{
    /// <summary>
    /// Retrieves access tokens
    /// </summary>
    public static class AccessTokenRetriever
    {
        private static readonly NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// KeyValuePair access tokens collection
        /// Key - Username
        /// Value - Access token
        /// </summary>
        public static IDictionary<string, string> AccessTokens = new Dictionary<string, string>();

        public static async Task ObtainAccessTokenAsync(IPlaywright playwrightObject, string username, string password)
        {
            var requestContext = await playwrightObject.APIRequest.NewContextAsync(new APIRequestNewContextOptions
            {
                BaseURL = BaseConfig.BaseApiUrl
            });

            var response = await requestContext.PostAsync("/api/authaccount/login", new()
            {
                DataObject = new
                {
                    Email = username,
                    Password = password
                }
            });

            var responseJson = await response.JsonAsync();
            var token = responseJson.Value.Deserialize<AuthenticationResp>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }).Data.Token;

            switch (AccessTokens.ContainsKey(username))
            {
                case true:
                    AccessTokens[username] = token; 
                    break;
                case false:
                    AccessTokens.Add(username, token); 
                    break;
            }
        }
    }
}

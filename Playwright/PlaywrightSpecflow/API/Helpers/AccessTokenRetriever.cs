using Microsoft.Playwright;
using PlaywrightSpecflow.API.Models;
using PlaywrightUtils.CommonHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;

namespace PlaywrightSpecflow.API.Helpers
{
    /// <summary>
    /// Retrieves access tokens
    /// </summary>
    internal static class AccessTokenRetriever
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
            
            Assert.That(string.IsNullOrWhiteSpace(token));

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

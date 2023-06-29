using BoDi;
using Microsoft.Playwright;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlaywrightUtils.CommonHelpers;
using PlaywrightUtils.API.Actions;
using System.Reflection;
using TechTalk.SpecFlow;

namespace PlaywrightSpecflow.API.Hooks
{
    [Binding]
    internal class RestClientSupport
    {
        private static readonly NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();

        private readonly IObjectContainer _objectContainer;
        private static IPlaywright _playwrightDriver;
        RestClientManager _restClientManager;

        public RestClientSupport(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;
        }

        [BeforeTestRun]
        public static async Task BeforeTestRunSetup()
        {
            //Log appsettings values for environment
            LogAppsettingsValues();

            //Creates Playwrigth object
            _playwrightDriver = await Playwright.CreateAsync();
        }

        [BeforeScenario(Order = 0), Scope(Tag = "API")]
        public async Task SetupRestClient()
        {
            _restClientManager= new RestClientManager();
            await _restClientManager.InitializeRestClientAsync(_playwrightDriver, BaseConfig.BaseApiUrl);
            _objectContainer.RegisterInstanceAs(_restClientManager);
            _log.Debug("RestClient initialized.");
        }

        [AfterScenario(Order = 0), Scope(Tag = "API")]
        public async Task DisposeRestClient()
        {
            _log.Debug("Disposing RestClient...");
            await _restClientManager.DisposeRequestContextAsync();
            _log.Debug("RestClient disposed.");
        }

        private static void LogAppsettingsValues()
        {
            _log.Debug("Log appsetings values for execution");

            Type t = typeof(BaseConfig);
            PropertyInfo[] properties = t.GetProperties(BindingFlags.Static | BindingFlags.Public);

            dynamic jsonObject = new JObject();
            foreach (PropertyInfo pi in properties)
            {
                jsonObject[pi.Name] = (string)pi.GetValue(null);
            }

            _log.Debug(JsonConvert.SerializeObject(jsonObject, Formatting.Indented));
        }
    }
}

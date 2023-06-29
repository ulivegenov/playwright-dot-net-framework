using BoDi;
using Microsoft.Playwright;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlaywrightUtils.CommonHelpers;
using PlaywrightUtils.UI.Helpers;
using System.Reflection;
using TechTalk.SpecFlow;

namespace PlaywrightSpecflow.UI.Hooks
{
    [Binding]
    public class WebBrowserSupport
    {
        private static readonly NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();

        private readonly IObjectContainer _objectContainer;
        private ScenarioContext _scenarioContext;
        private static IPlaywright _playwrightDriver;
        private static BrowserFactory _browserFactory;

        public WebBrowserSupport(IObjectContainer objectContainer, ScenarioContext scenarioContext)
        {
            _objectContainer = objectContainer;
            _scenarioContext = scenarioContext;
            _browserFactory = new BrowserFactory(_scenarioContext);
        }

        [BeforeTestRun]
        public static async Task BeforeTestRunSetup()
        {
            //Log appsettings values for environment
            LogAppsettingsValues();

            //Creates Playwrigth object
            _playwrightDriver = await Playwright.CreateAsync();
        }

        [BeforeScenario(Order = 0), Scope(Tag = "UI")]
        public async Task InitializeBrowser()
        {
            var currentBrowser = await _browserFactory.InitializeBrowserAsync(_playwrightDriver);
            _log.Debug($"{currentBrowser.BrowserType.Name.ToUpper()} version {currentBrowser.Version.ToUpper()} is launched.");

            _objectContainer.RegisterInstanceAs(currentBrowser);
        }

        [AfterScenario(Order = 0), Scope(Tag = "UI")]
        public async Task QuitWebBrowser()
        {
            IBrowser browser = _objectContainer.Resolve<IBrowser>();

            if (browser != null)
            {
                var browserName = browser.BrowserType.Name.ToUpper();
                _log.Debug($"Closing {browserName} browser...");
                await browser.CloseAsync();
                _log.Debug($"{browserName} browser is closed.");
            }
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

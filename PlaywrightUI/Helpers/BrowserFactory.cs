using Microsoft.Playwright;
using TechTalk.SpecFlow;
using static PlaywrightUI.Helpers.Constants;

namespace PlaywrightUI.Helpers
{
    /// <summary>
    /// Creates a browser instance from supported browser type
    /// </summary>
    public class BrowserFactory
    {
        private static readonly NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();

        private ScenarioContext _scenarioContext;
        private readonly List<string> _supportedBrowsers;

        public BrowserFactory(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _supportedBrowsers = Enum.GetValues<SupportedBrowsers>()
                                     .Select(e => e.ToString())
                                     .ToList();
            InstallLatestBrowsersVersions();
        }

        /// <summary>
        /// Launching a browser instance from specific supported browser type
        /// </summary>
        public async Task<IBrowser> InitializeBrowserAsync(IPlaywright playwrightDriver)
        {
            var browser = SelectBrowserType();

            _log.Debug($"Launching {browser?.ToUpper()} browser...");

            return browser switch
            {
                CHROMIUM_BROWSER => await InitializeChromiumAsync(playwrightDriver),
                CHROME_BROWSER => await InitializeChromeAsync(playwrightDriver),
                EDGE_BROWSER => await InitializeMicrosoftEdgeAsync(playwrightDriver),
                FIREFOX_BROWSER => await InitializeFirefoxAsync(playwrightDriver),
                WEBKIT_BROWSER => await InitializeWebkitAsync(playwrightDriver),
                _ => throw new ArgumentException("Please, tag the scenario with a browser name from SupportedBrowsers in format @Browser:{browserName}!")
            };
        }

        /// <summary>
        /// Launching a Chromium browser instance.
        /// For Google Chrome, Microsoft Edge and other Chromium-based browsers, by default,
        /// Playwright uses open source Chromium builds.
        /// Since the Chromium project is ahead of the branded browsers, when the world is on Google Chrome N,
        /// Playwright already supports Chromium N+1 that will be released in Google Chrome and Microsoft Edge a few weeks later.
        /// </summary>
        public async Task<IBrowser> InitializeChromiumAsync(IPlaywright playwrightDriver)
        {
            return await playwrightDriver.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false
            });
        }

        /// <summary>
        /// Launching a Chrome browser instance.
        /// While Playwright can download and use the recent Chromium build, it can operate against the branded Google Chrome 
        /// browser available on the machine (note that Playwright doesn't install it by default).
        /// In particular, the current Playwright version will support Stable channel of this browser.
        /// </summary>
        public async Task<IBrowser> InitializeChromeAsync(IPlaywright playwrightDriver)
        {
            return await playwrightDriver.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,
                Channel = "chrome"
            });
        }

        /// <summary>
        /// Launching a Chrome browser instance.
        /// While Playwright can download and use the recent Chromium build, it can operate against the branded Microsoft Edge 
        /// browser available on the machine (note that Playwright doesn't install it by default).
        /// In particular, the current Playwright version will support Stable channel of this browser.
        /// </summary>
        private async Task<IBrowser> InitializeMicrosoftEdgeAsync(IPlaywright playwrightDriver)
        {
            return await playwrightDriver.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,
                Channel = "msedge"
            });
        }

        /// <summary>
        /// Launching a Firefox browser instance.
        /// Playwright's Firefox version matches the recent Firefox Stable build.
        /// Playwright doesn't work with the branded version of Firefox since it relies on patches.
        /// Instead you can test against the recent Firefox Stable build.
        /// </summary>
        public async Task<IBrowser> InitializeFirefoxAsync(IPlaywright playwrightDriver)
        {
            return await playwrightDriver.Firefox.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,
            });
        }

        /// <summary>
        /// Launching a Webkit browser instance.
        /// Playwright's WebKit version matches the recent WebKit trunk build,
        /// before it is used in Apple Safari and other WebKit-based browsers.
        /// This gives a lot of lead time to react on the potential browser update issues. 
        /// Playwright doesn't work with the branded version of Safari since it relies on patches.
        /// Instead you can test against the recent WebKit build.
        /// </summary>
        private async Task<IBrowser> InitializeWebkitAsync(IPlaywright playwrightDriver)
        {
            return await playwrightDriver.Webkit.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false
            });
        }

        /// <summary>
        /// Selects a supported browser, according to a scenario tags
        /// Correct scenario tag format for selecting a browser is e.g @Browser:Chromium
        /// </summary>
        private string? SelectBrowserType()
        {
            _log.Debug("Selecting supported browser...");
            _scenarioContext.TryGetValue<string>("Browser", out var browser);

            return _supportedBrowsers.Contains(browser) ? browser : null;
        }

        /// <summary>
        /// Installs latest browsers versions.
        /// If this method is not used a latest browsers versions should be installed
        /// via CLI, after every update of the Playwright NuGet packages
        /// </summary>
        private void InstallLatestBrowsersVersions()
        {
            _log.Debug($"Instaling latest browsers versions...");
            var exitCode = Program.Main(new[] { "install" });
            if (exitCode != 0)
            {
                throw new Exception($"Playwright exited with code {exitCode}");
            }

            _log.Debug($"Latest browser versions are installed.");
        }
    }
}

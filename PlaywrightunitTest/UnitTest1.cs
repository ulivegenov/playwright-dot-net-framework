using Microsoft.Playwright;

namespace PlaywrightBasic
{
    public class Tests
    {
        [Test]
        public async Task Test1Chromium()
        {
            //Playwright
            using var playwrightDriver = await Playwright.CreateAsync();

            // Browser
            await using var chromium = await playwrightDriver.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false
            });

            //Page
            var page = await chromium.NewPageAsync();
            await page.GotoAsync("https://www.google.bg/");
            await page.ClickAsync("#L2AGLb");
            await page.TypeAsync("#APjFqb", "Endava");
            await page.ClickAsync(".FPdoLc input");

            var actualText = await page.TextContentAsync(".eKjLze .DKV0Md");

            Assert.That(actualText, Is.EqualTo("Endava"));
        }

        [Test]
        public async Task Test1Firefox()
        {
            //Playwright
            using var playwrightDriver = await Playwright.CreateAsync();

            // Browser
            await using var firefox = await playwrightDriver.Firefox.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false
            });

            //Page
            var page = await firefox.NewPageAsync();
            await page.GotoAsync("https://www.google.bg/");
            await page.ClickAsync("#L2AGLb");
            await page.TypeAsync("#APjFqb", "Endava");
            await page.ClickAsync(".FPdoLc input");

            var actualText = await page.TextContentAsync(".eKjLze .DKV0Md");

            Assert.That(actualText, Is.EqualTo("Endava"));
        }

        [Test]
        public async Task Test1Webkit()
        {
            //Playwright
            using var playwrightDriver = await Playwright.CreateAsync();

            // Browser
            await using var safari = await playwrightDriver.Webkit.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false
            });

            //Page
            var page = await safari.NewPageAsync();
            await page.GotoAsync("https://www.google.bg/");
            await page.ClickAsync("#L2AGLb");
            await page.TypeAsync("#APjFqb", "Endava");
            await page.ClickAsync(".FPdoLc input");

            var actualText = await page.TextContentAsync(".eKjLze .DKV0Md");

            Assert.That(actualText, Is.EqualTo("Endava"));
        }
    }
}
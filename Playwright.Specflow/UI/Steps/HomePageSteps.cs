using Microsoft.Playwright;
using NUnit.Framework;
using PlaywrightSpecflow.UI.Pages;
using TechTalk.SpecFlow;

namespace PlaywrightUI.Steps
{
    [Binding, Scope(Tag = "UI")]
    internal class HomePageSteps
    {
        private IBrowser _browser;
        private HomePage _homePage;

        public HomePageSteps(IBrowser browser)
        {
            _browser = browser;
            _homePage = new HomePage(_browser);
        }

        [Given(@"I open Google's home page")]
        public async Task GivenIOpenGooglesHomePage()
        {
            await _homePage.NavigateToBaseUrlAsync();
        }

        [Given(@"I accept cookies")]
        public async Task GivenIAcceptCookies()
        {
            await _homePage.AcceptCookiesAsync();
        }

        [When(@"I fill ""([^""]*)"" in the seach field")]
        public async Task WhenIFillInTheSeachField(string text)
        {
            await _homePage.FillIntoSearchFieldAsync(text);
        }

        [When(@"I click on Google search button")]
        public async Task WhenIClickOnGoogleSearchButton()
        {
            await _homePage.ClickOnGoogleSearchButtonAsync();
        }

        [Then(@"Then I verify the first result is ""([^""]*)""")]
        public async Task ThenThenIVerifyTheFirstResultIs(string expectedText)
        {
            var actualText = await _homePage.GetFirstResultTextAsync();

            Assert.That(actualText, Is.EqualTo(expectedText));
        }
    }
}

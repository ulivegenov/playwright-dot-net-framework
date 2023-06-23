﻿using Microsoft.Playwright;

namespace PlaywrightUI.Pages
{
    internal class HomePage : BasePage
    {
        ILocator AcceptCookiesButton => Page.Locator("#L2AGLb");
        ILocator SearchField => Page.Locator("#APjFqb");
        ILocator GoogleSearchButton => Page.Locator("xpath=//input[@name='btnK']").Last;
        ILocator FirstReultText => Page.Locator(".eKjLze .DKV0Md");


        public HomePage(IBrowser browser) : base(browser)
        {
        }

        public async Task AcceptCookiesAsync()
        {
            await ClickOnAsync(AcceptCookiesButton);
        }

        public async Task FillIntoSearchFieldAsync(string searchString)
        {
            await FillIntoAsync(SearchField, searchString);
        }

        public async Task ClickOnGoogleSearchButtonAsync()
        {
            await ClickOnAsync(GoogleSearchButton);
        }

        public async Task<string> GetFirstResultTextAsync()
        {
            return await GetInnerTextAsync(FirstReultText);
        }
    }
}

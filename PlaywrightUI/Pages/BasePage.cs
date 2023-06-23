using Microsoft.Playwright;
using PlaywrightUI.Helpers;

namespace PlaywrightUI.Pages
{
    /// <summary>
    /// All Page classes should extend BasePage and use the predefined
    /// methods when interacting with WebElements
    /// </summary>
    internal abstract class BasePage
    {
        private IBrowser _browser;
        protected IPage _page;

        public BasePage(IBrowser browser)
        {
            _browser = browser;
            _page = _browser.NewPageAsync().GetAwaiter().GetResult();
        }

        protected IPage Page
        {
            get { return _page; }
            private  set { _page = value; }
        }

        /// <summary>
        /// Returns the Url of the current page.
        /// </summary>
        public string GetUrl()
        {
            return Page.Url;
        }

        /// <summary>
        /// Navigates to base url read from the configs.
        /// In case of multiple redirects, the navigation will resolve with the first non-redirect response.
        /// The method will throw an error if:
        ///   * there's an SSL error (e.g. in case of self-signed certificates).  
        ///   * target URL is invalid.
        ///   * the timeout is exceeded during navigation.
        ///   * the remote server does not respond or is unreachable.
        ///   * the main resource failed to load.
        ///   
        /// The method will not throw an error when any valid HTTP status code is returned by the remote server,
        /// including 404 "Not Found" and 500 "Internal Server Error". 
        /// The status code for such responses can be retrieved by calling Response.Status.
        /// </summary>
        public async Task NavigateToBaseUrlAsync()
        {
            await Page.GotoAsync(BaseConfig.BaseUrl);
        }

        /// <summary>
        /// Navigates to target url
        /// In case of multiple redirects, the navigation will resolve with the first non-redirect response.
        /// The method will throw an error if:
        ///   * there's an SSL error (e.g. in case of self-signed certificates).  
        ///   * target URL is invalid.
        ///   * the timeout is exceeded during navigation.
        ///   * the remote server does not respond or is unreachable.
        ///   * the main resource failed to load.
        ///   
        /// The method will not throw an error when any valid HTTP status code is returned by the remote server,
        /// including 404 "Not Found" and 500 "Internal Server Error". 
        /// The status code for such responses can be retrieved by calling Response.Status.
        /// /// <param name="targetUrl">The aimed url for naviagtion</param>
        /// </summary>
        public async Task NavigateToUrlAsync(string targetUrl)
        {
            await Page.GotoAsync(targetUrl);
        }

        /// <summary>
        /// This method double clicks the element by performing the following steps:
        ///    1.Wait for actionability checks on the element.
        ///    2.Scroll the element into view if needed.
        ///    3.Use Page.Mouse to click in the center of the element.
        ///    4.Wait for initiated navigations to either succeed or fail. 
        ///    Note that if the first click of the dblclick() triggers a navigation event, this method will throw.
        /// 
        /// <param name="mouseButton">enum MouseButton { Left, Right, Middle }? (optional). Defaults to left.</param>
        /// <param name="delay">Time to wait between mousedown and mouseup in milliseconds. Defaults to 0.</param>
        /// </summary>
        public async Task ClickOnAsync(ILocator locator, MouseButton mouseButton = MouseButton.Left, float delay = 0)
        {
            await locator.ClickAsync(new LocatorClickOptions
            {
                Button = mouseButton,
                Delay = delay
            });
        }

        /// <summary>
        /// This method double clicks the element by performing the following steps:
        ///    1.Wait for actionability checks on the element.
        ///    2.Scroll the element into view if needed.
        ///    3.Use Page.Mouse to double click in the center of the element.
        ///    4.Wait for initiated navigations to either succeed or fail. 
        ///    Note that if the first click of the dblclick() triggers a navigation event, this method will throw.
        /// 
        /// <param name="mouseButton">enum MouseButton { Left, Right, Middle }? (optional). Defaults to left.</param>
        /// <param name="delay">Time to wait between mousedown and mouseup in milliseconds. Defaults to 0.</param>
        /// </summary>
        public async Task DoubleClickOnAsync(ILocator locator, MouseButton mouseButton = MouseButton.Left, float delay = 0)
        {
            await locator.DblClickAsync(new LocatorDblClickOptions
            {
                Button = mouseButton,
                Delay = delay
            });
        }


        /// <summary>
        /// Ensures that checkbox or radio element is checked.
        /// If not, this method throws.
        /// </summary>
        public async Task CheckElementAsync(ILocator locator)
        {
            await locator.CheckAsync();
        }

        /// <summary>
        /// Ensures that checkbox or radio element is unchecked.
        /// If not, this method throws.
        /// </summary>
        public async Task UnCheckElementAsync(ILocator locator)
        {
            await locator.UncheckAsync();
        }

        /// <summary>
        /// Drags the source element towards the target element and drop it.
        /// <param name="sourceLocator">Locator of the element to drag.</param>
        /// <param name="targetLocator">Locator of the element to drag to.</param>
        /// </summary>
        public async Task DragAndDropAsync(ILocator sourceLocator, ILocator targetLocator)
        {
            await sourceLocator.DragToAsync(targetLocator);
        }

        /// <summary>
        /// Clear the input field.
        /// This method waits for actionability checks, focuses the element, clears it and triggers an input event after clearing.
        /// If the target element is not an <input>, <textarea> or [contenteditable] element, this method throws an error.
        /// However, if the element is inside the <label> element that has an associated control, the control will be cleared instead.
        /// </summary>
        public async Task ClearTextAsync(ILocator locator)
        {
            await locator.ClearAsync();
        }

        /// <summary>
        /// This method waits for actionability checks, focuses the element, fills it and triggers an input event after filling.
        /// Note that you can pass an empty string to clear the input field.
        /// If the target element is not an <input>, <textarea> or [contenteditable] element, this method throws an error.
        /// However, if the element is inside the <label> element that has an associated control, the control will be filled instead.
        /// To send fine-grained keyboard events, use TypeWithKeyAsync() method.
        /// <param name="text">A text to type into a focused element.</param>
        /// </summary>
        public async Task FillIntoAsync(ILocator locator, string text)
        {
            await locator.FillAsync(text);
        }

        /// <summary>
        /// Focuses the element, and then sends a keydown, keypress/input, and keyup event for each character in the text.
        /// To press a special key, like Control or ArrowDown, use PressAsync() method.
        /// Usage:
        ///     await locator.TypeAsync("Hello"); // Types instantly
        ///     await locator.TypeAsync("World", 100); // Types slower, like a user
        ///
        /// <param name="text">A text to type into a focused element.</param>
        /// <param name="delay">Time to wait between key presses in milliseconds. Defaults to 0.</param>
        /// </summary>
        public async Task TypeWithKeysAsync(ILocator locator, string text, float delay = 0)
        {
            await locator.TypeAsync(text, new()
            {
                Delay = delay,
            });
        }

        /// <summary>
        /// Focuses the element, and then uses Keyboard.DownAsync() and Keyboard.UpAsync().
        /// key can specify the intended keyboardEvent.key value or a single character to generate the text for. 
        /// A superset of the key values can be found here. Examples of the keys are:
        ///     F1 - F12, Digit0- Digit9, KeyA- KeyZ, Backquote, Minus, Equal, Backslash, Backspace, 
        ///     Tab, Delete, Escape, ArrowDown, End, Enter, Home, Insert, PageDown, PageUp, ArrowRight, ArrowUp, etc.
        /// Following modification shortcuts are also supported: Shift, Control, Alt, Meta, ShiftLeft.
        /// Holding down Shift will type the text that corresponds to the key in the upper case.
        /// If key is a single character, it is case-sensitive, so the values a and A will generate different respective texts.
        /// Shortcuts such as key: "Control+o" or key: "Control+Shift+T" are supported as well. 
        /// When specified with the modifier, modifier is pressed and being held while the subsequent key is being pressed.
        /// 
        /// <param name="keyName">Name of the key, or shortcut to press or a character to generate, such as ArrowLeft, Control+Shift+T or a.</param>
        /// <param name="delay">Time to wait between keydown and keyup in milliseconds. Defaults to 0.</param>
        /// </summary>
        public async Task PressKeysAsync(ILocator locator, string keyName, float delay = 0)
        {
            await locator.PressAsync(keyName, new()
            {
                Delay = delay
            });
        }

        /// <summary>
        /// Calls focus on the matching element.
        /// </summary>
        public async Task FocusInElementAsync(ILocator locator)
        {
            await locator.FocusAsync();
        }

        /// <summary>
        /// Returns the matching element's attribute value.
        /// <param name="attributeName">Attribute name to get the value for.</param>
        /// </summary>
        public async Task GetElementAttributeAsync(ILocator locator, string attributeName)
        {
            await locator.GetAttributeAsync(attributeName);
        }

        /// <summary>
        /// Returns the element.innerText.
        /// </summary>
        public async Task<string> GetInnerTextAsync(ILocator locator)
        {
            return await locator.InnerTextAsync();
        }

        /// <summary>
        /// Returns the node.textContent.
        /// </summary>
        public async Task<string> GetTextContentAsync(ILocator locator)
        {
            return await locator.TextContentAsync();
        }

        /// <summary>
        /// Returns input.value for the selected <input> or <textarea> or <select> element.
        /// Throws for non-input elements.
        /// However, if the element is inside the <label> element that has an associated control, returns the value of the control.
        /// </summary>
        public async Task<string> GetInputValueAsync(ILocator locator)
        {
            return await locator.InputValueAsync();
        }

        /// <summary>
        /// Returns the element.innerHTML.
        /// </summary>
        public async Task<string> GetInnerHTMLAsync(ILocator locator)
        {
            return await locator.InnerHTMLAsync();
        }

        /// <summary>
        /// Returns whether the element is disabled.
        /// </summary>
        public async Task<bool> IsElementDisabledAsync(ILocator locator)
        {
            return await locator.IsDisabledAsync();
        }

        /// <summary>
        /// Returns whether the element is enabled.
        /// </summary>
        public async Task<bool> IsElementEnabledAsync(ILocator locator)
        {
            return await locator.IsEnabledAsync();
        }

        /// <summary>
        /// Returns whether the element is checked. Throws if the element is not a checkbox or radio input.
        /// </summary>
        public async Task<bool> IsElementCheckedAsync(ILocator locator)
        {
            return await locator.IsCheckedAsync();
        }

        /// <summary>
        /// Returns whether the element is editable.
        /// Element is considered editable when it is enabled and does not have readonly property set.
        /// </summary>
        public async Task<bool> IsElementEditableAsync(ILocator locator)
        {
            return await locator.IsEditableAsync();
        }

        /// <summary>
        /// Returns whether the element is hidden, the opposite of visible.
        /// Does not wait for the element to become hidden and returns immediately.
        /// </summary>
        public async Task<bool> IsElementHiddenAsync(ILocator locator)
        {
            return await locator.IsHiddenAsync();
        }

        /// <summary>
        /// Returns whether the element is visible..
        /// Does not wait for the element to become visible and returns immediately.
        /// </summary>
        public async Task<bool> IsElementVisibleAsync(ILocator locator)
        {
            return await locator.IsVisibleAsync();
        }

        /// <summary>
        /// 1.Wait for actionability checks on the element, unless force option is set.
        /// 2.Scroll the element into view if needed.
        /// 3.Use Page.Mouse to hover over the center of the element, or the specified position.
        /// 4.Wait for initiated navigations to either succeed or fail, unless noWaitAfter option is set.
        /// If the element is detached from the DOM at any moment during the action, this method throws.
        /// When all steps combined have not finished during the specified timeout, this method throws a TimeoutError. 
        /// Passing zero timeout disables this.
        /// </summary>
        public async Task HoverOnElementAsync(ILocator locator)
        {
            await locator.HoverAsync();
        }

        /// <summary>
        /// This method waits for actionability checks, then tries to scroll element into view, unless it is completely visible.
        /// </summary>
        public async Task ScrollElementInotViewAsync(ILocator locator)
        {
            await locator.ScrollIntoViewIfNeededAsync();
        }

        /// <summary>
        /// Selects option or options in <select>.
        /// <param name="options">Options to select.</param>
        /// If the <select> has the multiple attribute, all matching options are selected,
        /// otherwise only the first option matching one of the passed options is selected. 
        /// String values are matching both values and labels.
        /// This method waits for actionability checks,
        /// waits until all specified options are present in the <select> element and selects these options.
        /// If the target element is not a <select> element, this method throws an error. 
        /// However, if the element is inside the <label> element that has an associated control, the control will be used instead.
        /// Returns the array of option values that have been successfully selected.
        /// Triggers a change and input event once all the provided options have been selected.
        /// </summary>
        public async Task<IEnumerable<string>> SelectOptionAsync(ILocator locator, params string[] options)
        {
            return await locator.SelectOptionAsync(options);
        }

        /// <summary>
        /// Returns when element specified by locator satisfies the state option.
        /// If target element already satisfies the condition, the method returns immediately. 
        /// Otherwise, waits for up to timeout milliseconds until the condition is met.
        /// <param name="elementState">enum WaitForSelectorState { Attached, Detached, Visible, Hidden }</param>
        ///     * 'attached' - wait for element to be present in DOM.
        ///     * 'detached' - wait for element to not be present in DOM.
        ///     * 'visible' - wait for element to have non-empty bounding box and no visibility:hidden. 
        ///       Note that element without any content or with display:none has an empty bounding box and is not considered visible.
        ///     * 'hidden' - wait for element to be either detached from DOM, or have an empty bounding box or visibility:hidden.
        ///       This is opposite to the 'visible' option.
        /// <param name="timeout">Maximum time in milliseconds. Defaults to 30000 (30 seconds). Pass 0 to disable timeout.</param>
        /// </summary>
        public async Task WaitForElementState(ILocator locator, WaitForSelectorState elementState, float? timeout = null)
        {
            await locator.WaitForAsync(new LocatorWaitForOptions
            {
                State = elementState,
                Timeout = timeout
            });
        }
    }
}

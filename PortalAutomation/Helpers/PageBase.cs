using Microsoft.Playwright;

namespace PortalAutomation.Helpers;

/// <summary>
/// Base class for all page objects. Provides common page interaction methods.
/// </summary>
public abstract class PageBase
{
    protected readonly IPage Page;

    protected PageBase(IPage page)
    {
        Page = page ?? throw new ArgumentNullException(nameof(page));
    }

    /// <summary>
    /// Navigate to the page.
    /// </summary>
    public abstract Task NavigateAsync();

    /// <summary>
    /// Click an element by selector.
    /// </summary>
    protected async Task ClickAsync(string selector)
    {
        await Page.ClickAsync(selector);
    }

    /// <summary>
    /// Fill a text input by selector.
    /// </summary>
    protected async Task FillAsync(string selector, string value)
    {
        await Page.FillAsync(selector, value);
    }

    /// <summary>
    /// Get text content of an element.
    /// </summary>
    protected async Task<string?> GetTextAsync(string selector)
    {
        return await Page.TextContentAsync(selector);
    }

    /// <summary>
    /// Wait for an element to be visible.
    /// </summary>
    protected async Task WaitForElementAsync(string selector, int timeout = 30000)
    {
        await Page.WaitForSelectorAsync(selector, new() { Timeout = timeout });
    }

    /// <summary>
    /// Check if an element is visible.
    /// </summary>
    protected async Task<bool> IsVisibleAsync(string selector)
    {
        return await Page.IsVisibleAsync(selector);
    }

    /// <summary>
    /// Select an option from a dropdown by value.
    /// </summary>
    protected async Task SelectOptionAsync(string selector, string value)
    {
        await Page.SelectOptionAsync(selector, value);
    }

    /// <summary>
    /// Press a key.
    /// </summary>
    protected async Task PressKeyAsync(string key)
    {
        await Page.Keyboard.PressAsync(key);
    }

    /// <summary>
    /// Wait for navigation to complete.
    /// </summary>
    protected async Task WaitForNavigationAsync()
    {
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }
}

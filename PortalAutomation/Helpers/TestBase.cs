using Microsoft.Playwright;

namespace PortalAutomation.Helpers;

/// <summary>
/// Base class for all test classes. Handles Playwright initialization and cleanup.
/// </summary>
public class TestBase : IAsyncLifetime
{
    protected IPlaywright? Playwright { get; private set; }
    protected IBrowser? Browser { get; private set; }
    protected IBrowserContext? Context { get; private set; }
    protected IPage? Page { get; private set; }

    /// <summary>
    /// Initialize Playwright, browser, context, and page before each test.
    /// </summary>
    public async Task InitializeAsync()
    {
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();

        // Launch browser (can be configured via settings)
        Browser = await Playwright.Chromium.LaunchAsync(new()
        {
            Headless = false, // Set to true for CI/CD environments
            SlowMo = 100 // Slow down operations by 100ms for visibility
        });

        // Create a new browser context (isolated session)
        Context = await Browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 1920, Height = 1080 },
            // Add other context options as needed (e.g., geolocation, permissions)
        });

        // Create a new page
        Page = await Context.NewPageAsync();
    }

    /// <summary>
    /// Cleanup Playwright resources after each test.
    /// </summary>
    public async Task DisposeAsync()
    {
        if (Page != null)
            await Page.CloseAsync();

        if (Context != null)
            await Context.CloseAsync();

        if (Browser != null)
            await Browser.CloseAsync();

        Playwright?.Dispose();
    }

    /// <summary>
    /// Navigate to a specific URL.
    /// </summary>
    protected async Task NavigateToAsync(string url)
    {
        if (Page == null)
            throw new InvalidOperationException("Page is not initialized");

        await Page.GotoAsync(url);
    }

    /// <summary>
    /// Take a screenshot (useful for debugging failed tests).
    /// </summary>
    protected async Task TakeScreenshotAsync(string fileName)
    {
        if (Page == null)
            throw new InvalidOperationException("Page is not initialized");

        await Page.ScreenshotAsync(new() { Path = fileName });
    }
}

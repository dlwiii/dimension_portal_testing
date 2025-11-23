using Microsoft.Playwright;

namespace PortalAutomation.Helpers;

/// <summary>
/// Helper class to explore pages and find selectors.
/// </summary>
public class PageExplorer
{
    public static async Task ExploreLoginPageAsync()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new() { Headless = false, SlowMo = 500 });
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync("https://uat-dimension.calance.us");

        // Wait for page to load
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Take a screenshot
        await page.ScreenshotAsync(new() { Path = "login-page.png", FullPage = true });

        // Get page HTML and print form elements
        var formElements = await page.QuerySelectorAllAsync("input, button[type='submit'], button");

        Console.WriteLine("=== Login Page Elements ===");
        foreach (var element in formElements)
        {
            var tagName = await element.EvaluateAsync<string>("el => el.tagName");
            var type = await element.EvaluateAsync<string>("el => el.type || ''");
            var name = await element.EvaluateAsync<string>("el => el.name || ''");
            var id = await element.EvaluateAsync<string>("el => el.id || ''");
            var placeholder = await element.EvaluateAsync<string>("el => el.placeholder || ''");
            var className = await element.EvaluateAsync<string>("el => el.className || ''");
            var text = await element.EvaluateAsync<string>("el => el.textContent || ''");

            Console.WriteLine($"\nTag: {tagName}");
            if (!string.IsNullOrEmpty(type)) Console.WriteLine($"  Type: {type}");
            if (!string.IsNullOrEmpty(name)) Console.WriteLine($"  Name: {name}");
            if (!string.IsNullOrEmpty(id)) Console.WriteLine($"  ID: {id}");
            if (!string.IsNullOrEmpty(placeholder)) Console.WriteLine($"  Placeholder: {placeholder}");
            if (!string.IsNullOrEmpty(className)) Console.WriteLine($"  Class: {className}");
            if (!string.IsNullOrEmpty(text.Trim())) Console.WriteLine($"  Text: {text.Trim()}");
        }

        // Keep browser open for manual inspection
        Console.WriteLine("\n\nPress Enter to close browser...");
        Console.ReadLine();

        await browser.CloseAsync();
    }
}

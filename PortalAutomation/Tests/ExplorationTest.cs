using Microsoft.Playwright;
using PortalAutomation.Helpers;

namespace PortalAutomation.Tests;

/// <summary>
/// Temporary test to explore the login page structure.
/// </summary>
public class ExplorationTest : TestBase
{
    [Fact]
    public async Task ExploreLoginPage_FindSelectors()
    {
        await Page!.GotoAsync("https://uat-dimension.calance.us");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Take a screenshot
        await Page.ScreenshotAsync(new() { Path = "login-page.png", FullPage = true });

        // Get all input elements
        var inputs = await Page.QuerySelectorAllAsync("input");
        Console.WriteLine($"\n=== Found {inputs.Count} input elements ===\n");

        foreach (var input in inputs)
        {
            var id = await input.GetAttributeAsync("id") ?? "";
            var name = await input.GetAttributeAsync("name") ?? "";
            var type = await input.GetAttributeAsync("type") ?? "";
            var placeholder = await input.GetAttributeAsync("placeholder") ?? "";

            Console.WriteLine($"Input type='{type}'");
            if (!string.IsNullOrEmpty(id)) Console.WriteLine($"  id: '{id}'");
            if (!string.IsNullOrEmpty(name)) Console.WriteLine($"  name: '{name}'");
            if (!string.IsNullOrEmpty(placeholder)) Console.WriteLine($"  placeholder: '{placeholder}'");
            Console.WriteLine();
        }

        // Get all buttons
        var buttons = await Page.QuerySelectorAllAsync("button");
        Console.WriteLine($"\n=== Found {buttons.Count} button elements ===\n");

        foreach (var button in buttons)
        {
            var id = await button.GetAttributeAsync("id") ?? "";
            var type = await button.GetAttributeAsync("type") ?? "";
            var text = (await button.TextContentAsync() ?? "").Trim();

            Console.WriteLine($"Button text='{text}'");
            if (!string.IsNullOrEmpty(id)) Console.WriteLine($"  id: '{id}'");
            if (!string.IsNullOrEmpty(type)) Console.WriteLine($"  type: '{type}'");
            Console.WriteLine();
        }

        // Output the page title
        var title = await Page.TitleAsync();
        Console.WriteLine($"\nPage Title: {title}");

        // Keep the browser open for a moment for manual inspection
        await Task.Delay(3000);
    }
}

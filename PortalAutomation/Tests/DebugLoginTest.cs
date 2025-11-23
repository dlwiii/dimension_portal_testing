using Microsoft.Playwright;
using PortalAutomation.Helpers;
using PortalAutomation.Pages;

namespace PortalAutomation.Tests;

/// <summary>
/// Debug test to explore login behavior.
/// </summary>
public class DebugLoginTest : TestBase
{
    [Fact]
    public async Task Debug_LoginProcess()
    {
        var username = Environment.GetEnvironmentVariable("PORTAL_USERNAME") ?? "dlwiii";
        var password = Environment.GetEnvironmentVariable("PORTAL_PASSWORD") ?? "daniel314Pi$";

        Console.WriteLine($"Testing login for user: {username}");

        var loginPage = new LoginPage(Page!);
        await loginPage.NavigateAsync();

        Console.WriteLine($"Initial URL: {Page!.Url}");
        await Page.ScreenshotAsync(new() { Path = "01-before-login.png" });

        // Enter credentials
        await loginPage.EnterUsernameAsync(username);
        Console.WriteLine("Username entered");

        await loginPage.EnterPasswordAsync(password);
        Console.WriteLine("Password entered");

        await Page.ScreenshotAsync(new() { Path = "02-credentials-entered.png" });

        // Click login
        await loginPage.ClickLoginButtonAsync();
        Console.WriteLine("Login button clicked");

        // Wait a bit
        await Task.Delay(3000);

        Console.WriteLine($"URL after login: {Page.Url}");
        await Page.ScreenshotAsync(new() { Path = "03-after-login.png" });

        // Wait for navigation if any
        try
        {
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = 10000 });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Wait for navigation error: {ex.Message}");
        }

        Console.WriteLine($"Final URL: {Page.Url}");
        Console.WriteLine($"Page title: {await Page.TitleAsync()}");

        await Page.ScreenshotAsync(new() { Path = "04-final-state.png" });

        // Check for any error messages
        var errorElements = await Page.QuerySelectorAllAsync(".error, .alert, [role='alert']");
        Console.WriteLine($"Found {errorElements.Count} potential error elements");

        foreach (var error in errorElements)
        {
            var text = await error.TextContentAsync();
            if (!string.IsNullOrWhiteSpace(text))
            {
                Console.WriteLine($"Error message: {text}");
            }
        }

        // Keep browser open for inspection
        await Task.Delay(5000);
    }
}

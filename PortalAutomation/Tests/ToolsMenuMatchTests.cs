using FluentAssertions;
using PortalAutomation.Helpers;
using PortalAutomation.Pages;

namespace PortalAutomation.Tests;

/// <summary>
/// Tests for Tools Menu → Match* items.
/// Uses dynamic discovery to test all Match menu items with a single loop approach.
/// </summary>
public class ToolsMenuMatchTests : TestBase
{
    private static string GetUsername() =>
        Environment.GetEnvironmentVariable("PORTAL_USERNAME")
        ?? throw new InvalidOperationException("PORTAL_USERNAME environment variable not set");

    private static string GetPassword() =>
        Environment.GetEnvironmentVariable("PORTAL_PASSWORD")
        ?? throw new InvalidOperationException("PORTAL_PASSWORD environment variable not set");

    [Fact]
    public async Task AllMatchMenuItems_ShouldLoadSuccessfully()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║  Tools Menu Match Items - Dynamic Discovery Test            ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════╝\n");

        // ═══════════════════════════════════════════════════════════════
        // SETUP: Login and Select Company
        // ═══════════════════════════════════════════════════════════════
        Console.WriteLine("📋 SETUP PHASE");
        Console.WriteLine("─────────────────────────────────────────────────────────────");

        var username = GetUsername();
        var password = GetPassword();

        Console.WriteLine("1. Logging in...");
        var loginPage = new LoginPage(Page!);
        await loginPage.NavigateAsync();
        await loginPage.LoginAsync(username, password);
        await loginPage.WaitForLoginCompleteAsync();
        Console.WriteLine($"   ✓ Logged in. URL: {Page!.Url}");

        // Select MOCK company if on company selection page
        if (Page.Url.Contains("login_company"))
        {
            Console.WriteLine("2. Selecting MOCK company...");
            var companyPage = new CompanySelectionPage(Page);
            await companyPage.SelectCompanyAsync("MOCK");
            Console.WriteLine($"   ✓ Company selected. URL: {Page.Url}");
        }

        // Verify we're on the /run page
        Page.Url.Should().Contain("/run", "because we should be on the main application page");
        Console.WriteLine("   ✓ On main application page (/run)\n");

        // ═══════════════════════════════════════════════════════════════
        // DISCOVERY: Find All Match Menu Items
        // ═══════════════════════════════════════════════════════════════
        Console.WriteLine("🔍 DISCOVERY PHASE");
        Console.WriteLine("─────────────────────────────────────────────────────────────");

        var toolsMenu = new ToolsMenuPage(Page);
        await toolsMenu.OpenToolsMenuAsync();
        Console.WriteLine("   ✓ Opened Tools menu");

        var matchItems = await toolsMenu.DiscoverMatchMenuItemsAsync();
        Console.WriteLine($"   ✓ Found {matchItems.Count} Match menu items:\n");

        foreach (var (text, href) in matchItems)
        {
            Console.WriteLine($"      • {text}");
            Console.WriteLine($"        └─ {href}");
        }

        matchItems.Should().NotBeEmpty("because there should be at least one Match menu item");
        Console.WriteLine($"\n   → Will test {matchItems.Count} menu items\n");

        // ═══════════════════════════════════════════════════════════════
        // TESTING LOOP: Test Each Match Item
        // ═══════════════════════════════════════════════════════════════
        Console.WriteLine("🧪 TESTING PHASE");
        Console.WriteLine("─────────────────────────────────────────────────────────────\n");

        var matchScreen = new MatchScreenPage(Page);
        int successCount = 0;

        for (int i = 0; i < matchItems.Count; i++)
        {
            var (itemText, itemHref) = matchItems[i];

            Console.WriteLine($"[{i + 1}/{matchItems.Count}] Testing: {itemText}");
            Console.WriteLine($"────────────────────────────────────────────────────────");

            try
            {
                // Click the menu item
                Console.WriteLine($"  🖱️  Clicking menu item...");
                await toolsMenu.ClickMenuItemByHrefAsync(itemHref);
                Console.WriteLine($"  ✓ Navigated to: {Page.Url}");

                // Wait for page to load (handles loading screen, optional modal, and reactTable)
                Console.WriteLine($"  ⏳ Waiting for page to load...");
                var loadSuccess = await matchScreen.WaitForPageLoadAsync(timeout: 60000);

                if (!loadSuccess)
                {
                    // Take screenshot on failure
                    var screenshotPath = $"match-item-failure-{i + 1}.png";
                    await Page.ScreenshotAsync(new() { Path = screenshotPath, FullPage = true });
                    Console.WriteLine($"  📸 Screenshot saved: {screenshotPath}");

                    // Fail fast
                    Assert.Fail($"❌ FAILED: {itemText} - reactTable did not load within timeout");
                }

                Console.WriteLine($"  ✅ SUCCESS: {itemText} loaded correctly\n");
                successCount++;

                // Navigate back to /run page for next test
                if (i < matchItems.Count - 1) // Don't navigate back after last item
                {
                    Console.WriteLine($"  ↩️  Navigating back to /run page...");
                    await toolsMenu.NavigateToRunPageAsync();
                    Console.WriteLine($"  ✓ Back on /run page\n");
                }
            }
            catch (Exception ex)
            {
                // Take screenshot on any exception
                var screenshotPath = $"match-item-error-{i + 1}.png";
                await Page.ScreenshotAsync(new() { Path = screenshotPath, FullPage = true });
                Console.WriteLine($"  📸 Screenshot saved: {screenshotPath}");

                // Fail fast - rethrow the exception
                throw new Exception($"Error testing '{itemText}': {ex.Message}", ex);
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // SUMMARY
        // ═══════════════════════════════════════════════════════════════
        Console.WriteLine("\n╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine($"║  ✅ ALL TESTS PASSED - {successCount}/{matchItems.Count} items tested successfully    ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════╝\n");
    }
}

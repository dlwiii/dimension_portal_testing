using Microsoft.Playwright;
using PortalAutomation.Helpers;
using PortalAutomation.Pages;

namespace PortalAutomation.Tests;

/// <summary>
/// Exploration test to discover Tools Menu items that start with "Match".
/// </summary>
public class ToolsMenuExplorationTest : TestBase
{
    private static string GetUsername() =>
        Environment.GetEnvironmentVariable("PORTAL_USERNAME")
        ?? throw new InvalidOperationException("PORTAL_USERNAME environment variable not set");

    private static string GetPassword() =>
        Environment.GetEnvironmentVariable("PORTAL_PASSWORD")
        ?? throw new InvalidOperationException("PORTAL_PASSWORD environment variable not set");

    [Fact]
    public async Task ExploreToolsMenu_FindMatchItems()
    {
        var username = GetUsername();
        var password = GetPassword();

        Console.WriteLine("=== Tools Menu Exploration ===\n");

        // Login first
        var loginPage = new LoginPage(Page!);
        await loginPage.NavigateAsync();
        await loginPage.LoginAsync(username, password);
        await loginPage.WaitForLoginCompleteAsync();

        Console.WriteLine($"Logged in. Current URL: {Page!.Url}");

        // Take screenshot of the page after login
        await Page.ScreenshotAsync(new() { Path = "tools-menu-01-after-login.png", FullPage = true });

        // If we're on the company selection page, enter "MOCK" company
        if (Page.Url.Contains("login_company"))
        {
            Console.WriteLine("\n=== Selecting Company: MOCK ===");

            // Look for companies input field - try different selectors
            var companiesInputSelectors = new[]
            {
                "input[name='companies']",
                "input[placeholder*='compan']",
                "#companies",
                "[aria-label*='compan']",
                "input[type='text']",
                "input[type='search']"
            };

            IElementHandle? companiesInput = null;
            string? usedInputSelector = null;

            foreach (var selector in companiesInputSelectors)
            {
                companiesInput = await Page.QuerySelectorAsync(selector);
                if (companiesInput != null)
                {
                    var isVisible = await companiesInput.IsVisibleAsync();
                    Console.WriteLine($"Found companies input with selector: {selector} (visible: {isVisible})");
                    usedInputSelector = selector;
                    break;
                }
            }

            if (companiesInput != null)
            {
                // Enter "MOCK" and press Enter
                await companiesInput.FillAsync("MOCK");
                Console.WriteLine("Entered 'MOCK' in companies field");
                await companiesInput.PressAsync("Enter");
                Console.WriteLine("Pressed Enter");

                // Wait for navigation to /run page
                await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                await Task.Delay(2000); // Wait for page to fully load
                Console.WriteLine($"After company selection. Current URL: {Page.Url}");
                await Page.ScreenshotAsync(new() { Path = "tools-menu-02-after-company-select.png", FullPage = true });
            }
            else
            {
                Console.WriteLine("WARNING: Could not find companies input field");
                // Take a screenshot to help debug
                await Page.ScreenshotAsync(new() { Path = "tools-menu-company-selection-page.png", FullPage = true });
            }
        }

        // Look for navigation elements - try common patterns for top navigation
        var navSelectors = new[]
        {
            "nav",
            "[role='navigation']",
            ".navbar",
            ".nav",
            ".navigation",
            "header nav",
            ".top-nav",
            ".main-nav"
        };

        IElementHandle? toolsMenuElement = null;
        string? usedSelector = null;

        foreach (var selector in navSelectors)
        {
            var elements = await Page.QuerySelectorAllAsync(selector);
            if (elements.Count > 0)
            {
                Console.WriteLine($"\nFound {elements.Count} element(s) with selector: {selector}");

                // Check each navigation element for "Tools" text
                foreach (var element in elements)
                {
                    var text = await element.TextContentAsync() ?? "";
                    if (text.Contains("Tools", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine($"  Found navigation with 'Tools': {selector}");
                        toolsMenuElement = element;
                        usedSelector = selector;
                        break;
                    }
                }

                if (toolsMenuElement != null) break;
            }
        }

        // Try to find Tools menu link/button
        var toolsLinkSelectors = new[]
        {
            "text=/^Tools$/i",
            "a:has-text('Tools')",
            "button:has-text('Tools')",
            "[aria-label*='Tools']",
            ".menu-item:has-text('Tools')"
        };

        IElementHandle? toolsLink = null;
        string? toolsLinkSelector = null;

        foreach (var selector in toolsLinkSelectors)
        {
            var element = await Page.QuerySelectorAsync(selector);
            if (element != null)
            {
                var isVisible = await element.IsVisibleAsync();
                Console.WriteLine($"\nFound Tools link: {selector} (visible: {isVisible})");
                toolsLink = element;
                toolsLinkSelector = selector;
                break;
            }
        }

        if (toolsLink != null)
        {
            Console.WriteLine($"\n=== Clicking Tools Menu ===");
            await toolsLink.ClickAsync();
            await Task.Delay(1000); // Wait for menu to open

            await Page.ScreenshotAsync(new() { Path = "tools-menu-02-menu-open.png", FullPage = true });
        }

        // Find all menu items
        var menuItemSelectors = new[]
        {
            ".dropdown-menu a",
            ".menu-item",
            "[role='menuitem']",
            ".nav-item a",
            "ul li a"
        };

        Console.WriteLine($"\n=== Searching for 'Match' menu items ===\n");
        var matchItems = new List<(string text, string? href, string selector)>();

        foreach (var selector in menuItemSelectors)
        {
            var items = await Page.QuerySelectorAllAsync(selector);
            Console.WriteLine($"Found {items.Count} items with selector: {selector}");

            foreach (var item in items)
            {
                var text = (await item.TextContentAsync() ?? "").Trim();
                if (text.StartsWith("Match", StringComparison.OrdinalIgnoreCase))
                {
                    var href = await item.GetAttributeAsync("href");
                    var isVisible = await item.IsVisibleAsync();

                    Console.WriteLine($"\n✓ Found Match Item:");
                    Console.WriteLine($"  Text: {text}");
                    Console.WriteLine($"  Href: {href ?? "N/A"}");
                    Console.WriteLine($"  Selector: {selector}");
                    Console.WriteLine($"  Visible: {isVisible}");

                    matchItems.Add((text, href, selector));
                }
            }
        }

        Console.WriteLine($"\n=== Summary ===");
        Console.WriteLine($"Total 'Match' items found: {matchItems.Count}");
        foreach (var (text, href, selector) in matchItems)
        {
            Console.WriteLine($"  - {text} → {href ?? "Unknown URL"}");
        }

        // Keep browser open for manual inspection
        Console.WriteLine("\n=== Keeping browser open for 10 seconds for manual inspection ===");
        await Task.Delay(10000);
    }
}

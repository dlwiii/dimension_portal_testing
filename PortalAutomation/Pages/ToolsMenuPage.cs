using Microsoft.Playwright;
using PortalAutomation.Helpers;

namespace PortalAutomation.Pages;

/// <summary>
/// Page object for the Tools Menu navigation.
/// Accessible from the main application page (/run) after company selection.
/// </summary>
public class ToolsMenuPage : PageBase
{
    // Tools Menu selectors
    private const string ToolsMenuLinkSelector = "text=/^Tools$/i";
    private const string DropdownMenuSelector = ".dropdown-menu";

    // Match menu item selectors
    private const string MatchProjectsSelector = ".dropdown-menu a[href='/tools/match_tools/get_project_match_data']";
    private const string MatchCommitmentsSelector = ".dropdown-menu a[href='/tools/match_tools/get_commitment_match_data']";

    public ToolsMenuPage(IPage page) : base(page)
    {
    }

    /// <summary>
    /// Navigate method not applicable for Tools Menu (it's a dropdown, not a page).
    /// Use this from the /run page.
    /// </summary>
    public override async Task NavigateAsync()
    {
        // Tools menu is accessed from the main /run page, not via direct navigation
        await Task.CompletedTask;
    }

    /// <summary>
    /// Click the Tools menu to open the dropdown.
    /// </summary>
    public async Task OpenToolsMenuAsync()
    {
        await ClickAsync(ToolsMenuLinkSelector);
        // Wait a moment for dropdown to appear
        await Task.Delay(500);
    }

    /// <summary>
    /// Check if the Tools menu dropdown is visible.
    /// </summary>
    public async Task<bool> IsToolsMenuOpenAsync()
    {
        return await IsVisibleAsync(DropdownMenuSelector);
    }

    /// <summary>
    /// Click the "Match Projects" menu item.
    /// Note: Tools menu must be open first.
    /// </summary>
    public async Task ClickMatchProjectsAsync()
    {
        await ClickAsync(MatchProjectsSelector);
        await WaitForNavigationAsync();
    }

    /// <summary>
    /// Click the "Match Commitments" menu item.
    /// Note: Tools menu must be open first.
    /// </summary>
    public async Task ClickMatchCommitmentsAsync()
    {
        await ClickAsync(MatchCommitmentsSelector);
        await WaitForNavigationAsync();
    }

    /// <summary>
    /// Check if "Match Projects" menu item is visible.
    /// </summary>
    public async Task<bool> IsMatchProjectsVisibleAsync()
    {
        return await IsVisibleAsync(MatchProjectsSelector);
    }

    /// <summary>
    /// Check if "Match Commitments" menu item is visible.
    /// </summary>
    public async Task<bool> IsMatchCommitmentsVisibleAsync()
    {
        return await IsVisibleAsync(MatchCommitmentsSelector);
    }

    /// <summary>
    /// Get all visible "Match" menu items with their text and href.
    /// Returns list of tuples (text, href) for dynamic testing.
    /// </summary>
    public async Task<List<(string text, string href)>> DiscoverMatchMenuItemsAsync()
    {
        var items = new List<(string text, string href)>();
        var elements = await Page.QuerySelectorAllAsync($"{DropdownMenuSelector} a");

        foreach (var element in elements)
        {
            var text = await element.TextContentAsync();
            if (text != null && text.Trim().StartsWith("Match", StringComparison.OrdinalIgnoreCase))
            {
                var href = await element.GetAttributeAsync("href");
                if (href != null)
                {
                    items.Add((text.Trim(), href));
                }
            }
        }

        return items;
    }

    /// <summary>
    /// Click a menu item by its href attribute.
    /// </summary>
    public async Task ClickMenuItemByHrefAsync(string href)
    {
        var selector = $"{DropdownMenuSelector} a[href='{href}']";
        await ClickAsync(selector);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    /// <summary>
    /// Navigate back to the /run page.
    /// Uses direct navigation for reliability (multiple dropdowns on page make menu navigation unreliable).
    /// </summary>
    public async Task NavigateToRunPageAsync()
    {
        // Direct navigation is more reliable than trying to click through menus
        // when there are multiple dropdown elements on the page
        await Page.GotoAsync("https://uat-dimension.calance.us/run");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }
}

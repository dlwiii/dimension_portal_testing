using Microsoft.Playwright;
using PortalAutomation.Helpers;

namespace PortalAutomation.Pages;

/// <summary>
/// Page object for Match screens (Match Projects, Match Commitments, etc.).
/// Handles the complex page load flow including optional "Select Project" modal.
/// </summary>
public class MatchScreenPage : PageBase
{
    private const string LoadingTextSelector = "text=/loading large data sets/i";
    private const string SelectProjectModalSelector = "text=/Select Project/i";
    private const string ReactTableSelector = "#reactTable";
    private const int DefaultTimeout = 60000; // 60 seconds

    public MatchScreenPage(IPage page) : base(page)
    {
    }

    /// <summary>
    /// Not applicable for Match screens (accessed via Tools menu).
    /// </summary>
    public override async Task NavigateAsync()
    {
        await Task.CompletedTask;
    }

    /// <summary>
    /// Wait for the Match screen to fully load and verify success.
    ///
    /// Flow:
    /// 1. Optional: Wait for "loading large data sets" to appear/disappear
    /// 2. Check for "Select Project" modal
    /// 3. If modal present: select first item and submit
    /// 4. Wait for #reactTable to appear
    /// 5. Return true if reactTable appears, false otherwise
    /// </summary>
    public async Task<bool> WaitForPageLoadAsync(int timeout = DefaultTimeout)
    {
        try
        {
            // Step 1: Optional - wait briefly for loading indicator (may not always appear)
            try
            {
                await Page.WaitForSelectorAsync(LoadingTextSelector, new() { Timeout = 5000 });
                Console.WriteLine("  ✓ Saw 'loading large data sets' indicator");
            }
            catch
            {
                // Loading text might not appear or might disappear too quickly
                Console.WriteLine("  ℹ Loading indicator not detected (may have loaded quickly)");
            }

            // Step 2: Check for "Select Project" modal
            var hasModal = await HandleSelectProjectModalAsync();
            if (hasModal)
            {
                Console.WriteLine("  ✓ Handled 'Select Project' modal");
            }

            // Step 3: Wait for the final success indicator - reactTable
            Console.WriteLine($"  ⏳ Waiting for #reactTable (timeout: {timeout}ms)...");
            await Page.WaitForSelectorAsync(ReactTableSelector, new() { Timeout = timeout });

            // Verify it's visible
            var isVisible = await Page.IsVisibleAsync(ReactTableSelector);
            if (isVisible)
            {
                Console.WriteLine("  ✅ #reactTable loaded successfully");
                return true;
            }
            else
            {
                Console.WriteLine("  ❌ #reactTable found but not visible");
                return false;
            }
        }
        catch (TimeoutException)
        {
            Console.WriteLine($"  ❌ Timeout waiting for #reactTable after {timeout}ms");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  ❌ Error during page load: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Check for and handle the optional "Select Project" modal.
    /// Returns true if modal was present and handled, false otherwise.
    /// </summary>
    private async Task<bool> HandleSelectProjectModalAsync()
    {
        try
        {
            // Check if the "Select Project" modal is present
            var modal = await Page.WaitForSelectorAsync(SelectProjectModalSelector, new() { Timeout = 3000 });
            if (modal == null)
            {
                return false;
            }

            Console.WriteLine("  📋 'Select Project' modal detected");

            // Wait for the modal's table to load (it should have its own table/list)
            await Task.Delay(1000); // Give it a moment to populate

            // Find the first selectable item in the modal
            // Try common patterns for selectable table rows
            var selectableItemSelectors = new[]
            {
                "table tbody tr:first-child",
                ".modal table tbody tr:first-child",
                "[role='dialog'] table tbody tr:first-child",
                "table tr:first-child td:first-child"
            };

            IElementHandle? firstItem = null;
            foreach (var selector in selectableItemSelectors)
            {
                firstItem = await Page.QuerySelectorAsync(selector);
                if (firstItem != null && await firstItem.IsVisibleAsync())
                {
                    Console.WriteLine($"  ✓ Found first item with selector: {selector}");
                    break;
                }
            }

            if (firstItem != null)
            {
                await firstItem.ClickAsync();
                Console.WriteLine("  ✓ Selected first item in modal");
            }

            // Find and click the submit button
            var submitSelectors = new[]
            {
                "button[type='submit']",
                "button:has-text('Submit')",
                "button:has-text('OK')",
                "button:has-text('Select')",
                ".modal button[type='submit']"
            };

            foreach (var selector in submitSelectors)
            {
                var submitButton = await Page.QuerySelectorAsync(selector);
                if (submitButton != null && await submitButton.IsVisibleAsync())
                {
                    await submitButton.ClickAsync();
                    Console.WriteLine($"  ✓ Clicked submit button: {selector}");
                    await Task.Delay(500); // Wait for modal to close
                    return true;
                }
            }

            Console.WriteLine("  ⚠ Modal detected but couldn't find submit button");
            return true;
        }
        catch (TimeoutException)
        {
            // Modal not present - this is normal
            return false;
        }
    }

    /// <summary>
    /// Check if the reactTable is currently visible.
    /// </summary>
    public async Task<bool> IsReactTableVisibleAsync()
    {
        return await IsVisibleAsync(ReactTableSelector);
    }
}

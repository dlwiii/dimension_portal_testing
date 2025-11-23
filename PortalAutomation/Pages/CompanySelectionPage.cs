using Microsoft.Playwright;
using PortalAutomation.Helpers;

namespace PortalAutomation.Pages;

/// <summary>
/// Page object for the Company Selection page (/login_company).
/// Appears after successful login if "use_company_id" is not checked.
/// </summary>
public class CompanySelectionPage : PageBase
{
    private const string PageUrl = "https://uat-dimension.calance.us/login_company";
    private const string CompaniesInputSelector = "input[type='text']";

    public CompanySelectionPage(IPage page) : base(page)
    {
    }

    /// <summary>
    /// Navigate to company selection page.
    /// Note: Typically reached via login, not direct navigation.
    /// </summary>
    public override async Task NavigateAsync()
    {
        await Page.GotoAsync(PageUrl);
    }

    /// <summary>
    /// Enter a company name and press Enter to select it.
    /// </summary>
    /// <param name="companyName">Name of the company to select (e.g., "MOCK")</param>
    public async Task SelectCompanyAsync(string companyName)
    {
        var companiesInput = await Page.QuerySelectorAsync(CompaniesInputSelector);
        if (companiesInput == null)
        {
            throw new InvalidOperationException("Companies input field not found");
        }

        await companiesInput.FillAsync(companyName);
        await companiesInput.PressAsync("Enter");

        // Wait for navigation to complete
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    /// <summary>
    /// Check if we're on the company selection page.
    /// </summary>
    public async Task<bool> IsOnCompanySelectionPageAsync()
    {
        return Page.Url.Contains("login_company");
    }
}

using Microsoft.Playwright;
using PortalAutomation.Helpers;

namespace PortalAutomation.Pages;

/// <summary>
/// Page object for the Dimension portal login page.
/// </summary>
public class LoginPage : PageBase
{
    // Page URL
    private const string PageUrl = "https://uat-dimension.calance.us";

    // Locators - Dimension portal login page
    private const string UsernameInputSelector = "#user_id";
    private const string PasswordInputSelector = "#password";
    private const string LoginButtonSelector = "button[type='submit']";
    private const string ErrorMessageSelector = ".error-message, .alert-danger, [role='alert']";
    private const string DashboardSelector = "text=/dashboard|home|welcome/i"; // Adjust based on actual post-login page

    public LoginPage(IPage page) : base(page)
    {
    }

    /// <summary>
    /// Navigate to the login page.
    /// </summary>
    public override async Task NavigateAsync()
    {
        await Page.GotoAsync(PageUrl);
    }

    /// <summary>
    /// Enter username in the username field.
    /// </summary>
    public async Task EnterUsernameAsync(string username)
    {
        await FillAsync(UsernameInputSelector, username);
    }

    /// <summary>
    /// Enter password in the password field.
    /// </summary>
    public async Task EnterPasswordAsync(string password)
    {
        await FillAsync(PasswordInputSelector, password);
    }

    /// <summary>
    /// Click the login button.
    /// </summary>
    public async Task ClickLoginButtonAsync()
    {
        await ClickAsync(LoginButtonSelector);
    }

    /// <summary>
    /// Perform complete login with username and password.
    /// </summary>
    public async Task LoginAsync(string username, string password)
    {
        await EnterUsernameAsync(username);
        await EnterPasswordAsync(password);
        await ClickLoginButtonAsync();
    }

    /// <summary>
    /// Get the error message text (if displayed).
    /// </summary>
    public async Task<string?> GetErrorMessageAsync()
    {
        return await GetTextAsync(ErrorMessageSelector);
    }

    /// <summary>
    /// Check if error message is displayed.
    /// </summary>
    public async Task<bool> IsErrorMessageDisplayedAsync()
    {
        return await IsVisibleAsync(ErrorMessageSelector);
    }

    /// <summary>
    /// Check if logged in successfully by verifying URL changed from initial login page.
    /// </summary>
    public async Task<bool> IsLoggedInAsync()
    {
        // Wait a moment for navigation to complete
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Check if URL changed from initial login page
        // After successful login, it goes to /login_company (company selection page)
        var currentUrl = Page.Url;
        return currentUrl != PageUrl && !currentUrl.EndsWith("/login");
    }

    /// <summary>
    /// Wait for login to complete and verify success.
    /// </summary>
    public async Task WaitForLoginCompleteAsync(int timeout = 10000)
    {
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = timeout });
    }
}

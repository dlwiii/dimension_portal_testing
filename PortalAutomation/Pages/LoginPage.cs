using Microsoft.Playwright;
using PortalAutomation.Helpers;

namespace PortalAutomation.Pages;

/// <summary>
/// Page object for the Login page.
/// This is an example - update selectors to match your actual application.
/// </summary>
public class LoginPage : PageBase
{
    // Page URL
    private const string PageUrl = "https://your-portal-url.com/login"; // Update with actual URL

    // Locators (update these to match your application's HTML structure)
    private const string UsernameInputSelector = "input[name='username']";
    private const string PasswordInputSelector = "input[name='password']";
    private const string LoginButtonSelector = "button[type='submit']";
    private const string ErrorMessageSelector = ".error-message";
    private const string WelcomeMessageSelector = ".welcome-message";

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
    /// Check if welcome message is displayed (indicates successful login).
    /// </summary>
    public async Task<bool> IsWelcomeMessageDisplayedAsync()
    {
        return await IsVisibleAsync(WelcomeMessageSelector);
    }
}

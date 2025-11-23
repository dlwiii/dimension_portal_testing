using FluentAssertions;
using PortalAutomation.Helpers;
using PortalAutomation.Pages;

namespace PortalAutomation.Tests;

/// <summary>
/// Test class for Dimension portal login functionality.
/// Credentials are read from environment variables: PORTAL_USERNAME and PORTAL_PASSWORD
/// </summary>
public class LoginTests : TestBase
{
    private static string GetUsername() =>
        Environment.GetEnvironmentVariable("PORTAL_USERNAME")
        ?? throw new InvalidOperationException("PORTAL_USERNAME environment variable not set");

    private static string GetPassword() =>
        Environment.GetEnvironmentVariable("PORTAL_PASSWORD")
        ?? throw new InvalidOperationException("PORTAL_PASSWORD environment variable not set");

    [Fact]
    public async Task ValidLogin_ShouldSucceed()
    {
        // Arrange
        var loginPage = new LoginPage(Page!);
        await loginPage.NavigateAsync();

        var username = GetUsername();
        var password = GetPassword();

        // Act
        await loginPage.LoginAsync(username, password);
        await loginPage.WaitForLoginCompleteAsync();

        // Assert
        var isLoggedIn = await loginPage.IsLoggedInAsync();
        isLoggedIn.Should().BeTrue("because valid credentials should result in successful login");

        var currentUrl = Page!.Url;
        Console.WriteLine($"Logged in successfully. Current URL: {currentUrl}");
    }

    [Fact]
    public async Task InvalidLogin_ShouldShowErrorMessage()
    {
        // Arrange
        var loginPage = new LoginPage(Page!);
        await loginPage.NavigateAsync();

        // Act
        await loginPage.LoginAsync("invaliduser", "wrongpassword");
        await Task.Delay(2000); // Wait for error message

        // Assert
        var currentUrl = Page!.Url;
        currentUrl.Should().Contain("uat-dimension.calance.us", "because failed login should stay on login page");

        // Check if still on login page (login failed)
        var isLoggedIn = await loginPage.IsLoggedInAsync();
        isLoggedIn.Should().BeFalse("because invalid credentials should not result in successful login");
    }

    [Fact]
    public async Task EmptyCredentials_ShouldNotLogin()
    {
        // Arrange
        var loginPage = new LoginPage(Page!);
        await loginPage.NavigateAsync();

        // Act
        await loginPage.ClickLoginButtonAsync();
        await Task.Delay(1000); // Wait briefly

        // Assert
        var isLoggedIn = await loginPage.IsLoggedInAsync();
        isLoggedIn.Should().BeFalse("because empty credentials should not allow login");
    }

    [Fact]
    public async Task LoginPage_ShouldLoadSuccessfully()
    {
        // Arrange & Act
        var loginPage = new LoginPage(Page!);
        await loginPage.NavigateAsync();

        // Assert
        var title = await Page!.TitleAsync();
        title.Should().Be("Dimension", "because the login page should have the correct title");

        var currentUrl = Page.Url;
        currentUrl.Should().Contain("uat-dimension.calance.us", "because we should be on the Dimension portal");
    }
}

using FluentAssertions;
using PortalAutomation.Helpers;
using PortalAutomation.Pages;

namespace PortalAutomation.Tests;

/// <summary>
/// Test class for Login functionality.
/// This is an example - update test data and assertions to match your application.
/// </summary>
public class LoginTests : TestBase
{
    [Fact]
    public async Task ValidLogin_ShouldSucceed()
    {
        // Arrange
        var loginPage = new LoginPage(Page!);
        await loginPage.NavigateAsync();

        // Act
        await loginPage.LoginAsync("testuser", "testpassword"); // Update with valid test credentials

        // Assert
        var isWelcomeDisplayed = await loginPage.IsWelcomeMessageDisplayedAsync();
        isWelcomeDisplayed.Should().BeTrue("because valid credentials should result in successful login");
    }

    [Fact]
    public async Task InvalidLogin_ShouldShowErrorMessage()
    {
        // Arrange
        var loginPage = new LoginPage(Page!);
        await loginPage.NavigateAsync();

        // Act
        await loginPage.LoginAsync("invaliduser", "wrongpassword");

        // Assert
        var isErrorDisplayed = await loginPage.IsErrorMessageDisplayedAsync();
        isErrorDisplayed.Should().BeTrue("because invalid credentials should show an error message");
    }

    [Fact]
    public async Task EmptyCredentials_ShouldShowValidationError()
    {
        // Arrange
        var loginPage = new LoginPage(Page!);
        await loginPage.NavigateAsync();

        // Act
        await loginPage.ClickLoginButtonAsync();

        // Assert
        var isErrorDisplayed = await loginPage.IsErrorMessageDisplayedAsync();
        isErrorDisplayed.Should().BeTrue("because empty credentials should trigger validation");
    }

    [Theory]
    [InlineData("user1", "pass1")]
    [InlineData("user2", "pass2")]
    [InlineData("user3", "pass3")]
    public async Task MultipleInvalidLogins_ShouldAllFail(string username, string password)
    {
        // Arrange
        var loginPage = new LoginPage(Page!);
        await loginPage.NavigateAsync();

        // Act
        await loginPage.LoginAsync(username, password);

        // Assert
        var isErrorDisplayed = await loginPage.IsErrorMessageDisplayedAsync();
        isErrorDisplayed.Should().BeTrue($"because credentials ({username}/{password}) are invalid");
    }
}

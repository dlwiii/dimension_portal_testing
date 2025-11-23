# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a C# test automation framework using Playwright and xUnit to test web portal functionality. The project follows the Page Object Model (POM) architecture pattern for maintainability and reusability.

**Tech Stack**: C# (.NET 10.0), Playwright, xUnit, FluentAssertions

## Essential Commands

### Build and Test
- **Build project**: `dotnet build`
- **Run all tests**: `dotnet test`
- **Run specific test class**: `dotnet test --filter "FullyQualifiedName~ClassName"`
- **Run specific test method**: `dotnet test --filter "FullyQualifiedName~ClassName.TestMethodName"`
- **Run with detailed output**: `dotnet test --logger "console;verbosity=detailed"`

### Playwright Setup
- **Install browsers** (required after first clone or project changes):
  ```bash
  pwsh PortalAutomation/bin/Debug/net10.0/playwright.ps1 install
  ```
- **Generate selectors** (useful for creating new page objects):
  ```bash
  pwsh PortalAutomation/bin/Debug/net10.0/playwright.ps1 codegen <url>
  ```

### Exploration Tools
- **Page Explorer Script** (`explore.csx`):
  ```bash
  dotnet script explore.csx
  ```
  Opens browser and lists all input/button elements with their attributes. Useful for finding selectors when creating new page objects.

### Package Management
- **Add package**: `dotnet add PortalAutomation/PortalAutomation.csproj package <PackageName>`
- **Restore packages**: `dotnet restore`

## Credentials and Environment Variables

**CRITICAL**: Never store credentials in code or configuration files.

### Required Environment Variables
- `PORTAL_USERNAME` - Username for Dimension portal login
- `PORTAL_PASSWORD` - Password for Dimension portal login

### Setting Environment Variables
```powershell
# PowerShell (temporary, current session only)
$env:PORTAL_USERNAME = "your_username"
$env:PORTAL_PASSWORD = "your_password"

# Then run tests
dotnet test
```

### In Tests
Tests access credentials via `Environment.GetEnvironmentVariable()`:
```csharp
private static string GetUsername() =>
    Environment.GetEnvironmentVariable("PORTAL_USERNAME")
    ?? throw new InvalidOperationException("PORTAL_USERNAME environment variable not set");
```

If environment variables are not set, tests will fail with a clear error message indicating which variable is missing.

## Dimension Portal Login Flow

The Dimension portal supports two login methods:

### 1. Standard Login (Currently Tested)
1. User enters credentials on the main login page (`/login`)
2. After successful authentication, the portal navigates to one of two pages:
   - **`/run`** - If the "use_company_id" checkbox IS checked on the login form
   - **`/login_company`** - If the "use_company_id" checkbox is NOT checked (company selection page)

**Important**: When writing tests that verify successful login, account for both possible URLs. The `LoginPage.IsLoggedInAsync()` method already handles this by checking for either destination.

### 2. Procore SSO Login (Not Currently Tested)
The login page also includes a "Sign in with Procore" button:
- **Selector**: `class="Login_procoreBtn__rsoGh btn btn-primary btn-lg"`
- **Purpose**: Provides single sign-on (SSO) authentication via Procore
- **Status**: Not currently automated. Tests use standard username/password login only.

## Architecture

### Page Object Model Structure

```
Pages/          - Page objects (LoginPage.cs, DashboardPage.cs, etc.)
Tests/          - Test classes organized by feature area (LoginTests.cs, etc.)
Helpers/        - Base classes shared across the framework
  TestBase.cs   - Handles Playwright initialization/cleanup for each test
  PageBase.cs   - Provides common page interaction methods
Models/         - Data models and test data classes
Config/         - Configuration files (appsettings.json)
```

### Key Architectural Patterns

1. **TestBase.cs** - All test classes inherit from this
   - Implements `IAsyncLifetime` for xUnit async setup/teardown
   - Initializes Playwright, Browser, Context, and Page before each test
   - Disposes resources after each test
   - Provides helper methods like `NavigateToAsync()` and `TakeScreenshotAsync()`

2. **PageBase.cs** - All page objects inherit from this
   - Requires abstract `NavigateAsync()` method
   - Provides common methods: `ClickAsync()`, `FillAsync()`, `GetTextAsync()`, etc.
   - Encapsulates Playwright IPage interactions

3. **Page Objects** (e.g., LoginPage.cs)
   - One class per page/component
   - Define selectors as private constants
   - Expose public methods for user actions
   - No test logic or assertions in page objects

4. **Test Classes** (e.g., LoginTests.cs)
   - Inherit from TestBase
   - Use xUnit attributes: `[Fact]`, `[Theory]`, `[InlineData]`
   - Follow Arrange-Act-Assert pattern
   - Use FluentAssertions for readable assertions

## Adding New Tests

### Creating a New Page Object
1. Create `Pages/YourPage.cs` inheriting from `PageBase`
2. Define selectors as constants (use data-testid attributes when possible)
3. Implement `NavigateAsync()` method
4. Create methods for user actions (clicks, fills, validations)
5. Return values only for assertions (visibility checks, text content, etc.)

**Example**:
```csharp
public class DashboardPage : PageBase
{
    private const string PageUrl = "https://portal.com/dashboard";
    private const string UserMenuSelector = "[data-testid='user-menu']";

    public DashboardPage(IPage page) : base(page) { }

    public override async Task NavigateAsync() => await Page.GotoAsync(PageUrl);

    public async Task ClickUserMenuAsync() => await ClickAsync(UserMenuSelector);
}
```

### Creating a New Test Class
1. Create `Tests/YourTests.cs` inheriting from `TestBase`
2. Use dependency injection pattern with page objects
3. Name tests: `MethodUnderTest_Scenario_ExpectedBehavior`
4. Use FluentAssertions for assertions

**Example**:
```csharp
public class DashboardTests : TestBase
{
    [Fact]
    public async Task UserMenu_WhenClicked_ShouldShowOptions()
    {
        // Arrange
        var dashboardPage = new DashboardPage(Page!);
        await dashboardPage.NavigateAsync();

        // Act
        await dashboardPage.ClickUserMenuAsync();

        // Assert
        var isVisible = await dashboardPage.IsMenuVisibleAsync();
        isVisible.Should().BeTrue();
    }
}
```

## Important Patterns

### Organizing Tests by Feature Area
- Create separate test files for each major feature (Login, Dashboard, Reports, etc.)
- Group related tests in the same class
- Use descriptive class and method names

### Working with Selectors
- Prefer data-testid attributes: `[data-testid='login-button']`
- Use CSS selectors over XPath when possible
- Define all selectors as constants at the top of page objects
- Update selectors in one place when UI changes

### Async/Await Pattern
- All Playwright methods are async
- Always use `await` with Playwright operations
- Test methods must be `async Task`
- Use `Task` return type, not `void`

### Configuration Management
- Edit `Config/appsettings.json` for test settings
- Store test URLs, browser preferences, timeouts
- Never commit credentials - use environment variables for sensitive data

## Common Tasks

### Running a Single Test During Development
```bash
dotnet test --filter "FullyQualifiedName~YourTestClass.YourTestMethod"
```

### Debugging Tests
- Set `Headless = false` in TestBase.cs to see browser
- Adjust `SlowMo` value to slow down operations
- Use `await TakeScreenshotAsync("debug.png")` to capture state
- Run single test with debugger attached in IDE

### Updating for UI Changes
- When application UI changes, update selectors in affected page objects only
- Tests should not need changes if page object methods remain the same
- This is the main benefit of the Page Object Model

### Handling Flaky Tests
- Playwright has built-in auto-wait - avoid manual `Thread.Sleep()` or `Task.Delay()`
- Use `WaitForElementAsync()` for elements loaded dynamically
- Increase timeout values in appsettings.json if needed
- Check for race conditions in parallel operations

## File Naming Conventions

- Page Objects: `{PageName}Page.cs` (e.g., LoginPage.cs, DashboardPage.cs)
- Tests: `{Feature}Tests.cs` (e.g., LoginTests.cs, ReportsTests.cs)
- Models: `{ModelName}.cs` (e.g., User.cs, TestData.cs)
- Helpers/Utilities: `{Purpose}Helper.cs` or descriptive names

## Dependencies

Current NuGet packages:
- **Microsoft.Playwright** (1.56.0) - Browser automation
- **FluentAssertions** (8.8.0) - Readable assertions
- **xunit** - Test framework (included in project template)
- **Microsoft.NET.Test.Sdk** - Test runner

## Browser Management

Playwright supports three browser engines:
- **Chromium** (default in TestBase.cs)
- **Firefox**: Change to `Playwright.Firefox.LaunchAsync()`
- **WebKit**: Change to `Playwright.Webkit.LaunchAsync()`

Configure in TestBase.cs or make it configurable via appsettings.json.

# Portal Automation

A C# test automation framework using Playwright and xUnit for testing web portal functionality.

## Technology Stack

- **Language**: C# (.NET 10.0)
- **Test Framework**: xUnit
- **Browser Automation**: Playwright
- **Assertion Library**: FluentAssertions
- **Architecture**: Page Object Model (POM)

## Project Structure

```
PortalAutomation/
├── Pages/           # Page Object classes representing application pages
├── Tests/           # Test classes organized by feature area
├── Helpers/         # Base classes and utility methods
│   ├── TestBase.cs  # Base class for all tests (Playwright setup/teardown)
│   └── PageBase.cs  # Base class for all page objects
├── Models/          # Data models and test data classes
└── Config/          # Configuration files (appsettings.json)
```

## Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download) or later
- PowerShell (for Playwright browser installation)

## Setup

1. **Restore NuGet packages**:
   ```bash
   dotnet restore
   ```

2. **Install Playwright browsers**:
   ```bash
   pwsh PortalAutomation/bin/Debug/net10.0/playwright.ps1 install
   ```

   Or on Windows PowerShell:
   ```powershell
   .\PortalAutomation\bin\Debug\net10.0\playwright.ps1 install
   ```

3. **Update configuration**:
   - Edit `PortalAutomation/Config/appsettings.json`
   - Set your portal's base URL
   - Configure test credentials (use environment variables for sensitive data in CI/CD)

4. **Update page objects**:
   - Edit `PortalAutomation/Pages/LoginPage.cs`
   - Update selectors to match your application's HTML structure
   - Update URLs to match your portal

## Running Tests

### Build the project
```bash
dotnet build
```

### Run all tests
```bash
dotnet test
```

### Run tests with detailed output
```bash
dotnet test --logger "console;verbosity=detailed"
```

### Run a specific test class
```bash
dotnet test --filter "FullyQualifiedName~LoginTests"
```

### Run a specific test method
```bash
dotnet test --filter "FullyQualifiedName~LoginTests.ValidLogin_ShouldSucceed"
```

### Run tests by category (if using Traits)
```bash
dotnet test --filter "Category=Smoke"
```

## Creating New Tests

1. **Create a new page object** in `Pages/`:
   ```csharp
   public class DashboardPage : PageBase
   {
       // Define selectors and methods
   }
   ```

2. **Create a new test class** in `Tests/`:
   ```csharp
   public class DashboardTests : TestBase
   {
       [Fact]
       public async Task YourTest()
       {
           // Arrange, Act, Assert
       }
   }
   ```

## Configuration

Edit `Config/appsettings.json` to customize:
- Base URL
- Browser type (chromium, firefox, webkit)
- Headless mode
- Viewport size
- Timeouts
- Test data

## Best Practices

- Keep page objects focused on a single page or component
- Use descriptive test names following the pattern: `MethodUnderTest_Scenario_ExpectedBehavior`
- Use FluentAssertions for readable assertions
- Take screenshots on test failures for debugging
- Keep selectors updated when UI changes
- Use data-testid attributes in your application for stable selectors

## Troubleshooting

### Playwright browsers not found
Run the browser installation command again:
```bash
pwsh PortalAutomation/bin/Debug/net10.0/playwright.ps1 install
```

### Tests are flaky
- Increase timeout values in `appsettings.json`
- Use Playwright's built-in auto-wait instead of manual waits
- Check for race conditions in your tests

### Selectors not working
- Verify the HTML structure of your application
- Use Playwright's Inspector to find correct selectors:
  ```bash
  pwsh PortalAutomation/bin/Debug/net10.0/playwright.ps1 codegen <your-url>
  ```

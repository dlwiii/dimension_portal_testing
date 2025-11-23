#!/usr/bin/env dotnet-script
#r "nuget: Microsoft.Playwright, 1.56.0"

using Microsoft.Playwright;

var playwright = await Playwright.CreateAsync();
var browser = await playwright.Chromium.LaunchAsync(new() { Headless = false, SlowMo = 1000 });
var page = await browser.NewPageAsync();

await page.GotoAsync("https://uat-dimension.calance.us");
await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

// Get all form inputs
var inputs = await page.QuerySelectorAllAsync("input");
Console.WriteLine($"Found {inputs.Count} input elements:\n");

foreach (var input in inputs)
{
    var id = await input.GetAttributeAsync("id") ?? "";
    var name = await input.GetAttributeAsync("name") ?? "";
    var type = await input.GetAttributeAsync("type") ?? "";
    var placeholder = await input.GetAttributeAsync("placeholder") ?? "";
    var className = await input.GetAttributeAsync("class") ?? "";

    Console.WriteLine($"Input: type='{type}'");
    if (!string.IsNullOrEmpty(id)) Console.WriteLine($"  id: {id}");
    if (!string.IsNullOrEmpty(name)) Console.WriteLine($"  name: {name}");
    if (!string.IsNullOrEmpty(placeholder)) Console.WriteLine($"  placeholder: {placeholder}");
    if (!string.IsNullOrEmpty(className)) Console.WriteLine($"  class: {className}");
    Console.WriteLine();
}

// Get all buttons
var buttons = await page.QuerySelectorAllAsync("button");
Console.WriteLine($"\nFound {buttons.Count} button elements:\n");

foreach (var button in buttons)
{
    var id = await button.GetAttributeAsync("id") ?? "";
    var type = await button.GetAttributeAsync("type") ?? "";
    var className = await button.GetAttributeAsync("class") ?? "";
    var text = await button.TextContentAsync() ?? "";

    Console.WriteLine($"Button: '{text.Trim()}'");
    if (!string.IsNullOrEmpty(id)) Console.WriteLine($"  id: {id}");
    if (!string.IsNullOrEmpty(type)) Console.WriteLine($"  type: {type}");
    if (!string.IsNullOrEmpty(className)) Console.WriteLine($"  class: {className}");
    Console.WriteLine();
}

Console.WriteLine("\nPress Enter to close...");
Console.ReadLine();

await browser.CloseAsync();

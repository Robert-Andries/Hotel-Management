using FluentAssertions;
using Microsoft.Playwright;

namespace HM.Tests.Playwright;

public class WebUiTests : IClassFixture<PlaywrightFixture>
{
    private readonly PlaywrightFixture _fixture;
    private readonly string _hostUrl = "https://localhost:7003";

    public WebUiTests(PlaywrightFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Homepage_Should_HaveCorrectTitle()
    {
        await using var browser =
            await _fixture.Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
        var page = await browser.NewPageAsync();

        // Ensure WebApp is running
        try
        {
            await page.GotoAsync(_hostUrl);
            var title = await page.TitleAsync();
            title.Should().Contain("Robert Hotel");
        }
        catch (PlaywrightException ex)
        {
            Assert.Fail($"Failed to connect to WebUI. Ensure it is running at {_hostUrl}. Error: {ex.Message}");
        }
    }

    [Fact]
    public async Task PrivacyLink_Should_BeVisible()
    {
        await using var browser =
            await _fixture.Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
        var page = await browser.NewPageAsync();

        try
        {
            await page.GotoAsync(_hostUrl);
            var privacyLink =
                page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Privacy", Exact = true });
            await privacyLink.WaitForAsync(new LocatorWaitForOptions { Timeout = 5000 });

            (await privacyLink.IsVisibleAsync()).Should().BeTrue();
        }
        catch (PlaywrightException ex)
        {
            Assert.Fail($"Failed to connect to WebUI or find element. Ensure App is running. Error: {ex.Message}");
        }
    }
}

public class PlaywrightFixture : IDisposable
{
    public PlaywrightFixture()
    {
        Playwright = Microsoft.Playwright.Playwright.CreateAsync().Result;
    }

    public IPlaywright Playwright { get; }

    public void Dispose()
    {
        Playwright.Dispose();
    }
}
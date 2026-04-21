using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

[TestFixture]
public class LoginTests : PageTest
{
    [SetUp]
    public async Task SetUp()
    {
        await Page.GotoAsync("https://automationexercise.com/login");

        // Wait for cookie popup and click it
        try
        {
            await Page.WaitForSelectorAsync(".fc-button.fc-cta-consent.fc-primary-button",
                new PageWaitForSelectorOptions { Timeout = 5000 });
            await Page.ClickAsync(".fc-button.fc-cta-consent.fc-primary-button");
            await Page.WaitForTimeoutAsync(1000);
        }
        catch (TimeoutException)
        {
            // Popup didn't appear, continue without clicking
        }
    }

    //Test 1: Successfull Login
    [Test]
    public async Task ValidLogin_RedirectsToHomePage()
    {
        await Page.FillAsync("[data-qa='login-email']", "example@email.com");

        await Page.FillAsync("[data-qa='login-password']", "examplepassword");

        await Page.ClickAsync("[data-qa='login-button']");

        //Asserts user lands on dashboard
        await Expect(Page).ToHaveTitleAsync("Automation Exercise - Signup / Login");
        await Expect(Page).ToHaveURLAsync("https://automationexercise.com/login");
    }
    //Test wrong details show error
    [Test]
    public async Task InvalidLogin_ShowsErrorMessage()
    {
        await Page.FillAsync("[data-qa='login-email']", "wrong@email.com");

        await Page.FillAsync("[data-qa='login-password']", "wrongpassword");

        await Page.ClickAsync("[data-qa='login-button']");


        //Show error message
        var errorMessage = Page.Locator("p:has-text('Your email or password is incorrect!')");
        await Expect(errorMessage).ToBeVisibleAsync();
    }
}

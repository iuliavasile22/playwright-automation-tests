using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

[TestFixture]
//Mockup change
public class LoginTests : PageTest
{
    [SetUp]
    public async Task SetUp()

    {
        await Page.GotoAsync("https://automationexercise.com/login");
    }

    //Test 1: Successfull Login
    [Test]
    public async Task ValidLogin_RedirectsToHomePage()
    {
        await Page.FillAsync("#email", "mockupemail.com");
        await Page.FillAsync("#password", "testPassword");
        await Page.ClickAsync("button[type='submit']");

        //Asserts user lands on dashboard
        await Expect(Page).ToHaveTitleAsync("https://automationexercise.com/");
        await Expect(Page.Locator("h1, span")).ToContainTextAsync("Automation Exercise");
    }

    //Test wrong details show error
    [Test]
    public async Task InvalidLogin_ShowsErrorMessage()
    {
        await Page.FillAsync("#email", "wrong@email.com");
        await Page.FillAsync("#password", "wrongpassword");
        await Page.ClickAsync("button[type='submit']");

        //Show error message
        var errorMessage = Page.Locator(".error-message");
        await Expect(errorMessage).ToBeVisibleAsync();
        await Expect(errorMessage).ToContainTextAsync("Invalid input user data");
    }


}

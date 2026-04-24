using System.Runtime.CompilerServices;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

[TestFixture]

public class LoginTests : PageTest
{

  [SetUp]
  public async Task SetUp()
  {
    await Page.GotoAsync("https://automationexercise.com/");
    try
    {

      //handle cookie popup
      await Page.WaitForSelectorAsync(".fc-button.fc-cta-consent.fc-primary-button",
                 new PageWaitForSelectorOptions { Timeout = 4000 });
      await Page.ClickAsync(".fc-button.fc-cta-consent.fc-primary-button");
      await Page.WaitForTimeoutAsync(1000);

    }
    catch (TimeoutException)
    {

    }
  }

  [Test]

  public async Task Valid_LoginTest()
  {
    await Expect(Page).ToHaveURLAsync("https://automationexercise.com/");

    await Page.ClickAsync("a:has-text('Signup / Login')");

    //Verify that 'Login to your account is visible'
    await Expect(Page.Locator("h2:has-text('Login to your account')")).ToBeVisibleAsync();

    //Enter correct email address and password
    await Page.FillAsync("data-qa='login-email'", "test_email123@gmail.com");
    await Page.FillAsync("data-qa=''login-password", "password1243");
    await Page.ClickAsync("[data-qa='login-button']");

    await Expect(Page.Locator("a:has-text('Logged in as')")).ToBeVisibleAsync();

    //Delete account
    await Page.ClickAsync("a:has-text('Delete Account')");
    await Expect(Page.Locator("h2:has-text('Account deleted!')")).ToBeVisibleAsync();
  }

  [Test]

  public async Task Invalid_LoginTest()
  {
    await Expect(Page).ToHaveURLAsync("https://automationexercise.com/");

    await Page.ClickAsync("a:has-text('Signup / Login')");

    //Verify that 'Login to your account is visible'
    await Expect(Page.Locator("h2:has-text('Login to your account')")).ToBeVisibleAsync();

    //Enter correct email address and password
    await Page.FillAsync("data-qa='login-email'", "wrong_email123@gmail.com");
    await Page.FillAsync("data-qa=''login-password", "wrongpassword1243");
    await Page.ClickAsync("[data-qa='login-button']");

    await Expect(Page.Locator("h2 p:has-text('Your email or password is incorrect!')")).ToBeVisibleAsync();

  }
}
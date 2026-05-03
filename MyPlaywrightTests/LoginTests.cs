using System.Runtime.CompilerServices;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

[TestFixture]

public class LoginTests : PageTest
{
  private string saved_email;
  private string saved_password;
  private bool _accountCreated = false;

  private async Task NavigateToHomepage()
  {
    await Page.GotoAsync("https://automationexercise.com/");
    try
    {
      await Page.WaitForSelectorAsync(".fc-button.fc-cta-consent.fc-primary-button",
          new PageWaitForSelectorOptions { Timeout = 4000 });
      await Page.ClickAsync(".fc-button.fc-cta-consent.fc-primary-button");
      await Page.WaitForTimeoutAsync(1000);
    }
    catch (TimeoutException) { }
  }

  [SetUp]
  public async Task SetUp()
  {
    await NavigateToHomepage();

    if (!_accountCreated)
    {
      var helper = new AccountRegistrationHelper(Page);
      await helper.RegisterAccount();
      saved_email = helper.Email;
      saved_password = helper.Password;
      _accountCreated = true;
      await NavigateToHomepage();
    }
  }
  [Test]

  public async Task Valid_LoginTest()
  {
    // Verify home page is visible successfully
    await Expect(Page).ToHaveURLAsync("https://automationexercise.com/");
    await Expect(Page.Locator("img[alt='Website for automation practice']")).ToBeVisibleAsync();

    await Page.ClickAsync("a:has-text('Signup / Login')");

    //Verify that 'Login to your account is visible'
    await Expect(Page.Locator("h2:has-text('Login to your account')")).ToBeVisibleAsync();

    //Enter correct email address and password
    await Page.FillAsync("[data-qa='login-email']", saved_email);
    await Page.FillAsync("[data-qa='login-password']", saved_password);
    await Page.ClickAsync("[data-qa='login-button']");

    await Expect(Page.Locator("a:has-text('Logged in as')")).ToBeVisibleAsync();

    //Delete account
    await Page.ClickAsync("a:has-text('Delete Account')");
    await Expect(Page.Locator("h2:has-text('Account deleted!')")).ToBeVisibleAsync();
  }

  [Test]

  public async Task Invalid_LoginTest()
  {
    // Verify home page is visible successfully
    await Expect(Page).ToHaveURLAsync("https://automationexercise.com/");
    await Expect(Page.Locator("img[alt='Website for automation practice']")).ToBeVisibleAsync();
    await Page.ClickAsync("a:has-text('Signup / Login')");

    //Verify that 'Login to your account is visible'
    await Expect(Page.Locator("h2:has-text('Login to your account')")).ToBeVisibleAsync();

    //Enter correct email address and password
    await Page.FillAsync("[data-qa='login-email']", "wrong_email@gmail.com");
    await Page.FillAsync("[data-qa='login-password']", "wrong_password");
    await Page.ClickAsync("[data-qa='login-button']");

    await Expect(Page.Locator("p:has-text('Your email or password is incorrect!')")).ToBeVisibleAsync();

  }
}
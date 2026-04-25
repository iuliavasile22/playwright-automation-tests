using System.Runtime.CompilerServices;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

[TestFixture]

public class LogoutTest : PageTest
{


  private string saved_email;
  private string saved_password;

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

    var helper = new AccountRegistrationHelper(Page);
    await helper.RegisterAccount();

    saved_email = helper.Email;
    saved_password = helper.Password;
  }

  [Test]

  public async Task Valid_Logout_Test()
  {

    await Page.ClickAsync("a:has-text('Signup / Login')");

    //Verify that 'Login to your account is visible'
    await Expect(Page.Locator("h2:has-text('Login to your account')")).ToBeVisibleAsync();

    //Enter correct email address and password
    await Page.FillAsync("[data-qa='login-email']", saved_email);
    await Page.FillAsync("[data-qa='login-password']", saved_password);
    await Page.ClickAsync("[data-qa='login-button']");

    await Expect(Page.Locator("a:has-text('Logged in as')")).ToBeVisibleAsync();

    await Page.ClickAsync("a:has-text('Logout')");
    await Expect(Page).ToHaveURLAsync("https://automationexercise.com/login");

  }

}
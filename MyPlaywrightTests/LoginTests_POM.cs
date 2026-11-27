using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using MyPlaywrightTests;
using NUnit.Framework;

[TestFixture]

public class LoginTests_POM : PageTest
{
  private string saved_email = string.Empty;
  private string saved_password = string.Empty;
  private bool _accountCreated = false;

  private async Task NavigateToHomepage()
  {
    await Page.AddInitScriptAsync("window.alert = () => true;");
    await Page.GotoAsync("https://automationexercise.com/");
    try
    {
      await Page.WaitForSelectorAsync(".fc-button.fc-cta-consent.fc-primary-button",
          new PageWaitForSelectorOptions { Timeout = 3000 });
      await Page.ClickAsync(".fc-button.fc-cta-consent.fc-primary-button");
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
    var loginPage = new LoginPage(Page);
    await loginPage.Open();
    await Expect(loginPage.LoginHeader).ToBeVisibleAsync();

    await loginPage.Login(saved_email, saved_password);

    await Expect(Page.Locator("a:has-text('Logged in as')")).ToBeVisibleAsync();
  }


  [Test]
  public async Task Invalid_LoginTest()
  {
    var loginPage = new LoginPage(Page);
    await loginPage.Open();
    await Expect(loginPage.LoginHeader).ToBeVisibleAsync();

    await loginPage.Login("wrong_email@gmail.com", "wrong_password");
    await Expect(loginPage.ErrorMessage).ToBeVisibleAsync();
  }
}
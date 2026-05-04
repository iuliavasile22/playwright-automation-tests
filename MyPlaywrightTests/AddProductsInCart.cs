using System.Runtime.CompilerServices;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

[TestFixture]
public class AddProductsInCart : PageTest
{
  static AddProductsInCart()
  {
    Environment.SetEnvironmentVariable("HEADED", "1");
  }

  private string saved_email = string.Empty;
  private string saved_password = string.Empty;
  private bool _accountCreated = false;

  private async Task NavigateToHomepage()
  {
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

  public async Task Valid_AddProductsInCart_Test()
  {
    await Page.ClickAsync("a:has-text('Products')");
  }
}
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System.Text.RegularExpressions;

[TestFixture]
public class Invalid_search_input : PageTest
{
  private string saved_email = String.Empty;
  private string saved_password = String.Empty;
  private bool _accountCreated = false;

  static Invalid_search_input()
  {
    Environment.SetEnvironmentVariable("HEADED", "1");
  }
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
    if (!_accountCreated)
    {
      await NavigateToHomepage();
      var helper = new AccountRegistrationHelper(Page);
      await helper.RegisterAccount();
      saved_email = helper.Email;
      saved_password = helper.Password;
      _accountCreated = true;
      await NavigateToHomepage();

      return;
    }
    await NavigateToHomepage();
  }

  private async Task DismissAds()
  {
    await Page.EvaluateAsync(@"
        document.querySelectorAll('ins, .adsbygoogle, iframe[id*=""google""], #google_vignette').forEach(el => el.remove());
    ");
    try
    {
      await Page.Keyboard.PressAsync("Escape");
    }
    catch (Exception) { }
  }

  [Test]
  public async Task Invalid_search_input_Test()
  {

  }
}
using System.Runtime.CompilerServices;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

[TestFixture]
public class SearchProduct : PageTest
{
  static SearchProduct()
  {
    Environment.SetEnvironmentVariable("HEADED", "1");
  }

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
    if (!_accountCreated)
    {
      await NavigateToHomepage();
      var helper = new AccountRegistrationHelper(Page);
      await helper.RegisterAccount();
      _accountCreated = true;
      return;
    }

    await NavigateToHomepage();
  }


  [Test]

  public async Task SearchProduct_Test()
  {
    //Click products button
    await Page.ClickAsync("a:has-text('Products')");

    // Remove ads on the page
    await Page.EvaluateAsync(@"
            document.querySelectorAll('ins, .adsbygoogle, iframe[id*=""google""]').forEach(el => el.remove());
        ");

    await Page.GotoAsync("https://automationexercise.com/products");

    //Fill the search input field
    await Page.FillAsync("[id='search_product']", "Winter Top");
    await Page.ClickAsync("[id='submit_search']");

    // Verify searched products heading is visible
    await Expect(Page.Locator("h2:has-text('Searched Products')")).ToBeVisibleAsync();

    var productNames = Page.Locator(".productinfo p");
    var count = await productNames.CountAsync();

    for (int i = 0; i < count; i++)
    {
      var productText = await productNames.Nth(i).InnerTextAsync();
      Assert.That(productText.ToLower(), Does.Contain("winter top"));
    }
  }
}
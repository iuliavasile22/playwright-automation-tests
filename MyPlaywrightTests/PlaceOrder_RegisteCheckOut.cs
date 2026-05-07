using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

[TestFixture]
public class PlaceOrder_RegisterCheckOut : PageTest
{
  static PlaceOrder_RegisterCheckOut()
  {
    Environment.SetEnvironmentVariable("HEADED", "1");
  }

  [SetUp]
  public async Task SetUp()
  {
    await Page.AddInitScriptAsync("window.alert = () => true;");
    await Page.GotoAsync("https://automationexercise.com/");
    try
    {
      await Page.WaitForSelectorAsync(".fc-button.fc-cta-consent.fc-primary-button",
          new PageWaitForSelectorOptions { Timeout = 4000 });
      await Page.ClickAsync(".fc-button.fc-cta-consent.fc-primary-button");
    }
    catch (TimeoutException) { }
  }

  [Test]
  public async Task RegisterCheckOut()
  {

    // Navigate to product page
    await Page.GotoAsync("https://automationexercise.com/product_details/1");

    // Remove ads on the page
    await Page.EvaluateAsync(@"
            document.querySelectorAll('ins, .adsbygoogle, iframe[id*=""google""]').forEach(el => el.remove());
        ");


    await Page.Locator("input#quantity").ClearAsync();
    await Page.Locator("input#quantity").FillAsync("2");

    await Page.ClickAsync("button:has-text('Add to cart')");
  }
}
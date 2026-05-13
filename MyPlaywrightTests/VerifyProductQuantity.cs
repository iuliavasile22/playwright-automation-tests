using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

[TestFixture]
public class VerifyProductQuantity : PageTest
{
  static VerifyProductQuantity()
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
  public async Task VerifyProductQuantity_Test()
  {
    await Expect(Page).ToHaveURLAsync("https://automationexercise.com/");
    await Expect(Page).ToHaveTitleAsync("Automation Exercise");

    await Page.ClickAsync(" a[href='/product_details/1']");

    // Remove ads on the page
    await Page.EvaluateAsync(@"
            document.querySelectorAll('ins, .adsbygoogle, iframe[id*=""google""]').forEach(el => el.remove());
        ");

    await Page.Locator("input#quantity").FillAsync("4");

    await Page.ClickAsync("button.btn.btn-default.cart");

    await Page.ClickAsync("#cartModal a[href='/view_cart']");

    // Verify product quantity in cart
    await Expect(Page.Locator("td.cart_quantity button"))
        .ToContainTextAsync("4");
  }
}
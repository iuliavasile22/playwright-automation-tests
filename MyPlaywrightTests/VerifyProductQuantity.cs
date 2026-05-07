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
          new PageWaitForSelectorOptions { Timeout = 4000 });
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
      return;
    }

    await NavigateToHomepage();
  }

  [Test]
  public async Task VerifyProductQuantity_Test()
  {
    // Navigate to product page
    await Page.GotoAsync("https://automationexercise.com/product_details/1");

    // Remove ads on the page
    await Page.EvaluateAsync(@"
            document.querySelectorAll('ins, .adsbygoogle, iframe[id*=""google""]').forEach(el => el.remove());
        ");


    await Page.Locator("input#quantity").ClearAsync();
    await Page.Locator("input#quantity").FillAsync("4");

    await Page.ClickAsync("button:has-text('Add to cart')");

    // Modal appears - click View Cart
    await Page.ClickAsync("#cartModal a[href='/view_cart']");

    // Verify product quantity in cart
    await Expect(Page.Locator("td.cart_quantity button"))
        .ToContainTextAsync("4");
  }
}
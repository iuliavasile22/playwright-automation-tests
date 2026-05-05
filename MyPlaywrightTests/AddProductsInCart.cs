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
    await NavigateToHomepage();

    if (!_accountCreated)
    {
      var helper = new AccountRegistrationHelper(Page);
      await helper.RegisterAccount();
      saved_email = helper.Email;
      saved_password = helper.Password;
      _accountCreated = true;
      await NavigateToHomepage();
      return;
    }
  }

  [Test]
  public async Task Valid_AddProductsInCart_Test()
  {
    await Page.ClickAsync("a:has-text('Products')");

    // Remove ads before interacting
    await Page.EvaluateAsync(@"
            document.querySelectorAll('ins, .adsbygoogle, iframe[id*=""google""]').forEach(el => el.remove());
        ");

    // Add first product to cart
    await Page.Locator(".product-image-wrapper").Nth(0).HoverAsync();
    await Page.ClickAsync(".add-to-cart[data-product-id='1']");
    await Page.ClickAsync(".modal-footer .btn:has-text('Continue Shopping')");

    // Add second product to cart
    await Page.Locator(".product-image-wrapper").Nth(1).HoverAsync();
    await Page.ClickAsync(".add-to-cart[data-product-id='2']");

    await Page.ClickAsync("a[href='/view_cart']");

    await Expect(Page.Locator("td.cart_description h4 a:has-text('Blue Top')"))
        .ToBeVisibleAsync(new() { Timeout = 15000 });

    await Expect(Page.Locator("td.cart_description a:has-text('Men Tshirt')"))
        .ToBeVisibleAsync(new() { Timeout = 15000 });

    // Verify prices, quantity and total
    await Expect(Page.Locator("td.cart_price").Nth(0)).ToContainTextAsync("Rs. 500");
    await Expect(Page.Locator("td.cart_quantity").Nth(0)).ToContainTextAsync("1");
    await Expect(Page.Locator("td.cart_total").Nth(0)).ToContainTextAsync("Rs. 500");

    await Expect(Page.Locator("td.cart_price").Nth(1)).ToContainTextAsync("Rs. 400");
    await Expect(Page.Locator("td.cart_quantity").Nth(1)).ToContainTextAsync("1");
    await Expect(Page.Locator("td.cart_total").Nth(1)).ToContainTextAsync("Rs. 400");
  }
}
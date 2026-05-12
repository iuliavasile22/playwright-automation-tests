using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System.Text.RegularExpressions;

[TestFixture]
public class View_cart_after_navigation : PageTest
{
  private string saved_email = String.Empty;
  private string saved_password = String.Empty;
  private bool _accountCreated = false;

  static View_cart_after_navigation()
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
  public async Task View_cart_after_navigation_Test()
  {

    await Page.ClickAsync("a:has-text('Signup / Login')");

    //Verify that 'Login to your account is visible'
    await Expect(Page.Locator("h2:has-text('Login to your account')")).ToBeVisibleAsync();

    //Enter correct email address and password
    await Page.FillAsync("[data-qa='login-email']", saved_email);
    await Page.FillAsync("[data-qa='login-password']", saved_password);
    await Page.ClickAsync("[data-qa='login-button']");

    await Expect(Page.Locator("a:has-text('Logged in as')")).ToBeVisibleAsync();

    // Add first product to cart
    await Page.Locator(".product-image-wrapper").Nth(0).HoverAsync();
    await Page.ClickAsync(".add-to-cart[data-product-id='1']");
    await Page.ClickAsync(".modal-footer .btn:has-text('Continue Shopping')");

    // Add second product to cart
    await Page.Locator(".product-image-wrapper").Nth(1).HoverAsync();
    await Page.ClickAsync(".add-to-cart[data-product-id='2']");

    await Page.ClickAsync("a[href='/view_cart']");

    await DismissAds();

    await Page.GotoAsync("https://automationexercise.com/products");

    await DismissAds();

    await Page.GotoAsync("https://automationexercise.com/view_cart");

    await DismissAds();

    // Verify prices, quantity and total
    await Expect(Page.Locator("td.cart_price").Nth(0)).ToContainTextAsync("Rs. 500");
    await Expect(Page.Locator("td.cart_quantity").Nth(0)).ToContainTextAsync("1");
    await Expect(Page.Locator("td.cart_total").Nth(0)).ToContainTextAsync("Rs. 500");

    await Expect(Page.Locator("td.cart_price").Nth(1)).ToContainTextAsync("Rs. 400");
    await Expect(Page.Locator("td.cart_quantity").Nth(1)).ToContainTextAsync("1");
    await Expect(Page.Locator("td.cart_total").Nth(1)).ToContainTextAsync("Rs. 400");
  }
}
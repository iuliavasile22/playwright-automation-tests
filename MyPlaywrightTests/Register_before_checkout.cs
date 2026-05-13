using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System.Text.RegularExpressions;

[TestFixture]
public class Register_before_Checkout : PageTest
{
  private string saved_email = String.Empty;
  private string saved_password = String.Empty;
  private bool _accountCreated = false;

  static Register_before_Checkout()
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
  public async Task Register_before_checkout_Test()
  {
    await Expect(Page).ToHaveURLAsync("https://automationexercise.com/");
    await Expect(Page).ToHaveTitleAsync("Automation Exercise");

    // Dismiss ads on new page
    await DismissAds();

    await Page.ClickAsync("a:has-text('Signup / Login')");

    //Verify that 'Login to your account is visible'
    await Expect(Page.Locator("h2:has-text('Login to your account')")).ToBeVisibleAsync();

    //Enter correct email address and password
    await Page.FillAsync("[data-qa='login-email']", saved_email);
    await Page.FillAsync("[data-qa='login-password']", saved_password);
    await Page.ClickAsync("[data-qa='login-button']");

    // Verify user is logged in
    await Expect(Page.Locator("a:has-text('Logged in as')")).ToBeVisibleAsync(new() { Timeout = 700 });

    // Add first product to cart
    await Page.Locator(".product-image-wrapper").Nth(1).HoverAsync();
    await Page.ClickAsync(".add-to-cart[data-product-id='2']");
    await Page.ClickAsync(".modal-footer .btn:has-text('Continue Shopping')");

    // Add second product to cart
    await Page.Locator(".product-image-wrapper").Nth(2).HoverAsync();
    await Page.ClickAsync(".add-to-cart[data-product-id='3']");
    await Page.ClickAsync(".modal-footer .btn:has-text('Continue Shopping')");

    //Add third product to cart
    await Page.Locator(".product-image-wrapper").Nth(3).HoverAsync();
    await Page.ClickAsync(".add-to-cart[data-product-id='4']");
    await Page.ClickAsync(".modal-footer .btn:has-text('Continue Shopping')");

    //Click cart button
    await Page.ClickAsync("a[href='/view_cart']");
    await Expect(Page).ToHaveURLAsync("https://automationexercise.com/view_cart");

    await Page.ClickAsync("a:has-text('Proceed To Checkout')");

    await Expect(Page.Locator("#address_delivery")).ToContainTextAsync("Test");
    await Expect(Page.Locator("#address_delivery")).ToContainTextAsync("User");
    await Expect(Page.Locator("#address_delivery")).ToContainTextAsync("Test Company");

    await Expect(Page.Locator("#address_delivery")).ToContainTextAsync("123 Test Street");
    await Expect(Page.Locator("#address_delivery")).ToContainTextAsync("456 Test Street");

    await Expect(Page.Locator("#address_delivery")).ToContainTextAsync("India");

    await Expect(Page.Locator("#address_delivery")).ToContainTextAsync("Test State");
    await Expect(Page.Locator("#address_delivery")).ToContainTextAsync("Test City");
    await Expect(Page.Locator("#address_delivery")).ToContainTextAsync("12345");
    await Expect(Page.Locator("#address_delivery")).ToContainTextAsync("1234567890");

    // Review order - verify products are in the order summary
    await Expect(Page.Locator("td.cart_description h4 a:has-text('Men Tshirt')")).ToBeVisibleAsync();
    await Expect(Page.Locator("td.cart_description h4 a:has-text('Sleeveless Dress')")).ToBeVisibleAsync();
    await Expect(Page.Locator("td.cart_description h4 a:has-text('Stylish Dress')")).ToBeVisibleAsync();


    // Verify prices in order summary
    await Expect(Page.Locator("td.cart_price").Nth(0)).ToContainTextAsync("Rs. 400");
    await Expect(Page.Locator("td.cart_price").Nth(1)).ToContainTextAsync("Rs. 1000");
    await Expect(Page.Locator("td.cart_price").Nth(2)).ToContainTextAsync("Rs. 1500");

    // Verify total
    await Expect(Page.Locator("p.cart_total_price").Last).ToContainTextAsync("Rs. 2900");

    // Enter description in comment text area
    await Page.FillAsync("textarea.form-control", "This is a test order comment.");

    // Click Place Order
    await Page.ClickAsync("a:has-text('Place Order')");

    await DismissAds();

    // Enter payment details
    await Page.FillAsync("[data-qa='name-on-card']", "Mockup_name User");
    await Page.FillAsync("[data-qa='card-number']", "4111111111111211");
    await Page.FillAsync("[data-qa='cvc']", "124");
    await Page.FillAsync("[data-qa='expiry-month']", "4");
    await Page.FillAsync("[data-qa='expiry-year']", "2027");

    // Click Pay and Confirm Order
    await Page.ClickAsync("[data-qa='pay-button']");

    await Page.WaitForLoadStateAsync(LoadState.Load);

    // Verify order placed successfully - most reliable assertion
    await Expect(Page).ToHaveTitleAsync("Automation Exercise - Order Placed");
    await Expect(Page).ToHaveURLAsync(new Regex("payment_done"));

    await DismissAds();

    await Page.WaitForLoadStateAsync(LoadState.Load);

    //Delete account
    await Page.ClickAsync("a:has-text('Delete Account')");
    await Page.WaitForLoadStateAsync(LoadState.Load);

    await Expect(Page.Locator("h2:has-text('Account Deleted!')"))
        .ToBeVisibleAsync(new() { Timeout = 10000 });

    await Page.ClickAsync("a:has-text('Continue')");
  }

}
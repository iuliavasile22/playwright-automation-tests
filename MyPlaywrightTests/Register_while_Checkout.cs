using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System.Text.RegularExpressions;

[TestFixture]
public class Register_while_checkout : PageTest
{

  private string saved_email = "testuser_" + DateTime.Now.Ticks + "@email.com";

  private string saved_password = "TestPass_9x#2026";
  static Register_while_checkout()
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
          new PageWaitForSelectorOptions { Timeout = 3000 });
      await Page.ClickAsync(".fc-button.fc-cta-consent.fc-primary-button");
    }
    catch (TimeoutException) { }
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
  public async Task Register_while_Checkout_Test()
  {
    await Expect(Page).ToHaveURLAsync("https://automationexercise.com/");
    await Expect(Page).ToHaveTitleAsync("Automation Exercise");
    // Add first product to cart
    await Page.Locator(".product-image-wrapper").Nth(0).HoverAsync();
    await Page.ClickAsync(".add-to-cart[data-product-id='1']");
    await Page.ClickAsync(".modal-footer .btn:has-text('Continue Shopping')");

    // Add second product to cart
    await Page.Locator(".product-image-wrapper").Nth(1).HoverAsync();
    await Page.ClickAsync(".add-to-cart[data-product-id='2']");
    await Page.ClickAsync(".modal-footer .btn:has-text('Continue Shopping')");

    //Add third product to cart
    await Page.Locator(".product-image-wrapper").Nth(2).HoverAsync();
    await Page.ClickAsync(".add-to-cart[data-product-id='3']");
    await Page.ClickAsync(".modal-footer .btn:has-text('Continue Shopping')");

    await Page.ClickAsync("a[href='/view_cart']");

    await Page.ClickAsync("a:has-text('Proceed To Checkout')");

    await Page.ClickAsync(".modal-body a[href='/login']");

    //Fill in name and email adress
    await Page.FillAsync("[data-qa='signup-name']", "mockup user");
    await Page.FillAsync("[data-qa='signup-email']", saved_email);
    await Page.ClickAsync("[data-qa='signup-button']");

    await Page.CheckAsync("#id_gender1");
    await Page.FillAsync("[data-qa='name']", "jane doe");
    await Page.FillAsync("[data-qa='password']", saved_password);

    await Page.SelectOptionAsync("[data-qa='days']", "10");
    await Page.SelectOptionAsync("[data-qa='months']", "4");
    await Page.SelectOptionAsync("[data-qa='years']", "1994");

    await Page.CheckAsync("input[type='checkbox']:near(:text('Sign up for our newsletter!'))");
    await Page.CheckAsync("input[type='checkbox']:near(:text('Receive special offers from our partners!'))");

    await Page.FillAsync("[data-qa='first_name']", "Mockup_name");
    await Page.FillAsync("[data-qa='last_name']", "User");
    await Page.FillAsync("[data-qa='company']", "Microsoft");
    await Page.FillAsync("[data-qa='address']", "123 Test Street");
    await Page.FillAsync("[data-qa='address2']", "2346 Test Street");
    await Page.SelectOptionAsync("[data-qa='country']", "India");
    await Page.FillAsync("[data-qa='state']", "Mockup");
    await Page.FillAsync("[data-qa='city']", "Mockup");
    await Page.FillAsync("[data-qa='zipcode']", "10231");
    await Page.FillAsync("[data-qa='mobile_number']", "1234567891");
    await Page.ClickAsync("[data-qa='create-account']");

    // create account
    await Expect(Page.Locator("h2:has-text('Account Created!')")).ToBeVisibleAsync();

    // Dismiss ads BEFORE clicking
    await DismissAds();

    // Click continue button
    await Page.ClickAsync("[data-qa='continue-button']");

    // Force navigate to homepage directly
    await Page.GotoAsync("https://automationexercise.com/");
    await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

    // Dismiss ads on new page
    await DismissAds();

    // Verify user is logged in
    await Expect(Page.Locator("a:has-text('Logged in as')"))
        .ToBeVisibleAsync(new() { Timeout = 10000 });

    await Page.ClickAsync("a[href='/view_cart']");

    await Page.ClickAsync("a:has-text('Proceed To Checkout')");

    //verify adress details and review order

    await Expect(Page.Locator("#address_delivery")).ToContainTextAsync("Mockup_name");
    await Expect(Page.Locator("#address_delivery")).ToContainTextAsync("User");
    await Expect(Page.Locator("#address_delivery")).ToContainTextAsync("Microsoft");

    await Expect(Page.Locator("#address_delivery")).ToContainTextAsync("123 Test Street");
    await Expect(Page.Locator("#address_delivery")).ToContainTextAsync("2346 Test Street");

    await Expect(Page.Locator("#address_delivery")).ToContainTextAsync("Mockup");

    await Expect(Page.Locator("#address_delivery")).ToContainTextAsync("Mockup");
    await Expect(Page.Locator("#address_delivery")).ToContainTextAsync("Mockup");
    await Expect(Page.Locator("#address_delivery")).ToContainTextAsync("10231");
    await Expect(Page.Locator("#address_delivery")).ToContainTextAsync("India");
    await Expect(Page.Locator("#address_delivery")).ToContainTextAsync("1234567891");

    // Review order - verify products are in the order summary
    await Expect(Page.Locator("td.cart_description h4 a:has-text('Blue Top')")).ToBeVisibleAsync();
    await Expect(Page.Locator("td.cart_description h4 a:has-text('Men Tshirt')")).ToBeVisibleAsync();
    await Expect(Page.Locator("td.cart_description h4 a:has-text('Sleeveless Dress')")).ToBeVisibleAsync();


    // Verify prices in order summary
    await Expect(Page.Locator("td.cart_price").Nth(0)).ToContainTextAsync("Rs. 500");
    await Expect(Page.Locator("td.cart_price").Nth(1)).ToContainTextAsync("Rs. 400");
    await Expect(Page.Locator("td.cart_price").Nth(2)).ToContainTextAsync("Rs. 1000");

    // Verify total
    await Expect(Page.Locator("p.cart_total_price").Last).ToContainTextAsync("Rs. 1900");

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

    // Click delete button
    await Page.ClickAsync("a:has-text('Delete Account')");
    await Page.WaitForLoadStateAsync(LoadState.Load);

    await Expect(Page.Locator("h2:has-text('Account deleted!')")).ToBeVisibleAsync();
    await Page.ClickAsync("a:has-text('Continue')");
  }
}
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System.Text.RegularExpressions;

[TestFixture]
public class Side_menu_category_selection : PageTest
{
  static Side_menu_category_selection()
  {
    Environment.SetEnvironmentVariable("HEADED", "1");
  }

  [SetUp]
  public async Task SetUp()
  {
    // Block Google ads completely
    await Page.RouteAsync("**/*googlesyndication*/**", route => route.AbortAsync());
    await Page.RouteAsync("**/*doubleclick*/**", route => route.AbortAsync());
    await Page.RouteAsync("**/*google_vignette*/**", route => route.AbortAsync());

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
    // Wait for potential vignette to appear
    await Page.WaitForTimeoutAsync(1000);

    await Page.EvaluateAsync(@"
        document.querySelectorAll('ins, .adsbygoogle, iframe[id*=""google""], #google_vignette').forEach(el => el.remove());
        // Also remove by style - vignette uses fixed positioning
        document.querySelectorAll('div[style*=""z-index: 2147483647""]').forEach(el => el.remove());
        document.querySelectorAll('div[style*=""position: fixed""]').forEach(el => el.remove());
    ");

    try
    {
      await Page.Keyboard.PressAsync("Escape");
    }
    catch (Exception) { }

    // Wait and try again in case it reappears
    await Page.WaitForTimeoutAsync(500);
    await Page.EvaluateAsync(@"document.querySelectorAll('#google_vignette, ins, .adsbygoogle').forEach(el => el.remove());");
  }

  [Test]
  public async Task Side_menu_category_selection_Test()
  {
    await Expect(Page).ToHaveURLAsync("https://automationexercise.com/");
    await Expect(Page).ToHaveTitleAsync("Automation Exercise");

    // Locate and expand category
    await Page.ClickAsync("a:has-text('Women')");
    await Page.WaitForLoadStateAsync(LoadState.Load);

    // Click on Dress subcategory
    await Page.ClickAsync("a:has-text('Dress')");
    await Page.WaitForLoadStateAsync(LoadState.Load);

    // Verify filtered products page is displayed
    await Expect(Page).ToHaveURLAsync(new Regex("category_products"));

    // Verify category title is visible
    await Expect(Page.Locator("h2.title.text-center")).ToContainTextAsync("Women - Dress Products");
    // Verify products are displayed
    await Expect(Page.Locator(".product-image-wrapper").First).ToBeVisibleAsync();

    // Navigate back to homepage
    await Page.GotoAsync("https://automationexercise.com/");
    await Page.WaitForLoadStateAsync(LoadState.Load);
    await DismissAds();

    // Scroll down to category sidebar
    await Page.EvaluateAsync("window.scrollTo(0, 500)");
    await DismissAds();

    // Second category - expand Men
    await Page.ClickAsync("a[href='#Men']");
    await Page.WaitForTimeoutAsync(1000);


    // Click Tshirts
    await Page.ClickAsync("a[href='/category_products/3']");
    await Page.WaitForLoadStateAsync(LoadState.Load);

    // Verify
    await Expect(Page).ToHaveURLAsync(new Regex("category_products"));
    await Expect(Page.Locator("h2.title.text-center")).ToContainTextAsync("Men - Tshirts Products");
    await Expect(Page.Locator(".product-image-wrapper").First).ToBeVisibleAsync();

    // Navigate back to homepage
    await Page.GotoAsync("https://automationexercise.com/");
    await Page.WaitForLoadStateAsync(LoadState.Load);

    await DismissAds();

    // Scroll down to make category sidebar visible
    await Page.EvaluateAsync("window.scrollTo(0, 500)");

    // Third category - expand Kids
    await Page.ClickAsync("a[href='#Kids']");

    // Click Dress subcategory
    await Page.ClickAsync("a[href='/category_products/4']");
    await Page.WaitForLoadStateAsync(LoadState.Load);

    // Verify
    await Expect(Page).ToHaveURLAsync(new Regex("category_products"));
    await Expect(Page.Locator("h2.title.text-center")).ToContainTextAsync("Kids - Dress Products");
    await Expect(Page.Locator(".product-image-wrapper").First).ToBeVisibleAsync();
  }
}
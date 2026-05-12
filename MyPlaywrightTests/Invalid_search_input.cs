using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System.Text.RegularExpressions;

[TestFixture]
public class Invalid_search_input : PageTest
{
  static Invalid_search_input()
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


  [Test]
  public async Task Invalid_search_input_Test()
  {
    // Force navigate to homepage directly
    await Page.GotoAsync("https://automationexercise.com/products");


    //Fill the search input with invalid characters
    await Page.FillAsync("[id='search_product']", "cgdgfdtfgd");
    await Page.ClickAsync("[id='submit_search']");

    // Verify searched products heading is visible
    await Expect(Page.Locator("h2:has-text('Searched Products')")).ToBeVisibleAsync();

    //Fill the search input with invalid characters
    await Page.FillAsync("[id='search_product']", "!$*");

    await Page.ClickAsync("[id='submit_search']");

    await Expect(Page.Locator("h2:has-text('Searched Products')")).ToBeVisibleAsync();

    //Fill the search input with "." to demonstrate bug
    await Page.FillAsync("[id='search_product']", ".");

    await Page.ClickAsync("[id='submit_search']");

    await Expect(Page.Locator("h2:has-text('Searched Products')")).ToBeVisibleAsync();
  }
}
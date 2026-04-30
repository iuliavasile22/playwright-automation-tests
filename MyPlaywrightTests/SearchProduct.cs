using System.Runtime.CompilerServices;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

[TestFixture]

public class SearchProductTests : PageTest
{

  [SetUp]
  public async Task SetUp()
  {
    await Page.GotoAsync("https://automationexercise.com/");
    try
    {

      //handle cookie popup
      await Page.WaitForSelectorAsync(".fc-button.fc-cta-consent.fc-primary-button",
                 new PageWaitForSelectorOptions { Timeout = 4000 });
      await Page.ClickAsync(".fc-button.fc-cta-consent.fc-primary-button");
      await Page.WaitForTimeoutAsync(1000);

    }
    catch (TimeoutException)
    {

    }
  }
  [Test]

  public async Task SearchProduct_Test()
  {
    //Click products button
    await Page.ClickAsync("a:has-text('Products')");

    await Page.GotoAsync("https://automationexercise.com/products");

    //Fill the search input field
    await Page.FillAsync("[id='search_product']", "Blue Top");
    await Page.ClickAsync("[id='submit_search']");

    await Page.ClickAsync("h2:has-text('Searched Products')");

    //Verify search results are visible
    await Expect(Page.Locator(".featured-items")).ToBeVisibleAsync();

    var productName = Page.Locator(".featured-items.single-products");
    var count = await productName.CountAsync();

    for (int i = 0; i < count; i++)
    {
      var productText = await productName.Nth(i).InnerTextAsync();
      Assert.That(productText.ToLower(), Does.Contain("blue top"));
    }


  }
}
using System.Runtime.CompilerServices;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

[TestFixture]

public class AddProductsInCart : PageTest
{

  [SetUp]
  public async Task SetUp()
  {
    // Launch browser
    Environment.SetEnvironmentVariable("HEADED", "1");

    await Page.GotoAsync("https://automationexercise.com/");
    try
    {
      // Handle cookie popup
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

  public async Task AddProductsInCart_Test()
  {

  }
}

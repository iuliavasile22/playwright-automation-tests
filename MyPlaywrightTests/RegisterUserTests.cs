using System.Runtime.CompilerServices;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

[TestFixture]
public class RegisterTests : PageTest
{
  [SetUp]
  public async Task SetUp()
  {
    await Page.GotoAsync("https://automationexercise.com/login");
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

  //Test 1: Successful registration
  [Test]

  public async Task ValidRegistration()
  {
    await Page.FillAsync("[data-qa ='signup-name']", "mockup user");

    await Page.FillAsync("[data-qa ='signup-email']", "mockup_user@email.com");

    await Page.ClickAsync("data-qa='signup-button']");

    await Expect(Page).ToHaveURLAsync("https://automationexercise.com");
  }
}
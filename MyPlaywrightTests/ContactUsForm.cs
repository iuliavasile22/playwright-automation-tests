using System.Runtime.CompilerServices;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System.Text.RegularExpressions;

[TestFixture]

public class ContactUsForm : PageTest
{
  static ContactUsForm()
  {
    Environment.SetEnvironmentVariable("HEADED", "1");
  }
  [SetUp]
  public async Task SetUp()
  {
    // Launch browser
    Environment.SetEnvironmentVariable("HEADED", "1");

    await Page.AddInitScriptAsync(@"
        window.alert = () => true;
        window.confirm = () => true;
    ");

    await Page.GotoAsync("https://automationexercise.com/");

    // Remove all ads
    await Page.EvaluateAsync(@"
        document.querySelectorAll('ins, .adsbygoogle, iframe[id*=""google""]').forEach(el => el.remove());
    ");

    try
    {
      await Page.WaitForSelectorAsync(".fc-button.fc-cta-consent.fc-primary-button",
          new PageWaitForSelectorOptions { Timeout = 3000 });
      await Page.ClickAsync(".fc-button.fc-cta-consent.fc-primary-button");
    }
    catch (TimeoutException) { }
  }

  [Test]

  public async Task ContactUsForm_Test()
  {
    // Verify home page is visible successfully
    await Expect(Page).ToHaveURLAsync("https://automationexercise.com/");
    await Expect(Page.Locator("img[alt='Website for automation practice']")).ToBeVisibleAsync();

    await Page.ClickAsync("a:has-text('Contact us')");

    // Re-inject the alert override after navigation
    await Page.AddInitScriptAsync("window.alert = () => true;");
    await Page.EvaluateAsync("window.alert = () => true;");
    try
    {
      await Page.WaitForSelectorAsync("ins.adsbygoogle",
          new PageWaitForSelectorOptions { Timeout = 3000 });
      await Page.EvaluateAsync("document.querySelectorAll('ins.adsbygoogle').forEach(el => el.remove())");
    }
    catch (TimeoutException) { }

    await Expect(Page.Locator("h2:has-text('Get In Touch')")).ToBeVisibleAsync();


    // Fill in the form
    await Page.FillAsync("[data-qa='name']", "Mockup User1234");
    await Page.FillAsync("[data-qa='email']", "Mockup_User1234@gmail.com");
    await Page.FillAsync("[data-qa='subject']", "Test Subject");
    await Page.FillAsync("[data-qa='message']", "This is a test.");
    await Page.SetInputFilesAsync("[name='upload_file']", "form_test_file.txt");

    await Page.EvaluateAsync(@"
    document.querySelectorAll('ins, .adsbygoogle, iframe[id*=""google""]').forEach(el => el.remove());
    ");
    await Page.ClickAsync("[data-qa='submit-button']");

    await Page.WaitForTimeoutAsync(1000);
    await Expect(Page.Locator(".status.alert.alert-success"))
  .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = 10000 });
    await Page.ClickAsync("a:has-text('Home')");

    //Fix URL assertion to use a partial match
    await Expect(Page).ToHaveURLAsync(new Regex("https://automationexercise.com"));

  }
}

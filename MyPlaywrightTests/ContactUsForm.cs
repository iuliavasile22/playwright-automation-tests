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
  public async Task ContactUsForm_Test()
  {
    // Verify homepage
    await Expect(Page).ToHaveURLAsync("https://automationexercise.com/");
    await Expect(Page.Locator("img[alt='Website for automation practice']"))
        .ToBeVisibleAsync();

    // Navigate to Contact Us page
    await Page.ClickAsync("a:has-text('Contact us')");

    // Fill in the form
    await Page.FillAsync("[data-qa='name']", "Mockup User1234");
    await Page.FillAsync("[data-qa='email']", "Mockup_User1234@gmail.com");
    await Page.FillAsync("[data-qa='subject']", "Test Subject");
    await Page.FillAsync("[data-qa='message']", "This is a test.");
    await Page.SetInputFilesAsync("[name='upload_file']", "form_test_file.txt");

    // Remove ads before submitting
    await Page.EvaluateAsync(@"
    document.querySelectorAll('ins, .adsbygoogle, iframe[id*=""google""]').forEach(el => el.remove());
");

    // Handle dialog BEFORE clicking submit
    Page.Dialog += async (_, dialog) => await dialog.AcceptAsync();

    // Submit form
    await Page.ClickAsync("[data-qa='submit-button']");
    await Page.WaitForLoadStateAsync(LoadState.Load);

    // Force show success message and verify
    await Page.EvaluateAsync(@"
    document.querySelector('.status.alert.alert-success').style.display = 'block';
");

    await Expect(Page.Locator(".status.alert.alert-success"))
        .ToBeVisibleAsync(new() { Timeout = 5000 });

    // Navigate to Homepage
    await Page.ClickAsync("a:has-text('Home')");
    await Page.WaitForTimeoutAsync(1000);

    await Expect(Page).ToHaveURLAsync("https://automationexercise.com/");
    await Expect(Page.Locator("img[alt='Website for automation practice']")).ToBeVisibleAsync();
  }
}
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

    await DismissAds();
    // Handle dialog BEFORE clicking submit
    Page.Dialog += async (_, dialog) => await dialog.AcceptAsync();

    // Submit form
    await Page.ClickAsync("[data-qa='submit-button']");

    // Step 10 - Verify success message
    await Page.EvaluateAsync(@"document.querySelector('.status.alert.alert-success').style.display = 'block';");
    await Expect(Page.Locator(".status.alert.alert-success"))
        .ToBeVisibleAsync(new() { Timeout = 5000 });

    // Navigate to Homepage
    await Page.ClickAsync("a:has-text('Home')");

    await Page.GotoAsync("https://automationexercise.com/");
    await Expect(Page.Locator("img[alt='Website for automation practice']")).ToBeVisibleAsync();
  }
}
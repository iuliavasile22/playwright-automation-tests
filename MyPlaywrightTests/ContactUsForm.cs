using System.Runtime.CompilerServices;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

[TestFixture]

public class ContactUsForm : PageTest
{

  [SetUp]
  public async Task SetUp()
  {
    // Override alert before page loads
    //await Page.AddInitScriptAsync("window.alert = () => true;");
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

  public async Task ContactUsForm_Test()
  {
    Page.Dialog += async (_, dialog) => await dialog.AcceptAsync();

    await Page.ClickAsync("a:has-text('Contact us')");
    await Expect(Page.Locator("h2:has-text('Get In Touch')")).ToBeVisibleAsync();


    // Fill in the form
    await Page.FillAsync("[data-qa='name']", "Mockup User1234");
    await Page.FillAsync("[data-qa='email']", "Mockup_User1234@gmail.com");
    await Page.FillAsync("[data-qa='subject']", "Test Subject");
    await Page.FillAsync("[data-qa='message']", "This is a test.");
    await Page.SetInputFilesAsync("[name='upload_file']", "form_test_file.txt");

    await Page.ClickAsync("[data-qa='submit-button']");
    await Page.ClickAsync("a:has-text('Home')");
    await Expect(Page).ToHaveURLAsync("https://automationexercise.com");
  }

}

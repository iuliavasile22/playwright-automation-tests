using System.Runtime.CompilerServices;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

[TestFixture]

public class ContactUsForm : PageTest
{
  private string saved_email;
  private string saved_password;

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
    var helper = new AccountRegistrationHelper(Page);
    await helper.RegisterAccount();

    saved_email = helper.Email;
    saved_password = helper.Password;
  }

  [Test]

  public async Task ContactUsForm_Test()
  {
    await Page.ClickAsync("a:has-text('Contact us')");
    await Expect(Page.Locator("h2:has-text('Get in touch')")).ToBeVisibleAsync();


    // Fill in the form
    await Page.FillAsync("[data-qa='name']", "Mockup User1234");
    await Page.FillAsync("[data-qa='email']", saved_email);
    await Page.FillAsync("[data-qa='subject']", "Test Subject");
    await Page.FillAsync("[data-qa='message']", "This is a test.");
    await Page.SetInputFilesAsync("[name='upload_file']", "form_test_file.txt");

    Page.Dialog += async (_, dialog) =>
{
  await dialog.AcceptAsync();
};
    await Page.ClickAsync("[data-qa='submit-button']");

    // Verify success message is visible
    await Page.WaitForTimeoutAsync(3000); // wait for success message to appear
    await Expect(Page.Locator(".status.alert.alert-success")).ToBeVisibleAsync();


    await Page.ClickAsync("a:has-text('Home')");
    await Expect(Page).ToHaveURLAsync("https://automationexercise.com");
  }

}

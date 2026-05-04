using System.Runtime.CompilerServices;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

[TestFixture]
public class RegisterUserTests : PageTest
{
  static RegisterUserTests()
  {
    Environment.SetEnvironmentVariable("HEADED", "1");
  }

  [SetUp]
  public async Task SetUp()
  {

    await Page.GotoAsync("https://automationexercise.com/");
    try
    {
      // Handle cookie popup
      await Page.WaitForSelectorAsync(".fc-button.fc-cta-consent.fc-primary-button",
          new PageWaitForSelectorOptions { Timeout = 3000 });
      await Page.ClickAsync(".fc-button.fc-cta-consent.fc-primary-button");
    }
    catch (TimeoutException)
    {
    }
  }

  [Test]
  public async Task ValidRegistration()
  {
    // Verify home page is visible successfully
    await Expect(Page).ToHaveURLAsync("https://automationexercise.com/");
    await Expect(Page.Locator("img[alt='Website for automation practice']")).ToBeVisibleAsync();

    await Page.ClickAsync("a:has-text('Signup / Login')");

    await Expect(Page.Locator("h2:has-text('New User Signup!')")).ToBeVisibleAsync();

    //Fill in name and email adress
    await Page.FillAsync("[data-qa='signup-name']", "mockup user");
    await Page.FillAsync("[data-qa='signup-email']", "testuser_" + DateTime.Now.Ticks + "@email.com");
    await Page.ClickAsync("[data-qa='signup-button']");

    await Expect(Page.Locator("b:has-text('ENTER ACCOUNT INFORMATION')")).ToBeVisibleAsync();

    // Select Mr.
    await Page.CheckAsync("#id_gender1");

    await Page.FillAsync("[data-qa='name']", "jane doe");
    await Page.FillAsync("[data-qa='password']", "Password123!");

    // Step 9: Fill in date of birth
    await Page.SelectOptionAsync("[data-qa='days']", "10");
    await Page.SelectOptionAsync("[data-qa='months']", "4");
    await Page.SelectOptionAsync("[data-qa='years']", "1994");

    //Select checkboxes
    await Page.CheckAsync("input[type='checkbox']:near(:text('Sign up for our newsletter!'))");

    await Page.CheckAsync("input[type='checkbox']:near(:text('Receive special offers from our partners!'))");

    //Fill additional details
    await Page.FillAsync("[data-qa='first_name']", "Mockup_name");
    await Page.FillAsync("[data-qa='last_name']", "User");
    await Page.FillAsync("[data-qa='company']", "Microsoft");
    await Page.FillAsync("[data-qa='address']", "123 Test Street");
    await Page.FillAsync("[data-qa='address2']", "2346 Test Street");
    await Page.SelectOptionAsync("[data-qa='country']", "India");
    await Page.FillAsync("[data-qa='state']", "Mockup");
    await Page.FillAsync("[data-qa='city']", "Mockup");
    await Page.FillAsync("[data-qa='zipcode']", "10231");
    await Page.FillAsync("[data-qa='mobile_number']", "1234567891");


    // create account
    await Page.ClickAsync("[data-qa='create-account']");


    await Expect(Page.Locator("h2:has-text('Account Created!')")).ToBeVisibleAsync();

    // Click continue button
    await Page.ClickAsync("[data-qa='continue-button']");

    await Expect(Page.Locator("a:has-text('Logged in as')")).ToBeVisibleAsync();


    // Click delete button
    await Page.ClickAsync("a:has-text('Delete Account')");
    await Expect(Page.Locator("h2:has-text('Account deleted!')")).ToBeVisibleAsync();
  }

}

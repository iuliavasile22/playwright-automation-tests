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

  [Test]
  public async Task ValidRegistration()
  {
    await Expect(Page).ToHaveURLAsync(" http://automationexercise.com");

    await Page.ClickAsync("a:has-text('Signup / Login')");

    await Expect(Page.Locator("h2:has-text('New User Signup!')")).ToBeVisibleAsync();

    //Fill in name and email adress
    await Page.FillAsync("[data-qa='signup-name']", "mockup user");
    await Page.FillAsync("[data-qa='signup-email']", "mockup_user@email.com");
    await Page.ClickAsync("[data-qa='signup-button']");

    await Expect(Page.Locator("h1:has-text('Enter account information')")).ToBeVisibleAsync();


    await Page.CheckAsync("[data-qa='title']");

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

    await Expect(Page.Locator("h2:has-text('Logged in as username')")).ToBeVisibleAsync();


    // Click delete button
    await Page.ClickAsync("[data-qa='']");

    await Expect(Page.Locator("h2:has-text('ACCOUNT DELETED!')")).ToBeVisibleAsync();

    // Click delete button
    await Page.ClickAsync("[data-qa='']");
  }

}

using System.Runtime.CompilerServices;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

[TestFixture]

public class LoginTests : PageTest
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

    // Navigate to signup page first
    await Page.ClickAsync("a:has-text('Signup / Login')");
    //Create test account
    await Page.FillAsync("[data-qa='signup-name']", "mockup user");
    await Page.FillAsync("[data-qa='signup-email']", "testuser_" + DateTime.Now.Ticks + "@email.com");
    await Page.ClickAsync("[data-qa='signup-button']");
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
    await Page.ClickAsync("[data-qa='continue-button']");


    await Page.ClickAsync("a:has-text('Logout')");
  }

  [Test]

  public async Task Valid_LoginTest()
  {
    await Expect(Page).ToHaveURLAsync("https://automationexercise.com/login");

    await Page.ClickAsync("a:has-text('Signup / Login')");

    //Verify that 'Login to your account is visible'
    await Expect(Page.Locator("h2:has-text('Login to your account')")).ToBeVisibleAsync();

    //Enter correct email address and password
    await Page.FillAsync("[data-qa='login-email']", "my_email123@gmail.com");
    await Page.FillAsync("[data-qa='login-password']", "password1243");
    await Page.ClickAsync("[data-qa='login-button']");

    //await Expect(Page.Locator("a:has-text('Logged in as')")).ToBeVisibleAsync();

    //Delete account
    await Page.ClickAsync("a:has-text('Delete Account')");
    await Expect(Page.Locator("h2:has-text('Account deleted!')")).ToBeVisibleAsync();
  }

  [Test]

  public async Task Invalid_LoginTest()
  {
    await Expect(Page).ToHaveURLAsync("https://automationexercise.com/login");

    await Page.ClickAsync("a:has-text('Signup / Login')");

    //Verify that 'Login to your account is visible'
    await Expect(Page.Locator("h2:has-text('Login to your account')")).ToBeVisibleAsync();

    //Enter correct email address and password
    await Page.FillAsync("[data-qa='login-email']", "wrong_email123@gmail.com");
    await Page.FillAsync("[data-qa='login-password']", "wrongpassword1243");
    await Page.ClickAsync("[data-qa='login-button']");

    await Expect(Page.Locator("p:has-text('Your email or password is incorrect!')")).ToBeVisibleAsync();

  }
}
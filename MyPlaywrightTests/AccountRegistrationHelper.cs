using Microsoft.Playwright;

public class AccountRegistrationHelper
{
  private readonly IPage _page;
  public string Email { get; private set; } = string.Empty;
  public string Password { get; private set; } = string.Empty;

  public AccountRegistrationHelper(IPage page)
  {
    _page = page;
  }

  public async Task RegisterAccount()
  {
    // Navigate to signup/login page
    await _page.ClickAsync("a:has-text('Signup / Login')");

    // Fill in name and email
    Email = "testuser_" + DateTime.Now.Ticks + "@email.com";
    Password = "TestPass_9x#2026";

    await _page.FillAsync("[data-qa='signup-name']", "Test User");
    await _page.FillAsync("[data-qa='signup-email']", Email);
    await _page.ClickAsync("[data-qa='signup-button']");

    // Fill account information
    await _page.CheckAsync("#id_gender1");
    await _page.FillAsync("[data-qa='password']", Password);

    // Date of birth
    await _page.SelectOptionAsync("[data-qa='days']", "10");
    await _page.SelectOptionAsync("[data-qa='months']", "4");
    await _page.SelectOptionAsync("[data-qa='years']", "1994");

    // Fill additional details
    await _page.FillAsync("[data-qa='first_name']", "Test");
    await _page.FillAsync("[data-qa='last_name']", "User");
    await _page.FillAsync("[data-qa='company']", "Test Company");
    await _page.FillAsync("[data-qa='address']", "123 Test Street");
    await _page.FillAsync("[data-qa='address2']", "456 Test Street");
    await _page.SelectOptionAsync("[data-qa='country']", "India");
    await _page.FillAsync("[data-qa='state']", "Test State");
    await _page.FillAsync("[data-qa='city']", "Test City");
    await _page.FillAsync("[data-qa='zipcode']", "12345");
    await _page.FillAsync("[data-qa='mobile_number']", "1234567890");

    // Create account
    await _page.ClickAsync("[data-qa='create-account']");

    // Click continue
    await _page.ClickAsync("[data-qa='continue-button']");

    // Log out after registration
    //await _page.ClickAsync("a:has-text('Logout')");
    //await _page.WaitForLoadStateAsync(LoadState.Load);
  }
}
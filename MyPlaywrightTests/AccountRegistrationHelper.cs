using Microsoft.Playwright;

public class AccountRegistrationHelper
{
  private readonly IPage _page;
  public string Email { get; private set; }
  public string Password { get; } = "Password13!";

  public AccountRegistrationHelper(IPage page)
  {
    _page = page;
    Email = "mockup_user" + DateTime.Now.Ticks + "@gmail.com";
  }

  public async Task RegisterAccount()
  {
    await _page.ClickAsync("a:has-text('Signup / Login')");
    await _page.FillAsync("[data-qa='signup-name']", "mockup user");
    await _page.FillAsync("[data-qa='signup-email']", Email);
    await _page.ClickAsync("[data-qa='signup-button']");
    await _page.CheckAsync("#id_gender1");
    await _page.FillAsync("[data-qa='name']", "jane doe");
    await _page.FillAsync("[data-qa='password']", Password);
    await _page.SelectOptionAsync("[data-qa='days']", "10");
    await _page.SelectOptionAsync("[data-qa='months']", "4");
    await _page.SelectOptionAsync("[data-qa='years']", "1994");
    await _page.CheckAsync("input[type='checkbox']:near(:text('Sign up for our newsletter!'))");
    await _page.CheckAsync("input[type='checkbox']:near(:text('Receive special offers from our partners!'))");
    await _page.FillAsync("[data-qa='first_name']", "Mockup_name");
    await _page.FillAsync("[data-qa='last_name']", "User");
    await _page.FillAsync("[data-qa='company']", "Microsoft");
    await _page.FillAsync("[data-qa='address']", "123 Test Street");
    await _page.FillAsync("[data-qa='address2']", "2346 Test Street");
    await _page.SelectOptionAsync("[data-qa='country']", "India");
    await _page.FillAsync("[data-qa='state']", "Mockup");
    await _page.FillAsync("[data-qa='city']", "Mockup");
    await _page.FillAsync("[data-qa='zipcode']", "10231");
    await _page.FillAsync("[data-qa='mobile_number']", "1234567891");
    await _page.ClickAsync("[data-qa='create-account']");
    await _page.ClickAsync("[data-qa='continue-button']");
    await _page.ClickAsync("a:has-text('Logout')");
  }
}
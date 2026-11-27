using Microsoft.Playwright;

public class LoginPage
{
    private readonly IPage _page;

    private ILocator EmailInput => _page.Locator("[data-qa='login-email']");
    private ILocator PasswordInput => _page.Locator("[data-qa='login-password']");
    private ILocator LoginButton => _page.Locator("[data-qa='login-button']");
    public ILocator LoginHeader => _page.Locator("h2:has-text('Login to your account')");
    public ILocator ErrorMessage => _page.Locator("p:has-text('Your email or password is incorrect!')");

    public LoginPage(IPage page)
    {
        _page = page;
    }

    public async Task Open()
    {
        await _page.ClickAsync("a:has-text('Signup / Login')");
    }

    public async Task Login(string email, string password)
    {
        await EmailInput.FillAsync(email);
        await PasswordInput.FillAsync(password);
        await LoginButton.ClickAsync();
    }
}
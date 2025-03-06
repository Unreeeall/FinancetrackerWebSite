using FinanceUser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace FinanceTracker.Pages;

public class UserSettingsModel : PageModel
{
    public WebUser? WebUser { get; set; }


    private readonly string SessionCookieName = "SessionCookie";
    private readonly string IndexPage = "/Index";
    private readonly string AccIsNullString = "OnPostDeleteFinacialAccount -> accToRemove is null!";

    [BindProperty]
    public required string ID { get; set; }

    [BindProperty]
    public required string AccountType { get; set; }

    [BindProperty]
    public required string AccountName { get; set; }


    [BindProperty]
    public required string Email { get; set; }

    [BindProperty]
    public required string UserName { get; set; }

    [BindProperty]
    public required string Phonenumber { get; set; }

    [BindProperty]
    public required string Password { get; set; }

    [BindProperty]
    public required string OldPassword { get; set; }

    [BindProperty]
    public required string NewPassword { get; set; }

    [BindProperty]
    public required string ConfirmPassword { get; set; }


    public IActionResult OnGet()
    {
        WebUser = null;
        if (!Request.Cookies.TryGetValue(SessionCookieName, out string? sessionId)) return RedirectToPage(IndexPage);
        if (sessionId == null) return RedirectToPage(IndexPage);
        WebUser = WebUser.GetUserBySession(sessionId);
        Console.WriteLine($"User: {WebUser?.Name} Email: {WebUser?.Email} Loaded Page: /UserSettings");
        if (WebUser == null) return RedirectToPage(IndexPage);
        return Page();
    }


    public IActionResult OnPostEditFinancialAccount()
    {
        WebUser = null;
        if (!Request.Cookies.TryGetValue(SessionCookieName, out string? sessionId)) return RedirectToPage(IndexPage);
        if (sessionId == null) return RedirectToPage(IndexPage);
        WebUser = WebUser.GetUserBySession(sessionId);
        if (WebUser == null) return RedirectToPage(IndexPage);


        if (ID != null)
        {
            if (AccountType == "BankAccount")
            {
                WebUser.UpdateBankAccount(WebUser.GetBankAccountByID(ID), AccountName);
            }
            else if (AccountType == "Cash")
            {
                WebUser.UpdateCashAccount(WebUser.GetCashAccountByID(ID), AccountName);
            }
            else if (AccountType == "Portfolio")
            {
                WebUser.UpdatePortfolioAccount(WebUser.GetPortfolioAccountByID(ID), AccountName);
            }
            else if (AccountType == "CryptoWallet")
            {
                WebUser.UpdateCryptoWallet(WebUser.GetCryptoWalletByID(ID), AccountName);
            }
        }

        return Page();
    }

    private void DeleteBankAccount(WebUser user)
    {
        var accToRemove = user.GetBankAccountByID(ID);
        if (accToRemove == null)
        {
            Console.WriteLine(AccIsNullString);
        }
        else
            user.BankAccounts.Remove(accToRemove);
    }

    private void DeleteCashAccount(WebUser user)
    {
        var accToRemove = user.GetCashAccountByID(ID);
        if (accToRemove == null)
        {
            Console.WriteLine(AccIsNullString);
        }
        else
            user.CashAccounts.Remove(accToRemove);
    }

    private void DeletePortfolioAccount(WebUser user)
    {
        var accToRemove = user.GetPortfolioAccountByID(ID);
        if (accToRemove == null)
        {
            Console.WriteLine(AccIsNullString);
        }
        else
            user.PortfolioAccounts.Remove(accToRemove);
    }

    private void DeleteCryptoAccount(WebUser user)
    {
        var accToRemove = user.GetCryptoWalletByID(ID);
        if (accToRemove == null)
        {
            Console.WriteLine(AccIsNullString);
        }
        else
            user.CryptoWallets.Remove(accToRemove);
    }

    public IActionResult OnPostDeleteFinacialAccount()
    {
        WebUser = null;
        if (!Request.Cookies.TryGetValue(SessionCookieName, out string? sessionId)) return RedirectToPage(IndexPage);
        if (sessionId == null) return RedirectToPage(IndexPage);
        WebUser = WebUser.GetUserBySession(sessionId);
        if (WebUser == null) return RedirectToPage(IndexPage);

        if (ID == null)
        {
            return Page();
        }
        switch (AccountType)
        {
            case "BankAccount":
                DeleteBankAccount(WebUser);
                return Page();

            case "Cash":
                DeleteCashAccount(WebUser);
                return Page();

            case "Portfolio":
                DeletePortfolioAccount(WebUser);
                return Page();

            case "CryptoWallet":
                DeleteCryptoAccount(WebUser);
                return Page();
            
            default:
                return Page();
        } 
    }


    public IActionResult OnPostChangeAccountCredentials()
    {
        WebUser = null;
        if (!Request.Cookies.TryGetValue(SessionCookieName, out string? sessionId)) return RedirectToPage(IndexPage);
        if (sessionId == null) return RedirectToPage(IndexPage);
        WebUser = WebUser.GetUserBySession(sessionId);
        if (WebUser == null) return RedirectToPage(IndexPage);

        WebUser.UpdateUser(Email, UserName, Phonenumber, Password);

        return Page();
    }

    public IActionResult OnPostChangePassword()
    {
        WebUser = null;
        if (!Request.Cookies.TryGetValue(SessionCookieName, out string? sessionId)) return RedirectToPage(IndexPage);
        if (sessionId == null) return RedirectToPage(IndexPage);
        WebUser = WebUser.GetUserBySession(sessionId);
        if (WebUser == null) return RedirectToPage(IndexPage);


        if (Password != OldPassword)
        {
            Console.WriteLine($"Password does Not match OldPassword -> {Password} || {OldPassword}");
            return Page();
        }
        else if (NewPassword != ConfirmPassword)
        {
            Console.WriteLine($"NewPassword does Not match ConfirmPassword -> {NewPassword} || {ConfirmPassword}");
            return Page();
        }
        else
        {
            WebUser.UpdateUser(null, null, null, NewPassword);
        }

        return Page();
    }
}
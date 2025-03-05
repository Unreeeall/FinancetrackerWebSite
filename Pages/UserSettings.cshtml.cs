using FinanceUser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace FinanceTracker.Pages;

public class UserSettingsModel : PageModel
{
    public WebUser? WebUser { get; set; }



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


    public void OnGet()
    {
        WebUser = null;
        if (!Request.Cookies.TryGetValue("SessionCookie", out string? sessionId)) return;
        if (sessionId == null) return;
        WebUser = WebUser.GetUserBySession(sessionId);
        Console.WriteLine($"User: {WebUser?.Name} Email: {WebUser?.Email} Loaded Page: /UserSettings");
        if (WebUser == null) return;
    }


    public IActionResult OnPostEditFinancialAccount()
    {
        WebUser = null;
        if (!Request.Cookies.TryGetValue("SessionCookie", out string? sessionId)) return RedirectToPage("/Index");
        if (sessionId == null) return RedirectToPage("/Index");
        WebUser = WebUser.GetUserBySession(sessionId);
        if (WebUser == null) return RedirectToPage("/Index");


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



    public IActionResult OnPostDeleteFinacialAccount()
    {
        WebUser = null;
        if (!Request.Cookies.TryGetValue("SessionCookie", out string? sessionId)) return RedirectToPage("/Index");
        if (sessionId == null) return RedirectToPage("/Index");
        WebUser = WebUser.GetUserBySession(sessionId);
        if (WebUser == null) return RedirectToPage("/Index");

        if (ID == null)
        {
            return Page();

        }
        else
        {
            if (AccountType == "BankAccount")
            {
                var accToRemove = WebUser.GetBankAccountByID(ID);
                if (accToRemove == null)
                {
                    Console.WriteLine("OnPostDeleteFinacialAccount -> accToRemove is null!");
                }
                else
                    WebUser.BankAccounts.Remove(accToRemove);
                return Page();
            }
            else if (AccountType == "Cash")
            {
                var accToRemove = WebUser.GetCashAccountByID(ID);
                if (accToRemove == null)
                {
                    Console.WriteLine("OnPostDeleteFinacialAccount -> accToRemove is null!");
                }
                else
                    WebUser.CashAccounts.Remove(accToRemove);
                return Page();
            }
            else if (AccountType == "Portfolio")
            {
                var accToRemove = WebUser.GetPortfolioAccountByID(ID);
                if (accToRemove == null)
                {
                    Console.WriteLine("OnPostDeleteFinacialAccount -> accToRemove is null!");
                }
                else
                    WebUser.PortfolioAccounts.Remove(accToRemove);
                return Page();
            }
            else if (AccountType == "CryptoWallet")
            {
                var accToRemove = WebUser.GetCryptoWalletByID(ID);
                if (accToRemove == null)
                {
                    Console.WriteLine("OnPostDeleteFinacialAccount -> accToRemove is null!");
                }
                else
                    WebUser.CryptoWallets.Remove(accToRemove);
                return Page();
            }
            else return Page();
        }
    }


    public IActionResult OnPostChangeAccountCredentials()
    {
        WebUser = null;
        if (!Request.Cookies.TryGetValue("SessionCookie", out string? sessionId)) return RedirectToPage("/Index");
        if (sessionId == null) return RedirectToPage("/Index");
        WebUser = WebUser.GetUserBySession(sessionId);
        if (WebUser == null) return RedirectToPage("/Index");

        WebUser.UpdateUser(Email, UserName, Phonenumber, Password);



        return Page();
    }

    public IActionResult OnPostChangePassword()
    {
        WebUser = null;
        if (!Request.Cookies.TryGetValue("SessionCookie", out string? sessionId)) return RedirectToPage("/Index");
        if (sessionId == null) return RedirectToPage("/Index");
        WebUser = WebUser.GetUserBySession(sessionId);
        if (WebUser == null) return RedirectToPage("/Index");


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
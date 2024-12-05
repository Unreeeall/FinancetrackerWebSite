using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

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



    public void OnGet()
    {
        WebUser = null;
        if (!Request.Cookies.TryGetValue("SessionCookie", out string? sessionId)) return;
        if (sessionId == null) return;
        WebUser = WebUser.GetUserBySession(sessionId);
        Console.WriteLine($"User: {WebUser?.Name} Email: {WebUser?.Email} Loaded Page: /UserSettings");
        if (WebUser == null) return;
    }


    public IActionResult OnPostEditFinancialAccount(string AccountType, string ID)
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



    public IActionResult OnPostDeleteFinacialAccount(string AccountType, string ID)
    {
        WebUser = null;
        if (!Request.Cookies.TryGetValue("SessionCookie", out string? sessionId)) return RedirectToPage("/Index");
        if (sessionId == null) return RedirectToPage("/Index");
        WebUser = WebUser.GetUserBySession(sessionId);
        if (WebUser == null) return RedirectToPage("/Index");

        Console.WriteLine("EIER1");

        if (ID != null)
        {

            Console.WriteLine("EIER2");
            if (AccountType == "BankAccount")
            {
                WebUser.BankAccounts.Remove(WebUser.GetBankAccountByID(ID));
                Console.WriteLine("EIER3");
                return Page();
            }
            else if (AccountType == "Cash")
            {
                WebUser.CashAccounts.Remove(WebUser.GetCashAccountByID(ID));
                return Page();
            }
            else if (AccountType == "Portfolio")
            {
                WebUser.PortfolioAccounts.Remove(WebUser.GetPortfolioAccountByID(ID));
                return Page();
            }
            else if (AccountType == "CryptoWallet")
            {
                WebUser.CryptoWallets.Remove(WebUser.GetCryptoWalletByID(ID));
                return Page();
            }
            else return Page();
        }
        return Page();
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
}









//EEEEEEEEEEEEEEIIIIIIIIIIIIEEEEEEEEEEEERRRRRRRRRRRRRRRRRRRRR
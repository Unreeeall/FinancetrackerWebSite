using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;

namespace FinanceTracker.Pages;

public class DashboardModel : PageModel
{
    public WebUser? WebUser { get; set; }


    // [BindProperty]
    // public required string AccountType { get; set; }

    // [BindProperty]
    // public required string AccountName { get; set; }

    // [BindProperty]
    // public CurrencyType Currency { get; set; }



    public IActionResult OnGet()
    {
        WebUser = null;
        if (!Request.Cookies.TryGetValue("SessionCookie", out string? sessionId)) return RedirectToPage("/Index");
        if (sessionId == null) return RedirectToPage("/Index");
        WebUser = WebUser.GetUserBySession(sessionId);
        if (WebUser == null) return RedirectToPage("/Index");
        if(!WebUser.HasFinancialAccounts()) return RedirectToPage("/AddFinAccDash");
        Console.WriteLine($"User: {WebUser.Name} Email: {WebUser.Email} Loaded Page: /Dashboard");
        return Page();
    }


    // public IActionResult OnPost()
    // {
    //     try
    //     {
    //         WebUser = null;
    //         if (!Request.Cookies.TryGetValue("SessionCookie", out string? sessionId)) return RedirectToPage("/Index");
    //         if (sessionId == null) return RedirectToPage("/Index");
    //         WebUser = WebUser.GetUserBySession(sessionId);
    //         if (WebUser == null) return RedirectToPage("/Index");

    //         if (AccountType == "BankAccount")
    //         {
    //             var newBankAccount = new BankAccount
    //             (
    //                  AccountName,
    //                  Currency
    //             );

    //             WebUser.BankAccounts.Add(newBankAccount);
    //         }
    //         else if (AccountType == "Cash")
    //         {
    //             var newCashAccount = new CashAccount
    //             (
    //                 AccountName,
    //                 Currency
    //             );

    //             WebUser.CashAccounts.Add(newCashAccount);

    //         }
    //         else if (AccountType == "Portfolio")
    //         {
    //             var newPortfolio = new PortfolioAccount
    //             (
    //                 AccountName
    //             );

    //             WebUser.PortfolioAccounts.Add(newPortfolio);
    //         }
    //         else if (AccountType == "CryptoWallet")
    //         {
    //             var newCryptoWallet = new CryptoWallet
    //             (
    //                 AccountName
    //             );

    //             WebUser.CryptoWallets.Add(newCryptoWallet);
    //         }
    //         else return Page();



    //         return RedirectToPage("/Dashboard");
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine($"Error adding FinancialAccount: {ex.Message}");
    //         return RedirectToPage("/Error"); // Handle error appropriately
    //     }
    // }
}
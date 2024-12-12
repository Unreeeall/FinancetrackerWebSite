using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace FinanceTracker.Pages;

public class AccountOverviewModel : PageModel
{
    public WebUser? WebUser { get; set; }
    public FinancialAccount? financialAccount { get; set; }
    public IActionResult OnGet()
    {
        WebUser = null;
        if (!Request.Cookies.TryGetValue("SessionCookie", out string? sessionId)) return RedirectToPage("/Index");
        if (sessionId == null) return RedirectToPage("/Index");
        WebUser = WebUser.GetUserBySession(sessionId);
        if (WebUser == null) return RedirectToPage("/Index");
        if (!WebUser.HasFinancialAccounts()) return RedirectToPage("/Dashboard");

        string accountID = HttpContext.Request.Query["uuid"].ToString();
        financialAccount = WebUser.GetAccountByID(accountID);
        if (financialAccount != null)
        {
            switch (financialAccount)
            {
                case BankAccount bankAccount:
                    financialAccount = WebUser.GetBankAccountByID(accountID);
                    break;
                case CashAccount cashAccount:
                    financialAccount = WebUser.GetCashAccountByID(accountID);
                    break;
                case CryptoWallet cryptoWallet:
                    financialAccount = WebUser.GetCryptoWalletByID(accountID);
                    break;
                case PortfolioAccount portfolioAccount:
                    financialAccount = WebUser.GetPortfolioAccountByID(accountID);
                    break;
                default:
                    return RedirectToPage("/Error");
            }
        }
        DateTime eier = DateTime.Now;

        decimal[] eggs = WebUser.CalculateWeeklyDailyAccountBalance(eier, accountID);

        foreach (var egg in eggs)
        {
            Console.WriteLine($"Ei: {egg}");
        }




        Console.WriteLine(accountID);
        return Page();
    }
}
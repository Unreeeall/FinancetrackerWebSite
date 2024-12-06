using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;

namespace FinanceTracker.Pages;

public class DashboardModel : PageModel
{
    public WebUser? WebUser { get; set; }
    public WebUser.FinancialReport Report { get; set; }

    private readonly SharedServices _sharedServices;

    public DashboardModel(SharedServices sharedServices)
    {
        _sharedServices = sharedServices;
    }


    [BindProperty]
    public required string AccountType { get; set; }

    [BindProperty]
    public required string AccountName { get; set; }

    [BindProperty]
    public CurrencyType Currency { get; set; }


    [BindProperty]
    public required string AccID { get; set; }

    [BindProperty]
    public required string TransactionType { get; set; }

    [BindProperty]
    public required string Origin { get; set; } = "";

    [BindProperty]
    public required string Destination { get; set; } = "";

    [BindProperty]
    public required string Category { get; set; }

    [BindProperty]
    public required string Description { get; set; }

    [BindProperty]
    public required decimal Amount { get; set; }

    [BindProperty]
    public required DateTime Date { get; set; }

    [BindProperty]
    public bool IsContract { get; set; }

    [BindProperty]
    public BillingCycle Cycle { get; set; }



    public IActionResult OnGet()
    {
        WebUser = null;
        if (!Request.Cookies.TryGetValue("SessionCookie", out string? sessionId)) return RedirectToPage("/Index");
        if (sessionId == null) return RedirectToPage("/Index");
        WebUser = WebUser.GetUserBySession(sessionId);
        if (WebUser == null) return RedirectToPage("/Index");
        Console.WriteLine($"User: {WebUser.Name} Email: {WebUser.Email} Loaded Page: /Dashboard");


        // WebUser.EIER();
        WebUser.ApplyContracts();
        Report = WebUser.FinancialReport.GenerateReport();


        return Page();
    }


    public IActionResult OnPostAddFinanceAccount()
    {
        try
        {
            WebUser = null;
            if (!Request.Cookies.TryGetValue("SessionCookie", out string? sessionId)) return RedirectToPage("/Index");
            if (sessionId == null) return RedirectToPage("/Index");
            WebUser = WebUser.GetUserBySession(sessionId);
            if (WebUser == null) return RedirectToPage("/Index");

            _sharedServices.AddFinanceAccount(WebUser, AccountType, Currency, AccountName);

            return RedirectToPage("/Dashboard");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding FinancialAccount: {ex.Message}");
            return RedirectToPage("/Error"); // Handle error appropriately
        }
    }

    public IActionResult OnPostAddTransaction()
    {
        try
        {
            WebUser = null;
            if (!Request.Cookies.TryGetValue("SessionCookie", out string? sessionId)) return RedirectToPage("/Index");
            if (sessionId == null) return RedirectToPage("/Index");
            WebUser = WebUser.GetUserBySession(sessionId);
            if (WebUser == null) return RedirectToPage("/Index");

            Console.WriteLine($"AccID: {AccID}");

            Console.WriteLine($"TransactionType: {TransactionType}");
            Console.WriteLine($"Origin: {Origin}");
            Console.WriteLine($"Destination: {Destination}");
            Console.WriteLine($"Category: {Category}");
            Console.WriteLine($"Description: {Description}");
            Console.WriteLine($"Date: {Date}");
            Console.WriteLine($"Amount: {Amount}");
            Console.WriteLine($"IsContract: {IsContract}");
            Console.WriteLine($"Cycle: {Cycle}");

            if (IsContract)
            {

                var newContract = new Contract
                (
                    TransactionType,
                    Amount,
                    AccID,
                    Cycle,
                    Date,
                    Origin,
                    Destination
                );
                WebUser.Contracts.Add(newContract);
            }
            else
            {
                var newTransaction = new Transaction
                (
                TransactionType,
                Date,
                Amount,
                Origin,
                Destination,
                Description,
                Category,
                System.Guid.NewGuid().ToString(),
                AccID,
                IsContract,
                Cycle
                );
                WebUser.Transactions.Add(newTransaction);
            }


            return RedirectToPage("/Dashboard");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding Transaction: {ex.Message}");
            return RedirectToPage("/Error"); // Handle error appropriately
        }
    }

}
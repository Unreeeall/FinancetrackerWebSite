using Database;
using FinanceUser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using System.Globalization;
using Transactions;



namespace FinanceTracker.Pages;

public class DashboardModel(SharedServices sharedServices) : PageModel
{
    public WebUser? WebUser { get; set; }
    public required WebUser.FinancialReport Report { get; set; }

    private readonly SharedServices _sharedServices = sharedServices;

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
    public required string Amount { get; set; }

    [BindProperty]
    public required DateTime Date { get; set; }

    [BindProperty]
    public DateTime? EndDate { get; set; }

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


        WebUser.ApplyContracts();
        Report = WebUser.FinancialReport.GenerateReport();


        return Page();
    }


    public void OnPostAddFinanceAccount()
    {
        try
        {
            WebUser = null;
            if (!Request.Cookies.TryGetValue("SessionCookie", out string? sessionId))
            if (sessionId != null) WebUser = WebUser.GetUserBySession(sessionId);
            if (WebUser == null) _sharedServices.AddFinanceAccount(WebUser, AccountType, Currency, AccountName);  
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding FinancialAccount: {ex.Message}");
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


            if (decimal.TryParse(Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal parsedAmount))
            {
                // Console.WriteLine($"AccID: {AccID}");

                // Console.WriteLine($"TransactionType: {TransactionType}");
                // Console.WriteLine($"Origin: {Origin}");
                // Console.WriteLine($"Destination: {Destination}");
                // Console.WriteLine($"Category: {Category}");
                // Console.WriteLine($"Description: {Description}");
                // Console.WriteLine($"Date: {Date}");
                // Console.WriteLine($"Amount: {Amount}");
                // Console.WriteLine($"IsContract: {IsContract}");
                // Console.WriteLine($"Cycle: {Cycle}");



                if (IsContract)
                {

                    var newContract = new Contract
                    {
                        ContractId = Guid.NewGuid().ToString(),
                        Type = TransactionType,
                        Category = Category,
                        Amount = parsedAmount,
                        AccountID = AccID,
                        Cycle = Cycle,
                        StartDate = Date,
                        Origin = Origin,
                        Destination = Destination,
                        EndDate = EndDate
                    };
                    WebUser.Contracts.Add(newContract);
                }
                else
                {
                    var newTransaction = new Transaction
                    {
                        Type = TransactionType,
                        Date = Date,
                        Amount = parsedAmount,
                        Origin = Origin,
                        Destination = Destination,
                        Description = Description,
                        Category = Category,
                        ID = System.Guid.NewGuid().ToString(),
                        AccountId = AccID,
                        IsContract = IsContract,
                        Cycle = Cycle
                    };
                    WebUser.Transactions.Add(newTransaction);
                } 
            }


            // if(TransactionType == "Expense"){
            //     ClientScript
            // }
            return RedirectToPage("/Dashboard");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding Transaction: {ex.Message}");
            return RedirectToPage("/Error"); // Handle error appropriately
        }
    }


    // [HttpGet]
    // [Route("download-transactions")]
    // public IActionResult DownloadTransactions()
    // {
    //     // Get the current user's transactions
    //     var transactions = WebUser.GetCurrentUserTransactions(WebUser);

    //     // Serialize the transactions to JSON
    //     string transactionsJson = JsonSerializer.Serialize(transactions, new JsonSerializerOptions { WriteIndented = true });

    //     // Convert the JSON string to a byte array
    //     var fileBytes = System.Text.Encoding.UTF8.GetBytes(transactionsJson);
    //     var fileName = "transactions.json";

    //     // Return the file for download
    //     return File(fileBytes, "Financetracker/json", fileName);
    // }



}
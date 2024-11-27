using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace FinanceTracker.Pages;

public class TransactionsModel : PageModel
{

    [BindProperty]
    public required string Type { get; set; }
    [BindProperty]
    public required string Category { get; set; }

    [BindProperty]
    public required string UseCase { get; set; }
    [BindProperty]
    public required int Amount { get; set; }
    [BindProperty]
    public required string Origin { get; set; }
    [BindProperty]
    public required string Destination { get; set; }
    [BindProperty]
    public required DateTime Date { get; set; }

    public required string ID { get; set; }

    [BindProperty]
    public required string TableTransactionId { get; set; }




    public WebUser? WebUser { get; set; }
    public IActionResult OnGet()
    {


        WebUser = null;
        if (!Request.Cookies.TryGetValue("SessionCookie", out string? sessionId)) return RedirectToPage("/Index");
        if (sessionId == null) return RedirectToPage("/Index");
        WebUser = WebUser.GetUserBySession(sessionId);
        if (WebUser == null) return RedirectToPage("/Index");
        Console.WriteLine($"User: {WebUser.Name} Email: {WebUser.Email} Loaded Page: /Transactions");
        return Page();
    }

    public IActionResult OnPostSubmit()
    {



        WebUser = null;
        if (!Request.Cookies.TryGetValue("SessionCookie", out string? sessionId)) return RedirectToPage("/Index");
        if (sessionId == null) return RedirectToPage("/Index");
        WebUser = WebUser.GetUserBySession(sessionId);
        if (WebUser == null) return RedirectToPage("/Index");

        var newTransaction = new Transaction
        (
            Type,
            Category,
            UseCase,
            Amount,
            Origin,
            Destination,
            Date,
            ID

        );

        newTransaction.ID = System.Guid.NewGuid().ToString();
        // Assuming WebUser is initialized somewhere and has a Transactions property
        WebUser.Transactions.Add(newTransaction);


        Console.WriteLine("User: " + WebUser.Name + "   Email: " + WebUser.Email + " Add Transaction Post");
        // Return JSON response of the transactions
        return RedirectToPage("Transactions");

    }


    public IActionResult OnPostDeleteTransaction(string TableTransactionId)
    {
        WebUser = null;
        if (!Request.Cookies.TryGetValue("SessionCookie", out string? sessionId)) return RedirectToPage("/Index");
        if (sessionId == null) return RedirectToPage("/Index");
        WebUser = WebUser.GetUserBySession(sessionId);
        if (WebUser == null) return RedirectToPage("/Index");

        if (TableTransactionId != null)
        {
            WebUser.Transactions.Remove(WebUser.GetTransactionByID(TableTransactionId));
            Console.WriteLine($"Transaction with ID: {TableTransactionId} deleted!");
        }

        return Page();
    }

}


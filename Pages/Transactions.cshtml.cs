using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;

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
    public required string Trans_ID { get; set; }




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

    public IActionResult OnPostAddTransaction()
    {

        try
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
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding transaction: {ex.Message}");
            return RedirectToPage("/Error"); // Handle error appropriately
        }
    }

    public IActionResult OnPostEditTransaction(string Trans_ID)
    {
        WebUser = null;
        if (!Request.Cookies.TryGetValue("SessionCookie", out string? sessionId)) return RedirectToPage("/Index");
        if (sessionId == null) return RedirectToPage("/Index");
        WebUser = WebUser.GetUserBySession(sessionId);
        if (WebUser == null) return RedirectToPage("/Index");

        if (Trans_ID != null)
        {

            var currentTransaction = WebUser.GetTransactionByID(Trans_ID);

            currentTransaction.Type = Type;
            currentTransaction.Category = Category;
            currentTransaction.UseCase = UseCase;
            currentTransaction.Amount = Amount;
            currentTransaction.Origin = Origin;
            currentTransaction.Destination = Destination;
            currentTransaction.Date = Date;
            currentTransaction.ID = Trans_ID;
        }

        return Page();
    }


    public IActionResult OnPostDeleteTransaction(string Trans_ID)
    {
        try
        {
            WebUser = null;
            if (!Request.Cookies.TryGetValue("SessionCookie", out string? sessionId)) return RedirectToPage("/Index");
            if (sessionId == null) return RedirectToPage("/Index");
            WebUser = WebUser.GetUserBySession(sessionId);
            if (WebUser == null) return RedirectToPage("/Index");

            if (Trans_ID != null)
            {
                WebUser.Transactions.Remove(WebUser.GetTransactionByID(Trans_ID));
                Console.WriteLine($"Transaction with ID: {Trans_ID} deleted!");
            }

            return Page();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting transaction: {ex.Message}");
            return RedirectToPage("/Error");
        }
    }

}


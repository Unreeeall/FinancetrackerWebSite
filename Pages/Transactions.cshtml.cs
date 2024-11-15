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




    public WebUser? WebUser {get; set; }
    public IActionResult OnGet()
    {
        WebUser = null;
        if(!Request.Cookies.TryGetValue("SessionCookie", out string? sessionId)) return RedirectToPage("/Index");
        if(sessionId == null) return RedirectToPage("/Index");
        WebUser = WebUser.GetUserBySession(sessionId);
        if(WebUser == null) return RedirectToPage("/Index");
        return Page();
    }

    public IActionResult OnPost()
{
    var newTransaction = new Transaction
    (
        Type,
        Category,
        UseCase,
        Amount,
        Origin,
        Destination,
        Date
    );

    // Assuming WebUser is initialized somewhere and has a Transactions property
    WebUser?.Transactions.Add(newTransaction);

    // Return JSON response of the transactions
    return new JsonResult(WebUser?.Transactions);
}
}
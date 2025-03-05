// using FinanceUser;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.RazorPages;

// namespace FinanceTracker.Pages;

// public class BudgetModel : PageModel
// {
//     public WebUser? WebUser {get; set; }
//     public IActionResult OnGet()
//     {
//         WebUser = null;
//         if(!Request.Cookies.TryGetValue("SessionCookie", out string? sessionId)) return RedirectToPage("/Index");
//         if(sessionId == null) return RedirectToPage("/Index");
//         WebUser = WebUser.GetUserBySession(sessionId);
//         if(WebUser == null) return RedirectToPage("/Index");
//         if(!WebUser.HasFinancialAccounts()) return RedirectToPage("/AddFinAccDash");
//         Console.WriteLine($"User: {WebUser.Name} Email: {WebUser.Email} Loaded Page: /Budget");
//         return Page();
//     }
// }
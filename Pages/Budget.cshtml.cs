using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace FinanceTracker.Pages;

public class BudgetModel : PageModel
{
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
}
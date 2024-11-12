using System.Dynamic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinanceTracker.Pages;

public class LogoutModel : PageModel
{
    public IActionResult OnGet()
    {
        if(!Request.Cookies.TryGetValue("SessionCookie", out string? sessionId)) return RedirectToPage("/Index");
        if(sessionId == null) return RedirectToPage("/Index");
        WebUser.DeleteSession(sessionId);
        Response.Cookies.Delete("SessionCookie");
        return RedirectToPage("/Index");
    }
}
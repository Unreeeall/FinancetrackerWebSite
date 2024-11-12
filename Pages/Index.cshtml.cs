using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace FinanceTracker.Pages;

public class IndexModel : PageModel
{
    public WebUser? user {get; set; }
    public IActionResult OnGet()
    {
        user = null;
        if(!Request.Cookies.TryGetValue("SessionCookie", out string? sessionId)) return Page();
        if(sessionId == null) return Page();
        user = WebUser.GetUserBySession(sessionId);
        if(user == null) return Page();
        return RedirectToPage("/Dashboard");
        
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace FinanceTracker.Pages;

public class IndexModel : PageModel
{
    public WebUser? WebUser {get; set; }
    public IActionResult OnGet()
    {
        WebUser = null;
        if(!Request.Cookies.TryGetValue("SessionCookie", out string? sessionId)) return Page();
        if(sessionId == null) return Page();
        WebUser = WebUser.GetUserBySession(sessionId);
        if(WebUser == null) {Console.WriteLine("Unkown user Loaded page: /Index ");  return Page(); } 
        Console.WriteLine($"User: {WebUser.Name} Email: {WebUser.Email} Loaded Page: /Index");

        return RedirectToPage("/Dashboard");
        
    }
}

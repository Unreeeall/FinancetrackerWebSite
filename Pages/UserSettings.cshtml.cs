using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace FinanceTracker.Pages;

public class UserSettingsModel : PageModel
{
    public WebUser? WebUser {get; set; }
    public void OnGet()
    {
        WebUser = null;
        if(!Request.Cookies.TryGetValue("SessionCookie", out string? sessionId)) return;
        if(sessionId == null) return;
        WebUser = WebUser.GetUserBySession(sessionId);
        Console.WriteLine($"User: {WebUser.Name} Email: {WebUser.Email} Loaded Page: /UserSettings");
        if(WebUser == null) return;
    }
}
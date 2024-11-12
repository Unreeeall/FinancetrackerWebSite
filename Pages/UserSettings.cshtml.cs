using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace FinanceTracker.Pages;

public class UserSettingsModel : PageModel
{
    public WebUser? user {get; set; }
    public void OnGet()
    {
        user = null;
        if(!Request.Cookies.TryGetValue("SessionCookie", out string? sessionId)) return;
        if(sessionId == null) return;
        user = WebUser.GetUserBySession(sessionId);
        if(user == null) return;
    }
}
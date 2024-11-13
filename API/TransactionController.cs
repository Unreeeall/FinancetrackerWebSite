using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TransactionController : ControllerBase
{
    [HttpGet]
    public IActionResult OnGet()
    {
        WebUser? user;
        if(!Request.Cookies.TryGetValue("SessionCookie", out string? sessionId)) return BadRequest();
        if(sessionId == null) return BadRequest();
        user = WebUser.GetUserBySession(sessionId);
        if(user == null) return BadRequest();



        return Content(JsonSerializer.Serialize(user.Transactions, new JsonSerializerOptions { WriteIndented = true }));
    }
}
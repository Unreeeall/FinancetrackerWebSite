using System;
using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinanceTracker.Pages;

public class LoginModel : PageModel
{

    [BindProperty]
    public required string Email { get; set; }

    [BindProperty]
    public required string Password { get; set; }

    [BindProperty]
    public bool SaveData { get; set; }


    public bool WrongPassword { get; set; }

    public void OnGet()
    {

    }

    public IActionResult OnPost()
    {
        Console.WriteLine(Email);
        Console.WriteLine(Password);
        Console.WriteLine(SaveData);


        
            if (WebUser.EmailExists(Email))
            {
                WebUser? user = WebUser.getUserByEmail(Email);
                if (user != null)
                {
                    Console.WriteLine($"Webuser Password: {user.Password}, Input Password: {Password}");
                    if (user.Password == Password)
                    {
                        TimeSpan expiretime;

                        if (SaveData)
                        {
                            expiretime = new TimeSpan(30, 0, 0, 0);
                        }
                        else
                        {
                            expiretime = new TimeSpan(1, 0, 0, 0);
                        }
                            string sessionString = user.CreateSession(DateTime.Now + expiretime);
                            Response.Cookies.Append("SessionCookie", sessionString, new CookieOptions{ HttpOnly = true, Expires = DateTime.UtcNow.Add(expiretime)});
                        WebUser.saveJson();
                        Console.WriteLine($"Login from user: {user.Name} {user.Email}");
                        return RedirectToPage("Index");
                    }
                    else
                    {
                        Console.WriteLine("Login Failed: Wrong Password");
                        WrongPassword = true;
                        return Page();
                    }
                }
                else
                {
                    Console.WriteLine("User not found.");
                }
            }
            else
            {
                Console.WriteLine("Email does not Exist!");
                return Page();
            }
            return Page();
    }
}


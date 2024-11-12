using System;
using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinanceTracker.Pages
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public required string Email { get; set; }
        [BindProperty]
        public required string Name { get; set; }
        [BindProperty]
        public string? Phonenumber { get; set; }
        [BindProperty]
        public required string Password { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            Console.WriteLine(Password);


            if (WebUser.EmailExists(Email))
            {
                TempData["EmailExists"] = true;
                return Page();
            }


            // Create a new User object with the form data
            var newUser = new WebUser
            (
                Email,
                Name,
                Phonenumber,
                Password
            );

            // Specify the file path where the JSON string will be written
            
            return RedirectToPage("Login");
        }
    }


}
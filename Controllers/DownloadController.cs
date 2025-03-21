using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using FinanceUser;


namespace FinanceTracker.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class DownloadController : ControllerBase
    {
        private static JsonSerializerOptions jsonSerializerOption = new JsonSerializerOptions { WriteIndented = true };
        [HttpGet]
        [Route("download-transactions")]
        public IActionResult DownloadTransactions(string userEmail)
        {

            Console.WriteLine($"Received request to download transactions for email: {userEmail}");
            var webUser = WebUser.getUserByEmail(userEmail);
            if (webUser != null)
            {
                // Debugging output
                Console.WriteLine($"User found: {webUser.Email}");
                Console.WriteLine($"Number of transactions: {webUser.Transactions.Count}");

                // Get the current user's transactions
                var transactions = WebUser.GetCurrentUserTransactions(webUser);

                // Serialize the transactions to JSON
                string transactionsJson = JsonSerializer.Serialize(transactions, jsonSerializerOption);

                // Debugging output

                // Convert the JSON string to a byte array
                var fileBytes = Encoding.UTF8.GetBytes(transactionsJson);
                var fileName = "BudgetBuddy-transactions.json";

                // Return the file for download
                return File(fileBytes, "application/json", fileName);
            }
            else
            {
                // Debugging output
                Console.WriteLine($"No user found with email: {userEmail}");
                return NotFound("User not found");
            }
        }

        
    }
}
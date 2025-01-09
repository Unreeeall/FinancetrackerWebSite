using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace FinanceTracker.Controllers
{
    public class TransactionController : Controller
    {
        [HttpPost]
        [Route("import-transactions")]
        public async Task<IActionResult> ImportTransactions(IFormFile file, string userEmail)
        {
            var webUser = WebUser.getUserByEmail(userEmail);
            if (webUser == null)
            {
                return NotFound("User not found.");
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded or the file is empty.");
            }

            List<Transaction>? importedTransactions;
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;

                importedTransactions = await JsonSerializer.DeserializeAsync<List<Transaction>>(stream);
            }
            if(importedTransactions == null)
            {
                return BadRequest("importedTransactions are empty");
            }
            

            var existingTransactions = WebUser.GetCurrentUserTransactions(webUser);
            var newTransactions = importedTransactions
                .Where(imported => !existingTransactions.Any(existing => existing.ID == imported.ID))
                .ToList();

            // Add new transactions to the existing list
            webUser.Transactions.AddRange(newTransactions);

            // Save the updated list
            WebUser.saveJson();

            Console.WriteLine($"Imported transactions: {newTransactions.Count}");
            Console.WriteLine($"Skipped transactions: {importedTransactions.Count - newTransactions.Count}");

            return Json(new { ImportedCount = newTransactions.Count, SkippedCount = importedTransactions.Count - newTransactions.Count });
        }



        
    }
}
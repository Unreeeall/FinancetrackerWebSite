using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;


namespace FinanceTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionTableController : Controller {


        [HttpGet]
        [Route("get-last-five-transactions")]
        public IActionResult GetLastFiveTransactions([FromQuery] string userEmail, [FromQuery] string accId)
        {
            var webUser = WebUser.getUserByEmail(userEmail);
            var transactions = webUser.GetTransactionsByAccID(accId)
                .OrderByDescending(t => t.Date)
                .Take(5)
                .Select(t => new
                {
                    t.Type,
                    t.Category,
                    t.Amount,
                    OriginName = webUser.GetAccountNameById(t.Origin),
                    DestinationName = webUser.GetAccountNameById(t.Destination),
                    t.Description,
                    t.Date
                });

            return Ok(transactions);
        }


        [HttpPost]
        [Route("add-Transaction")]
        public void AddTransaction([FromBody] TransactionRequest request)
        {
            try
            {
                var webUser = WebUser.getUserByEmail(request.UserEmail);

                if (decimal.TryParse(request.Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal parsedAmount))
                {

                    if (request.IsContract)
                    {

                        var newContract = new Contract
                        (
                            request.TransactionType,
                            request.Category,
                            parsedAmount,
                            request.AccID,
                            request.Cycle,
                            request.Date,
                            request.Origin,
                            request.Destination,
                            request.EndDate
                        );
                        webUser?.Contracts.Add(newContract);
                    }
                    else
                    {
                        var newTransaction = new Transaction
                        (
                        request.TransactionType,
                        request.Date,
                        parsedAmount,
                        request.Origin,
                        request.Destination,
                        request.Description,
                        request.Category,
                        System.Guid.NewGuid().ToString(),
                        request.AccID,
                        request.IsContract,
                        request.Cycle
                        );
                        webUser?.Transactions.Add(newTransaction);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding Transaction: {ex.Message}");
            }
        }

    }
}
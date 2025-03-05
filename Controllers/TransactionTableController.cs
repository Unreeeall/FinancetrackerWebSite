using FinanceUser;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Transactions;


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
            var transactions = webUser?.GetTransactionsByAccID(accId)
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
                        {
                            ContractId = Guid.NewGuid().ToString(),
                            Type = request.TransactionType,
                            Category = request.Category,
                            Amount = parsedAmount,
                            AccountID = request.AccID,
                            Cycle = (BillingCycle)request.Cycle,
                            StartDate = request.Date,
                            Origin = request.Origin,
                            Destination = request.Destination,
                            EndDate = request.EndDate
                        };
                        webUser?.Contracts.Add(newContract);
                    }
                    else
                    {
                        var newTransaction = new Transaction
                        {
                        Type = request.TransactionType,
                        Date = request.Date,
                        Amount = parsedAmount,
                        Origin = request.Origin,
                        Destination = request.Destination,
                        Description = request.Description,
                        Category = request.Category,
                        ID = System.Guid.NewGuid().ToString(),
                        AccountId = request.AccID,
                        IsContract = request.IsContract,
                        Cycle = request.Cycle
                        };
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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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

    }
}
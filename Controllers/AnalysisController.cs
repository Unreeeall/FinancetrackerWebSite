using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text.Json;
using System.Text;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinanceTracker.Controllers
{
    public class AnalysisController : Controller
    {
        [HttpGet]
        [Route("api/fetch-data")]
        public IActionResult FetchAccountData(string userEmail,string accID ,DateTime firstDayOfWeek)
        {
            var webUser = WebUser.getUserByEmail(userEmail);

            decimal[] weeklyAccBalance = webUser.CalculateWeeklyDailyAccountBalance(firstDayOfWeek, accID);
            
            return Ok();
        }



    }
}
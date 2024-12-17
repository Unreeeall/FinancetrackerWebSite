using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text.Json;
using System.Text;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinanceTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalysisController : Controller
    {
        [HttpGet]
        [Route("fetch-weekly-data")]
        public IActionResult GetWeeklyBalance([FromQuery] string userEmail, [FromQuery] DateTime firstDateOfWeek, [FromQuery] string accID)
        {
            var webUser = WebUser.getUserByEmail(userEmail);
            if (webUser == null) return NotFound("User not found.");

            decimal[] weeklyAccBalance = webUser.CalculateWeeklyDailyAccountBalance(firstDateOfWeek, accID);

            return Ok(weeklyAccBalance);
        }

        [HttpGet]
        [Route("fetch-monthly-data")]
        public IActionResult GetMonthylBalance([FromQuery] string userEmail, [FromQuery] DateTime firstDateOfWeek, [FromQuery] string accID)
        {
            var webUser = WebUser.getUserByEmail(userEmail);
            if (webUser == null) return NotFound("User not found.");

            decimal[] monthlyAccBalance = webUser.CalculateMonthlyDailyAccountBalance(firstDateOfWeek, accID);

            return Ok(monthlyAccBalance);
        }


        [HttpGet]
        [Route("fetch-yearly-data")]
        public IActionResult GetYearlyBalance([FromQuery] string userEmail, [FromQuery] DateTime firstDateOfWeek, [FromQuery] string accID)
        {
            var webUser = WebUser.getUserByEmail(userEmail);
            if (webUser == null) return NotFound("User not found.");

            decimal[] yearlyAccBalance = webUser.CalculateYearlyMonthlyAccountBalance(firstDateOfWeek, accID);

            return Ok(yearlyAccBalance);
        }



        [HttpGet]
        [Route("fetch-category-expense-data")]
        public IActionResult GetCategoryExpense([FromQuery] string userEmail, [FromQuery] DateTime firstDateOfWeek, [FromQuery] string accID, [FromQuery] string timeFrame)
        {

            Console.WriteLine("EIER 1");
            Dictionary<string, decimal> weekylExpensesByCategory = WebUser.FinancialReport.GenerateAccountExpenseReport(userEmail, accID, firstDateOfWeek, timeFrame);

            
            if (weekylExpensesByCategory == null)
            {
                return NotFound("No expenses found for the specified criteria.");
            }

            string[] Categories = weekylExpensesByCategory.Keys.ToArray();
            decimal[] Expenses = weekylExpensesByCategory.Values.ToArray();

            var newExpenseReport = new WebUser.ExpenseIncomeReport(
                Categories,
                Expenses
            );

            return Ok(newExpenseReport);
        }

        [HttpGet]
        [Route("fetch-category-income-data")]
        public IActionResult GetCategoryIncome([FromQuery] string userEmail, [FromQuery] DateTime firstDateOfWeek, [FromQuery] string accID, [FromQuery] string timeFrame)
        {
            Console.WriteLine("EIER 2");

            Dictionary<string, decimal> weekylIncomeByCategory = WebUser.FinancialReport.GenerateAccountIncomeReport(userEmail, accID, firstDateOfWeek, timeFrame);

            

            if (weekylIncomeByCategory == null)
            {
                return NotFound("No expenses found for the specified criteria.");
            }

            string[] Categories = weekylIncomeByCategory.Keys.ToArray();
            decimal[] Expenses = weekylIncomeByCategory.Values.ToArray();

            var newIncomeReport = new WebUser.ExpenseIncomeReport(
                Categories,
                Expenses
            );

            return Ok(newIncomeReport);
        }
    }
}
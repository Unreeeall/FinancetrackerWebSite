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
        [Route("fetch-weekly-category-expense-data")]
        public IActionResult GetWeeklyCategoryExpense([FromQuery] string userEmail, [FromQuery] DateTime firstDateOfWeek, [FromQuery] string accID)
        {
            Dictionary<string, decimal> weekylExpensesByCategory = WebUser.FinancialReport.GenerateWeeklyAccountExpenseReport(userEmail, accID, firstDateOfWeek);

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
        [Route("fetch-weekly-category-income-data")]
        public IActionResult GetWeeklyCategoryIncome([FromQuery] string userEmail, [FromQuery] DateTime firstDateOfWeek, [FromQuery] string accID)
        {

            Dictionary<string, decimal> weekylIncomeByCategory = WebUser.FinancialReport.GenerateWeeklyAccountIncomeReport(userEmail, accID, firstDateOfWeek);

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


        [HttpGet]
        [Route("fetch-monthly-category-expense-data")]
        public IActionResult GetMonthlyCategoryExpense([FromQuery] string userEmail, [FromQuery] DateTime firstDateOfWeek, [FromQuery] string accID)
        {
            Dictionary<string, decimal> monthlyExpenseByCategory = WebUser.FinancialReport.GenerateMonthlyAccountExpenseReport(userEmail, accID, firstDateOfWeek);

            if (monthlyExpenseByCategory == null)
            {
                return NotFound("No expenses found for the specified criteria.");
            }

            string[] Categories = monthlyExpenseByCategory.Keys.ToArray();
            decimal[] Expenses = monthlyExpenseByCategory.Values.ToArray();

            var newExpenseReport = new WebUser.ExpenseIncomeReport(
                Categories,
                Expenses
            );

            return Ok(newExpenseReport);
        }


        [HttpGet]
        [Route("fetch-monthly-category-income-data")]
        public IActionResult GetMonthlyCategoryIncome([FromQuery] string userEmail, [FromQuery] DateTime firstDateOfWeek, [FromQuery] string accID)
        {
            Dictionary<string, decimal> monthlyIncomeByCategory = WebUser.FinancialReport.GenerateMonthlyAccountIncomeReport(userEmail, accID, firstDateOfWeek);

            if (monthlyIncomeByCategory == null)
            {
                return NotFound("No expenses found for the specified criteria.");
            }

            string[] Categories = monthlyIncomeByCategory.Keys.ToArray();
            decimal[] Expenses = monthlyIncomeByCategory.Values.ToArray();

            var newIncomeReport = new WebUser.ExpenseIncomeReport(
                Categories,
                Expenses
            );

            return Ok(newIncomeReport);
        }


        [HttpGet]
        [Route("fetch-yearly-category-income-data")]
        public IActionResult GetYearlyCategoryIncome([FromQuery] string userEmail, [FromQuery] DateTime firstDateOfWeek, [FromQuery] string accID)
        {
            Dictionary<string, decimal> yearlyIncomeByCategory = WebUser.FinancialReport.GenerateYearlyAccountIncomeReport(userEmail, accID, firstDateOfWeek);

            if (yearlyIncomeByCategory == null)
            {
                return NotFound("No expenses found for the specified criteria.");
            }

            string[] Categories = yearlyIncomeByCategory.Keys.ToArray();
            decimal[] Expenses = yearlyIncomeByCategory.Values.ToArray();

            var newIncomeReport = new WebUser.ExpenseIncomeReport(
                Categories,
                Expenses
            );

            return Ok(newIncomeReport);
        }


        [HttpGet]
        [Route("fetch-yearly-category-expense-data")]
        public IActionResult GetYearlyCategoryExpense([FromQuery] string userEmail, [FromQuery] DateTime firstDateOfWeek, [FromQuery] string accID)
        {
            Dictionary<string, decimal> yearlyExpenseByCategory = WebUser.FinancialReport.GenerateMonthlyAccountExpenseReport(userEmail, accID, firstDateOfWeek);

            if (yearlyExpenseByCategory == null)
            {
                return NotFound("No expenses found for the specified criteria.");
            }

            string[] Categories = yearlyExpenseByCategory.Keys.ToArray();
            decimal[] Expenses = yearlyExpenseByCategory.Values.ToArray();

            var newExpenseReport = new WebUser.ExpenseIncomeReport(
                Categories,
                Expenses
            );

            return Ok(newExpenseReport);
        }



    }
}
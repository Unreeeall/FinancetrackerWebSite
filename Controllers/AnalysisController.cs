using FinanceUser;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalysisController : Controller
    {
        private readonly string UserNotFound = "User not found.";
        [HttpGet]
        [Route("fetch-weekly-data")]
        public IActionResult GetWeeklyBalance([FromQuery] string userEmail, [FromQuery] DateTime firstDateOfWeek, [FromQuery] string accID)
        {
            var webUser = WebUser.getUserByEmail(userEmail);
            if (webUser == null) return NotFound(UserNotFound);

            decimal[] weeklyAccBalance = webUser.CalculateWeeklyDailyAccountBalance(firstDateOfWeek, accID);

            return Ok(weeklyAccBalance);
        }

        [HttpGet]
        [Route("fetch-monthly-data")]
        public IActionResult GetMonthylBalance([FromQuery] string userEmail, [FromQuery] DateTime firstDateOfWeek, [FromQuery] string accID)
        {
            var webUser = WebUser.getUserByEmail(userEmail);
            if (webUser == null) return NotFound(UserNotFound);

            decimal[] monthlyAccBalance = webUser.CalculateMonthlyDailyAccountBalance(firstDateOfWeek, accID);

            return Ok(monthlyAccBalance);
        }


        [HttpGet]
        [Route("fetch-yearly-data")]
        public IActionResult GetYearlyBalance([FromQuery] string userEmail, [FromQuery] DateTime firstDateOfWeek, [FromQuery] string accID)
        {
            var webUser = WebUser.getUserByEmail(userEmail);
            if (webUser == null) return NotFound(UserNotFound);

            decimal[] yearlyAccBalance = webUser.CalculateYearlyMonthlyAccountBalance(firstDateOfWeek, accID);

            return Ok(yearlyAccBalance);
        }



        [HttpGet]
        [Route("fetch-category-expense-data")]
        public IActionResult GetCategoryExpense([FromQuery] string userEmail, [FromQuery] DateTime firstDateOfWeek, [FromQuery] string accID, [FromQuery] string timeFrame)
        {
            var webUser = WebUser.getUserByEmail(userEmail);
            if (webUser == null) return NotFound(UserNotFound);
            Dictionary<string, decimal>? weekylExpensesByCategory = webUser.GenerateAccountExpenseReport(userEmail, accID, firstDateOfWeek, timeFrame);


            if (weekylExpensesByCategory == null)
            {
                return NotFound("No expenses found for the specified criteria.");
            }

            string[] Categories = weekylExpensesByCategory.Keys.ToArray();
            decimal[] Expenses = weekylExpensesByCategory.Values.ToArray();

            var newExpenseReport = new ExpenseIncomeReport(
                Categories,
                Expenses
            );

            return Ok(newExpenseReport);
        }

        [HttpGet]
        [Route("fetch-category-income-data")]
        public IActionResult GetCategoryIncome([FromQuery] string userEmail, [FromQuery] DateTime firstDateOfWeek, [FromQuery] string accID, [FromQuery] string timeFrame)
        {
            var webUser = WebUser.getUserByEmail(userEmail);
            if (webUser == null) return NotFound(UserNotFound);

            Dictionary<string, decimal>? weekylIncomeByCategory = webUser.GenerateAccountIncomeReport(userEmail, accID, firstDateOfWeek, timeFrame);



            if (weekylIncomeByCategory == null)
            {
                return NotFound("No expenses found for the specified criteria.");
            }

            string[] Categories = weekylIncomeByCategory.Keys.ToArray();
            decimal[] Expenses = weekylIncomeByCategory.Values.ToArray();

            var newIncomeReport = new ExpenseIncomeReport(
                Categories,
                Expenses
            );

            return Ok(newIncomeReport);
        }


        [HttpGet]
        [Route("calculate-income")]
        public JsonResult CalculateAccIncome([FromQuery] DateTime date, [FromQuery] string userEmail, [FromQuery] string accID, [FromQuery] string rangeType)
        {
            var webUser = WebUser.getUserByEmail(userEmail); 
            var totalIncome = webUser?.CalculateAccIncomeForTimeframe(date, accID, rangeType);

            return Json(new { totalIncome = totalIncome });
        }

        [HttpGet]
        [Route("calculate-expense")]
        public JsonResult CalculateAccExpense([FromQuery] DateTime date, [FromQuery] string userEmail, [FromQuery] string accID, [FromQuery] string rangeType)
        {
            var webUser = WebUser.getUserByEmail(userEmail); 
            var totalExpense = webUser?.CalculateAccExpenseForTimeframe(date, accID, rangeType);

            return Json(new { totalExpense = totalExpense });
        }





        [HttpGet]
        [Route("fetch-weekly-income-data")]
        public IActionResult GetDailyIncome([FromQuery] string userEmail, [FromQuery] DateTime date, [FromQuery] string accID)
        {
            var webUser = WebUser.getUserByEmail(userEmail);
            decimal[]? dailyIncome = webUser?.GetDailyAccIncome(date, accID);

            return Ok(dailyIncome);
        }

        [HttpGet]
        [Route("fetch-weekly-expense-data")]
        public IActionResult GetDailyExpense([FromQuery] string userEmail, [FromQuery] DateTime date, [FromQuery] string accID)
        {
            var webUser = WebUser.getUserByEmail(userEmail);
            decimal[]? dailyExpense = webUser?.GetDailyAccExpense(date, accID);

            return Ok(dailyExpense);
        }


        [HttpGet]
        [Route("fetch-monthly-income-data")]
        public IActionResult GetMothlyIncome([FromQuery] string userEmail, [FromQuery] DateTime date, [FromQuery] string accID)
        {
            var webUser = WebUser.getUserByEmail(userEmail);
            decimal[]? dailyIncome = webUser?.GetMonthlyAccIncome(date, accID);

            return Ok(dailyIncome);
        }

        [HttpGet]
        [Route("fetch-monthly-expense-data")]
        public IActionResult GetMothlyExpense([FromQuery] string userEmail, [FromQuery] DateTime date, [FromQuery] string accID)
        {
            var webUser = WebUser.getUserByEmail(userEmail);
            decimal[]? dailyExpense = webUser?.GetMonthlyAccExpense(date, accID);

            return Ok(dailyExpense);
        }


        [HttpGet]
        [Route("fetch-yearly-income-data")]
        public IActionResult GetYearlyIncome([FromQuery] string userEmail, [FromQuery] DateTime date, [FromQuery] string accID)
        {
            var webUser = WebUser.getUserByEmail(userEmail);
            decimal[]? dailyIncome = webUser?.GetYearlyAccIncome(date, accID);

            return Ok(dailyIncome);
        }

        [HttpGet]
        [Route("fetch-yearly-expense-data")]
        public IActionResult GetYearlyExpense([FromQuery] string userEmail, [FromQuery] DateTime date, [FromQuery] string accID)
        {
            var webUser = WebUser.getUserByEmail(userEmail);
            decimal[]? dailyExpense = webUser?.GetYearlyAccExpense(date, accID);

            return Ok(dailyExpense);
        }
    }
}
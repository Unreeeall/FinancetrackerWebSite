using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Web;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.ComponentModel;
using FinanceTracker.Pages;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Data.SqlTypes;
using System.Globalization;






public class WebUser
{
    public string Email { get; set; }
    public string Name { get; set; }
    public string? Phonenumber { get; set; }
    public string Password { get; set; }

    public List<Session> Sessions { get; set; } = [];

    static Dictionary<string, SessionUser> IdDict { get; set; } = [];

    public static Dictionary<string, FinancialAccount> AccountLookup = new Dictionary<string, FinancialAccount>();

    public List<Transaction> Transactions { get; set; } = [];
    public List<Contract> Contracts { get; set; } = [];
    public List<BankAccount> BankAccounts { get; set; } = [];
    public List<CashAccount> CashAccounts { get; set; } = [];
    public List<PortfolioAccount> PortfolioAccounts { get; set; } = [];
    public List<CryptoWallet> CryptoWallets { get; set; } = [];

    private static string filePath = Path.Combine(Directory.GetCurrentDirectory(), "UserData.json");

    private static List<WebUser> userList = new List<WebUser>();


    public WebUser() { Email = ""; Name = ""; Phonenumber = ""; Password = ""; userList.Add(this); }
    public WebUser(string email, string name, string? phonenumber, string password)
    { Email = email; Name = name; Phonenumber = phonenumber; Password = password; userList.Add(this); }

    public static void loadJson()
    {
        if (System.IO.File.Exists(filePath))
        {
            // Read the existing JSON data from the file
            string existingJson = System.IO.File.ReadAllText(filePath);

            // Try to deserialize the existing JSON data into a list of User objects
            userList = JsonSerializer.Deserialize<List<WebUser>>(existingJson) ?? new List<WebUser>();
            foreach (var user in userList)
            {
                foreach (var session in user.Sessions)
                {
                    IdDict.Add(session.Id, new SessionUser(session, user));
                }
                foreach (var bankAccount in user.BankAccounts)
                {
                    AccountLookup[bankAccount.ID] = bankAccount;
                }

                foreach (var cashAccount in user.CashAccounts)
                {
                    AccountLookup[cashAccount.ID] = cashAccount;
                }

                foreach (var portfolioAccount in user.PortfolioAccounts)
                {
                    AccountLookup[portfolioAccount.ID] = portfolioAccount;
                }

                foreach (var cryptoWallet in user.CryptoWallets)
                {
                    AccountLookup[cryptoWallet.ID] = cryptoWallet;
                }
                // user.CalculateAccountBalances();
                user.ApplyContracts();
            }

        }
    }

    public static void saveJson()
    {

        foreach (var user in userList)
        {
            List<Session> SessionsToDelete = [];
            foreach (var session in user.Sessions)
            {
                if (session.ExpireDate < DateTime.Now)
                {
                    SessionsToDelete.Add(session);
                }
            }
            foreach (var sessiontodelete in SessionsToDelete)
            {
                user.Sessions.Remove(sessiontodelete);
            }
        }
        try
        {
            // Add the new data to the list
            // Serialize the updated list to a JSON string 
            string updatedJson = JsonSerializer.Serialize(userList, new JsonSerializerOptions { WriteIndented = true });

            // Write the updated JSON string to the specified file
            System.IO.File.WriteAllText(filePath, updatedJson);

            Console.WriteLine($"Data successfully appended to {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while writing to the file: {ex.Message}");
        }
    }

    public static List<Transaction> GetCurrentUserTransactions(WebUser webUser)
    {
        if (webUser != null)
        {
            // Return the list of transactions for the current user
            Console.WriteLine("EIEIEIEIEIEIEIEIIE");

            var test = webUser.Transactions;
            Console.WriteLine($"EIEIE: {test.Count}");
            return webUser.Transactions;
        }
        else
        {
            // If user is not found, return an empty list or handle accordingly
            return new List<Transaction>();
        }
    }

    public static WebUser? getUserByEmail(string email)
    {
        Console.WriteLine($"Email: {email}");
        foreach (var user in userList)
        {
            Console.WriteLine($"Checking user: {user.Email}");
            if (user.Email == email)
            {
                Console.WriteLine(user.Email, user.Name, user.Password);
                return user;
            }
        }
        return null;
    }


    public static bool EmailExists(string Email)
    {
        Console.WriteLine("EmailExists Email: ", Email);
        if (userList.Any(user => user.Email == Email))
        {
            return true;
        }
        return false;
    }


    public string CreateSession(DateTime expireDate)
    {
        string base64String = Stuff.GenerateRandomBase64String(32);
        Session session = new Session(base64String, expireDate);
        Sessions.Add(session);
        IdDict.Add(base64String, new SessionUser(session, this));
        return base64String;
    }

    public static WebUser? GetUserBySession(string session)
    {
        if (!IdDict.ContainsKey(session)) return null;
        SessionUser sessionuser = IdDict[session];
        if (sessionuser.Session.ExpireDate < DateTime.Now) return null;
        return sessionuser.User;
    }


    public static void DeleteSession(string session)
    {
        if (!IdDict.ContainsKey(session)) return;
        SessionUser sessionuser = IdDict[session];
        sessionuser.User.Sessions.Remove(sessionuser.Session);
        IdDict.Remove(session);
    }

    public Transaction? GetTransactionByID(string transactionID)
    {
        foreach (var transaction in Transactions)
        {
            if (transaction.ID == transactionID)
            {
                Console.WriteLine($"Transaction found: {transaction}");
                return transaction;
            }
        }
        Console.WriteLine($"No transaction found with ID: {transactionID}");
        return null;
    }


    public List<Transaction> GetTransactionsByAccID(string accountID)
    {
        List<Transaction> accountTransactions = [];
        foreach (var transaction in Transactions)
        {
            if (transaction.AccountId == accountID)
            {
                accountTransactions.Add(transaction);
            }
        }
        return accountTransactions;
    }

    public void UpdateUser(string? email = null, string? name = null, string? phonenumber = null, string? password = null)
    {
        if (!string.IsNullOrEmpty(email)) Email = email;
        if (!string.IsNullOrEmpty(name)) Name = name;
        if (!string.IsNullOrEmpty(phonenumber)) Phonenumber = phonenumber;
        if (!string.IsNullOrEmpty(password)) Password = password;
        saveJson();
    }

    public void UpdateBankAccount(BankAccount bankAccount, string? name)
    {
        if (!string.IsNullOrEmpty(name)) bankAccount.AccountName = name;
    }

    public void UpdateCashAccount(CashAccount cashAccount, string? name)
    {
        if (!string.IsNullOrEmpty(name)) cashAccount.AccountName = name;
    }

    public void UpdatePortfolioAccount(PortfolioAccount portfolioAccount, string? name)
    {
        if (!string.IsNullOrEmpty(name)) portfolioAccount.AccountName = name;
    }

    public void UpdateCryptoWallet(CryptoWallet cryptoWallet, string? name)
    {
        if (!string.IsNullOrEmpty(name)) cryptoWallet.AccountName = name;
    }


    public bool HasFinancialAccounts()
    {
        if (BankAccounts.Count == 0 && PortfolioAccounts.Count == 0 && CryptoWallets.Count == 0)
        {
            return false;
        }
        else return true;
    }

    public BankAccount? GetBankAccountByID(string accountID)
    {
        foreach (var bankAccount in BankAccounts)
        {
            if (bankAccount.ID == accountID)
            {
                Console.WriteLine($"Bankaccount found: {bankAccount}");
                return bankAccount;
            }
        }
        Console.WriteLine($"No Bankaccount found with ID {accountID}");
        return null;
    }

    public CashAccount? GetCashAccountByID(string accountID)
    {
        foreach (var cashAccount in CashAccounts)
        {
            if (cashAccount.ID == accountID)
            {
                Console.WriteLine($"CashAccount found: {cashAccount}");
                return cashAccount;
            }
        }
        Console.WriteLine($"No CashAccount found with ID {accountID}");
        return null;
    }

    public PortfolioAccount? GetPortfolioAccountByID(string accountID)
    {
        foreach (var portfolioAccount in PortfolioAccounts)
        {
            if (portfolioAccount.ID == accountID)
            {
                Console.WriteLine($"PortfolioAccount found: {portfolioAccount}");
                return portfolioAccount;
            }
        }
        Console.WriteLine($"No PortfolioAccount found with ID {accountID}");
        return null;
    }

    public CryptoWallet? GetCryptoWalletByID(string accountID)
    {
        foreach (var cryptoWallet in CryptoWallets)
        {
            if (cryptoWallet.ID == accountID)
            {
                Console.WriteLine($"CryptoWallet found: {cryptoWallet}");
                return cryptoWallet;
            }
        }
        Console.WriteLine($"No CryptoWallet found with ID {accountID}");
        return null;
    }


    public FinancialAccount? GetAccountByID(string accountID)
    {
        if (AccountLookup.TryGetValue(accountID, out var account))
        {
            return account;
        }
        Console.WriteLine($"No account found with ID {accountID}");
        return null;
    }


    public string GetAccountNameById(string accountID)
    {
        foreach (var account in BankAccounts)
        {
            if (account.ID == accountID)
            {
                return account.AccountName;
            }
        }
        foreach (var account in CashAccounts)
        {
            if (account.ID == accountID)
            {
                return account.AccountName;
            }
        }
        foreach (var account in PortfolioAccounts)
        {
            if (account.ID == accountID)
            {
                return account.AccountName;
            }
        }
        foreach (var account in CryptoWallets)
        {
            if (account.ID == accountID)
            {
                return account.AccountName;
            }
        }
        return "";
    }

    public int GetWeekNumber(DateTime date)
    {
        CultureInfo ciCurr = CultureInfo.CurrentCulture;
        int weekNum = ciCurr.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        return weekNum;
    }



    public decimal[] CalculateWeeklyDailyAccountBalance(DateTime firstDateOfWeek, string accID)
    {
        Console.WriteLine($"Calculating WeeklyDaily Account balance for Account: {accID} with starting date: {firstDateOfWeek}");

        decimal totalBalance = 0;
        List<Transaction> sortedTransactions = Transactions.OrderBy(o => o.Date).ToList();
        decimal[] dailyBalances = new decimal[7];
        int dayOfWeek = 0;
        TimeSpan daySinceFirstDayOfWeek;

        long weekNumber = firstDateOfWeek.Ticks / 6048000000000;

        foreach (var transaction in sortedTransactions)
        {
            long transactionWeekNumber = transaction.Date.Ticks / 6048000000000;
            daySinceFirstDayOfWeek = transaction.Date.Date - firstDateOfWeek.Date;

            for (; transactionWeekNumber >= weekNumber && dayOfWeek < daySinceFirstDayOfWeek.Days && dayOfWeek < 7; dayOfWeek++)
            {
                dailyBalances[dayOfWeek] = totalBalance;
            }

            if (transaction.AccountId == accID)
            {
                if (transaction.Type == "Income")
                {
                    totalBalance += transaction.Amount;
                }
                else if (transaction.Type == "Expense")
                {
                    totalBalance -= transaction.Amount;
                }
            }
            else if (transaction.Type == "Transfer")
            {
                if (transaction.Origin == accID)
                {
                    totalBalance -= transaction.Amount;
                }
                else if (transaction.Destination == accID)
                {
                    totalBalance += transaction.Amount;
                }
            }
        }
        for (; dayOfWeek < 7; dayOfWeek++)
        {
            dailyBalances[dayOfWeek] = totalBalance;
        }

        return dailyBalances;
    }


    public decimal[] CalculateMonthlyDailyAccountBalance(DateTime dateOfMonth, string accID)
    {
        DateTime firstDateOfMonth = dateOfMonth.AddDays(-dateOfMonth.Day + 1);
        Console.WriteLine($"Calculating MonthlyDaily Account balance for Account: {accID} with starting date: {firstDateOfMonth}");

        decimal totalBalance = 0;
        List<Transaction> sortedTransactions = Transactions.OrderBy(o => o.Date).ToList();
        int daysInMonth = DateTime.DaysInMonth(firstDateOfMonth.Year, firstDateOfMonth.Month);
        decimal[] dailyBalances = new decimal[daysInMonth];
        int dayOfMonth = 0;
        TimeSpan daySinceFirstDayOfMonth;

        // long weekNumber = firstDateOfMonth.Ticks / 6048000000000;

        foreach (var transaction in sortedTransactions)
        {
            // long transactionWeekNumber = transaction.Date.Ticks / 6048000000000;
            daySinceFirstDayOfMonth = transaction.Date.Date - firstDateOfMonth.Date;

            for (; (transaction.Date.Month >= firstDateOfMonth.Month || transaction.Date.Year > firstDateOfMonth.Year) && dayOfMonth < daySinceFirstDayOfMonth.Days && dayOfMonth < daysInMonth; dayOfMonth++)
            {
                dailyBalances[dayOfMonth] = totalBalance;
            }

            if (transaction.AccountId == accID)
            {
                if (transaction.Type == "Income")
                {
                    totalBalance += transaction.Amount;
                }
                else if (transaction.Type == "Expense")
                {
                    totalBalance -= transaction.Amount;
                }
            }
            else if (transaction.Type == "Transfer")
            {
                if (transaction.Origin == accID)
                {
                    totalBalance -= transaction.Amount;
                }
                else if (transaction.Destination == accID)
                {
                    totalBalance += transaction.Amount;
                }
            }
        }
        for (; dayOfMonth < daysInMonth; dayOfMonth++)
        {
            dailyBalances[dayOfMonth] = totalBalance;
        }

        return dailyBalances;
    }



    public decimal[] CalculateYearlyMonthlyAccountBalance(DateTime dateOfYear, string accID)
    {

        Console.WriteLine($"Calculating YearlyMonthly Account balance for Account: {accID} with starting date: {dateOfYear}");

        decimal totalBalance = 0;
        List<Transaction> sortedTransactions = Transactions.OrderBy(o => o.Date).ToList();

        decimal[] monthlyBalances = new decimal[12];
        int monthOfYear = 0;
        int monthsSinceCurrentMonth;



        foreach (var transaction in sortedTransactions)
        {

            for (; transaction.Date.Year == dateOfYear.Year && monthOfYear < transaction.Date.Month; monthOfYear++)
            {
                monthlyBalances[monthOfYear] = totalBalance;
            }
            for (; transaction.Date.Year > dateOfYear.Year && monthOfYear < 12; monthOfYear++)
            {
                monthlyBalances[monthOfYear] = totalBalance;
            }

            if (transaction.AccountId == accID)
            {
                if (transaction.Type == "Income")
                {
                    totalBalance += transaction.Amount;
                }
                else if (transaction.Type == "Expense")
                {
                    totalBalance -= transaction.Amount;
                }
            }
            else if (transaction.Type == "Transfer")
            {
                if (transaction.Origin == accID)
                {
                    totalBalance -= transaction.Amount;
                }
                else if (transaction.Destination == accID)
                {
                    totalBalance += transaction.Amount;
                }
            }
        }
        for (; monthOfYear < 12; monthOfYear++)
        {
            monthlyBalances[monthOfYear] = totalBalance;
        }

        return monthlyBalances;
    }





    // public void EIER()
    // {
    //     var report = FinancialReport.GenerateReport(userList);

    //     Console.WriteLine($"Total Income: {report.TotalIncome}");
    //     Console.WriteLine($"Total Expenses: {report.TotalExpenses}");

    //     Console.WriteLine("Income by Category:");
    //     foreach (var category in report.IncomeByCategory)
    //     {
    //         Console.WriteLine($"{category.Key}: {category.Value}");
    //     }

    //     Console.WriteLine("Expenses by Category:");
    //     foreach (var category in report.ExpensesByCategory)
    //     {
    //         Console.WriteLine($"{category.Key}: {category.Value}");
    //     }
    // }

    public void CalculateAccountBalances()
    {
        // Reset balances to zero before recalculating
        foreach (var bankAccount in BankAccounts)
        {
            bankAccount.Balance = 0;
        }
        foreach (var cashAccount in CashAccounts)
        {
            cashAccount.Balance = 0;
        }
        foreach (var portfolioAccount in PortfolioAccounts)
        {
            portfolioAccount.Balance = 0;
        }
        foreach (var cryptoWallet in CryptoWallets)
        {
            cryptoWallet.Balance = 0;
        }

        // Recalculate balances based on transactions
        foreach (var transaction in Transactions)
        {
            if (transaction.Type == "Income" && transaction.AccountId != null)
            {
                var account = GetAccountByID(transaction.AccountId);
                if (account != null)
                {
                    account.Balance += transaction.Amount;
                }
            }
            else if (transaction.Type == "Expense" && transaction.AccountId != null)
            {
                var account = GetAccountByID(transaction.AccountId);
                if (account != null)
                {
                    account.Balance -= transaction.Amount;
                }
            }
            else if (transaction.Type == "Transfer")
            {
                if (transaction.Origin != null)
                {
                    var originAccount = GetAccountByID(transaction.Origin);
                    if (originAccount != null)
                    {
                        originAccount.Balance -= transaction.Amount;
                    }
                }
                if (transaction.Destination != null)
                {
                    var destinationAccount = GetAccountByID(transaction.Destination);
                    if (destinationAccount != null)
                    {
                        destinationAccount.Balance += transaction.Amount;
                    }
                }
            }
            else if (transaction.Type == "Stock")
            {
                var portfolioAccount = PortfolioAccounts.FirstOrDefault(a => a.ID == transaction.AccountId);
                if (portfolioAccount != null)
                {
                    var investment = portfolioAccount.Investments.FirstOrDefault(i => i.Ticker == transaction.Ticker);
                    if (investment != null)
                    {
                        investment.Quantity += (int)transaction.Amount;
                    }
                    else
                    {
                        portfolioAccount.Investments.Add(new Investment
                        {
                            Ticker = transaction.Ticker,
                            Quantity = (int)transaction.Amount,
                            PurchasePrice = transaction.Amount
                        });
                    }
                }
            }
            else if (transaction.Type == "Crypto")
            {
                var cryptoWallet = CryptoWallets.FirstOrDefault(a => a.ID == transaction.AccountId);
                if (cryptoWallet != null)
                {
                    var holding = cryptoWallet.CryptoHoldings.FirstOrDefault(h => h.Coin == transaction.Coin);
                    if (holding != null)
                    {
                        holding.Amount += transaction.Amount;
                    }
                    else
                    {
                        cryptoWallet.CryptoHoldings.Add(new CryptoHolding
                        {
                            Coin = (CryptoCoin)transaction.Coin,
                            Amount = transaction.Amount
                        });
                    }
                }
            }
        }
    }




    public decimal CalculateMonthlyIncome(int year, int month)
    {
        decimal totalIncome = 0;

        foreach (var transaction in Transactions)
        {
            if (transaction.Date.Year == year && transaction.Date.Month == month)
            {
                if (transaction.Type == "Income")
                {
                    totalIncome += transaction.Amount;
                }

            }
        }
        return totalIncome;
    }

    public decimal CalculateMonthlyExpense(int year, int month)
    {
        decimal TotalExpense = 0;

        foreach (var transaction in Transactions)
        {
            if (transaction.Date.Year == year && transaction.Date.Month == month)
            {
                if (transaction.Type == "Expense")
                {
                    TotalExpense += transaction.Amount;
                }
            }
        }
        return TotalExpense;
    }

    public decimal CalculateMonthlyTransferIncome(int year, int month, string accountID)
    {
        decimal TotalTransferAmount = 0;

        foreach (var transaction in Transactions)
        {
            if (transaction.Date.Year == year && transaction.Date.Month == month)
            {
                if (transaction.Type == "Transfer")
                {
                    if (transaction.Destination == accountID)
                    {
                        TotalTransferAmount += transaction.Amount;
                    }
                }
            }
        }
        return TotalTransferAmount;
    }

    public decimal CalculateMonthlyTransferExpense(int year, int month, string accountID)
    {
        decimal TotalTransferAmount = 0;

        foreach (var transaction in Transactions)
        {
            if (transaction.Date.Year == year && transaction.Date.Month == month)
            {
                if (transaction.Type == "Transfer")
                {
                    if (transaction.Origin == accountID)
                    {
                        TotalTransferAmount += transaction.Amount;
                    }
                }
            }
        }
        return TotalTransferAmount;
    }

    public decimal CalculateMonthlyTotalPlus(int year, int month, string accountID)
    {
        decimal Income = CalculateMonthlyIncome(year, month);
        decimal Expense = CalculateMonthlyExpense(year, month);
        decimal TransferInc = CalculateMonthlyTransferIncome(year, month, accountID);
        decimal TransferExp = CalculateMonthlyTransferExpense(year, month, accountID);

        decimal TotalPlus = Income + TransferInc - (Expense + TransferExp);

        return TotalPlus;
    }

    public decimal CalculateMonthlyTotalMinus(int year, int month, string accountID)
    {
        decimal Expense = CalculateMonthlyExpense(year, month);
        decimal TransferExp = CalculateMonthlyTransferExpense(year, month, accountID);

        decimal TotalPlus = Expense + TransferExp;

        return TotalPlus;
    }

    public void ApplyContracts()
    {
        foreach (var contract in Contracts)
        {
            if (contract.EndDate == null || contract.EndDate >= DateTime.Today)
            {
                DateTime nextDate = contract.StartDate;
                while (nextDate <= DateTime.Today)
                {
                    if (nextDate >= contract.StartDate && (contract.EndDate == null || nextDate <= contract.EndDate))
                    {
                        bool transactionExists = Transactions.Any(t => t.ContractId == contract.ContractId && t.Date == nextDate);
                        if (!transactionExists)
                        {
                            var newTransaction = new Transaction(
                                contract.Type,
                                nextDate,
                                contract.Amount,
                                contract.Origin,
                                contract.Destination,
                                "Contract Payment",
                                contract.Category,
                                Guid.NewGuid().ToString(),
                                contract.AccountID,
                                true,
                                contract.Cycle,
                                contract.ContractId
                            );
                            Transactions.Add(newTransaction);
                        }
                    }
                    nextDate = GetNextBillingDate(nextDate, contract.Cycle);
                }
            }
        }

        CalculateAccountBalances();
    }


    private DateTime GetNextBillingDate(DateTime current, BillingCycle cycle)
    {
        return cycle switch
        {
            BillingCycle.Daily => current.AddDays(1),
            BillingCycle.Weekly => current.AddDays(7),
            BillingCycle.Biweekly => current.AddDays(14),
            BillingCycle.Monthly => current.AddMonths(1),
            BillingCycle.Quarterly => current.AddMonths(3),
            BillingCycle.Annually => current.AddYears(1),
            _ => current
        };
    }

    public class ExpenseIncomeReport
    {
        public string[]? Categories { get; set; }
        public decimal[]? Expenses { get; set; }


        public ExpenseIncomeReport(string[] categories, decimal[] expenses) { Categories = categories; Expenses = expenses; }
    }



    public class FinancialReport
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public Dictionary<string, decimal> IncomeByCategory { get; set; } = new Dictionary<string, decimal>();
        public Dictionary<string, decimal> ExpensesByCategory { get; set; } = new Dictionary<string, decimal>();

        // public static void SplitExpensesByCategory()
        // {
        //     Categories = ExpensesByCategory.Keys.ToArray();
        //     Expenses = ExpensesByCategory.Values.ToArray();
        // }


        public static FinancialReport GenerateReport()
        {
            var report = new FinancialReport();

            foreach (var user in userList)
            {
                foreach (var transaction in user.Transactions)
                {
                    if (transaction.Type == "Income")
                    {
                        report.TotalIncome += transaction.Amount;
                        if (transaction.Category == null) continue;

                        if (!report.IncomeByCategory.ContainsKey(transaction.Category))
                        {
                            report.IncomeByCategory[transaction.Category] = 0;
                        }
                        report.IncomeByCategory[transaction.Category] += transaction.Amount;
                    }
                    else if (transaction.Type == "Expense")
                    {
                        report.TotalExpenses += transaction.Amount;
                        if (transaction.Category == null) continue;

                        if (transaction.Category != null && !report.ExpensesByCategory.ContainsKey(transaction.Category))
                        {
                            report.ExpensesByCategory[transaction.Category] = 0;
                        }
                        report.ExpensesByCategory[transaction.Category] += transaction.Amount;
                    }
                }
            }

            return report;
        }


        //Weekly Expense
        public static Dictionary<string, decimal>? GenerateWeeklyAccountExpenseReport(string userEmail, string accID, DateTime date)
        {
            Dictionary<string, decimal> expensesByCategory = new Dictionary<string, decimal>();
            var webUser = WebUser.getUserByEmail(userEmail);
            if (webUser == null) return null;

            long weekNumber = date.Ticks / 6048000000000;

            Console.WriteLine($"Calculating Weekly Expenses for Account: {accID} with starting date: {date} (Weeknumber: {weekNumber})");

            foreach (var transaction in webUser.Transactions)
            {
                long transactionWeekNumber = transaction.Date.Ticks / 6048000000000;
                Console.WriteLine($"transactionWeekNumber: {transactionWeekNumber} -- -- weekNumber: {weekNumber}");

                if (transactionWeekNumber == weekNumber)
                {
                    if (transaction.Type == "Expense")
                    {
                        if (transaction.Category == null) continue;

                        Console.WriteLine($"Processing transaction for category: {transaction.Category}, Amount: {transaction.Amount}");

                        if (!expensesByCategory.ContainsKey(transaction.Category))
                        {
                            expensesByCategory[transaction.Category] = 0;
                        }
                        expensesByCategory[transaction.Category] += transaction.Amount;
                    }
                }
            }

            Console.WriteLine($"Final categorized  Weekly expenses: {string.Join(", ", expensesByCategory.Select(kvp => $"{kvp.Key}: {kvp.Value}"))}");
            if (expensesByCategory == null)
            {
                Console.WriteLine("EIER");
            }
            return expensesByCategory;
        }

        //WeeklyIncome
        public static Dictionary<string, decimal>? GenerateWeeklyAccountIncomeReport(string userEmail, string accID, DateTime date)
        {
            Dictionary<string, decimal> incomeByCategory = new Dictionary<string, decimal>();
            var webUser = WebUser.getUserByEmail(userEmail);
            if (webUser == null) return null;

            long weekNumber = date.Ticks / 6048000000000;

            Console.WriteLine($"Calculating Weekly Expenses for Account: {accID} with starting date: {date} (Weeknumber: {weekNumber})");

            foreach (var transaction in webUser.Transactions)
            {
                long transactionWeekNumber = transaction.Date.Ticks / 6048000000000;
                Console.WriteLine($"transactionWeekNumber: {transactionWeekNumber} -- -- weekNumber: {weekNumber}");

                if (transactionWeekNumber == weekNumber)
                {
                    if (transaction.Type == "Income")
                    {
                        if (transaction.Category == null) continue;

                        Console.WriteLine($"Processing transaction for category: {transaction.Category}, Amount: {transaction.Amount}");

                        if (!incomeByCategory.ContainsKey(transaction.Category))
                        {
                            incomeByCategory[transaction.Category] = 0;
                        }
                        incomeByCategory[transaction.Category] += transaction.Amount;
                    }
                }
            }

            Console.WriteLine($"Final categorized Weekly income: {string.Join(", ", incomeByCategory.Select(kvp => $"{kvp.Key}: {kvp.Value}"))}");
            if (incomeByCategory.Count == 0)
            {
                Console.WriteLine("EIER");
            }
            return incomeByCategory;
        }

        //Monthly Expense
        public static Dictionary<string, decimal>? GenerateMonthlyAccountExpenseReport(string userEmail, string accID, DateTime date)
        {
            Dictionary<string, decimal> expenseByCategory = new Dictionary<string, decimal>();
            var webUser = WebUser.getUserByEmail(userEmail);
            if (webUser == null) return null;


            Console.WriteLine($"Calculating Monthly Expenses for Account: {accID} with starting date: {date} (date.Month: {date.Month})");

            foreach (var transaction in webUser.Transactions)
            {

                Console.WriteLine($"transaction Month: {transaction.Date.Month} -- -- date.Month: {date.Month}");

                if (transaction.Date.Month == date.Month && transaction.Date.Year == date.Year)
                {
                    if (transaction.Type == "Expense")
                    {
                        if (transaction.Category == null) continue;

                        Console.WriteLine($"Processing transaction for category: {transaction.Category}, Amount: {transaction.Amount}");

                        if (!expenseByCategory.ContainsKey(transaction.Category))
                        {
                            expenseByCategory[transaction.Category] = 0;
                        }
                        expenseByCategory[transaction.Category] += transaction.Amount;
                    }
                }
            }

            Console.WriteLine($"Final categorized Monthly expense: {string.Join(", ", expenseByCategory.Select(kvp => $"{kvp.Key}: {kvp.Value}"))}");
            if (expenseByCategory.Count == 0)
            {
                Console.WriteLine("EIER");
            }
            return expenseByCategory;
        }



        //Monthly Income
        public static Dictionary<string, decimal>? GenerateMonthlyAccountIncomeReport(string userEmail, string accID, DateTime date)
        {
            Dictionary<string, decimal> incomeByCategory = new Dictionary<string, decimal>();
            var webUser = WebUser.getUserByEmail(userEmail);
            if (webUser == null) return null;


            Console.WriteLine($"Calculating Monthly Expenses for Account: {accID} with starting date: {date} (date.Month: {date.Month})");

            foreach (var transaction in webUser.Transactions)
            {

                Console.WriteLine($"transaction Month: {transaction.Date.Month} -- -- date.Month: {date.Month}");

                if (transaction.Date.Month == date.Month && transaction.Date.Year == date.Year)
                {
                    if (transaction.Type == "Income")
                    {
                        if (transaction.Category == null) continue;

                        Console.WriteLine($"Processing transaction for category: {transaction.Category}, Amount: {transaction.Amount}");

                        if (!incomeByCategory.ContainsKey(transaction.Category))
                        {
                            incomeByCategory[transaction.Category] = 0;
                        }
                        incomeByCategory[transaction.Category] += transaction.Amount;
                    }
                }
            }

            Console.WriteLine($"Final categorized Monthly Income: {string.Join(", ", incomeByCategory.Select(kvp => $"{kvp.Key}: {kvp.Value}"))}");
            if (incomeByCategory.Count == 0)
            {
                Console.WriteLine("EIER");
            }
            return incomeByCategory;
        }




        //Yearly Expense
        public static Dictionary<string, decimal>? GenerateYearlyAccountExpenseReport(string userEmail, string accID, DateTime date)
        {
            Dictionary<string, decimal> expenseByCategory = new Dictionary<string, decimal>();
            var webUser = WebUser.getUserByEmail(userEmail);
            if (webUser == null) return null;


            Console.WriteLine($"Calculating Monthly Expenses for Account: {accID} with starting date: {date} (date.Month: {date.Year})");

            foreach (var transaction in webUser.Transactions)
            {
                Console.WriteLine($"transaction Month: {transaction.Date.Year} -- -- date.Month: {date.Year}");

                if (transaction.Date.Year == date.Year)
                {
                    if (transaction.Type == "Expense")
                    {
                        if (transaction.Category == null) continue;

                        Console.WriteLine($"Processing transaction for category: {transaction.Category}, Amount: {transaction.Amount}");

                        if (!expenseByCategory.ContainsKey(transaction.Category))
                        {
                            expenseByCategory[transaction.Category] = 0;
                        }
                        expenseByCategory[transaction.Category] += transaction.Amount;
                    }
                }
            }

            Console.WriteLine($"Final categorized yearly expense: {string.Join(", ", expenseByCategory.Select(kvp => $"{kvp.Key}: {kvp.Value}"))}");
            if (expenseByCategory.Count == 0)
            {
                Console.WriteLine("EIER");
            }
            return expenseByCategory;
        }




        //Yearly Income
        public static Dictionary<string, decimal>? GenerateYearlyAccountIncomeReport(string userEmail, string accID, DateTime date)
        {
            Dictionary<string, decimal> incomeByCategory = new Dictionary<string, decimal>();
            var webUser = WebUser.getUserByEmail(userEmail);
            if (webUser == null) return null;


            Console.WriteLine($"Calculating Monthly Expenses for Account: {accID} with starting date: {date} (date.Month: {date.Month})");

            foreach (var transaction in webUser.Transactions)
            {

                Console.WriteLine($"transaction Month: {transaction.Date.Year} -- -- date.Month: {date.Year}");

                if (transaction.Date.Year == date.Year)
                {
                    if (transaction.Type == "Income")
                    {
                        if (transaction.Category == null) continue;

                        Console.WriteLine($"Processing transaction for category: {transaction.Category}, Amount: {transaction.Amount}");

                        if (!incomeByCategory.ContainsKey(transaction.Category))
                        {
                            incomeByCategory[transaction.Category] = 0;
                        }
                        incomeByCategory[transaction.Category] += transaction.Amount;
                    }
                }
            }

            Console.WriteLine($"Final categorized Yearly Income: {string.Join(", ", incomeByCategory.Select(kvp => $"{kvp.Key}: {kvp.Value}"))}");
            if (incomeByCategory.Count == 0)
            {
                Console.WriteLine("EIER");
            }
            return incomeByCategory;
        }


    }





}




public class Session(string id, DateTime expiredate)
{
    public string Id { get; set; } = id;
    public DateTime ExpireDate { get; set; } = expiredate;

}


public class SessionUser(Session session, WebUser user)
{
    public WebUser User { get; set; } = user;
    public Session Session { get; set; } = session;

}




// public interface IFinancialAccount
// {
//     string AccountName { get; set; }
//     decimal Balance { get; set; }
// }

public abstract class FinancialAccount
{
    public string AccountName { get; set; } = "";
    public decimal Balance { get; set; } = 0;

    public string ID { get; set; } = "";


}

public class BankAccount : FinancialAccount
{
    public BankAccount() { }
    public BankAccount(string accountName, CurrencyType currency)
    {
        AccountName = accountName;
        Currency = currency;
    }
    public CurrencyType Currency { get; set; }


}

public class CashAccount : FinancialAccount
{
    public CashAccount() { }

    public CashAccount(string accountName, CurrencyType curreny)
    {
        AccountName = accountName;
        Currency = curreny;
    }
    public CurrencyType Currency { get; set; }


}

public enum CurrencyType
{
    [Description("US Dollar")]
    USD,
    [Description("EURO")]
    EUR,
    [Description("British Pound")]
    GBP,
    [Description("Australian Dollar")]
    AUD,
    [Description("Canadian Dollar")]
    CAD,
    [Description("Danish Krone")]
    DKK,
    [Description("Swedish Krona")]
    SEK,
    [Description("Singapore Dollar")]
    SGD,
    [Description("Russian Ruble")]
    RUB,
    [Description("Zloty")]
    PLN,
    [Description("New Zealand Dollar")]
    NZD,
    [Description("Swiss Franc")]
    CHF,
    [Description("Southkorean Won")]
    KRW,
    [Description("Yen")]
    JPY,
    [Description("Dong")]
    VND,
    [Description("Yuan Renminbi")]
    CNY


}


public class PortfolioAccount : FinancialAccount
{
    public PortfolioAccount() { }
    public PortfolioAccount(string accountName)
    {
        AccountName = accountName;
    }
    public List<Investment> Investments { get; set; } = [];

}

public class Investment
{
    public string Ticker { get; set; } = "";
    public int Quantity { get; set; }
    public decimal PurchasePrice { get; set; }
}

public class CryptoWallet : FinancialAccount
{
    public CryptoWallet() { }
    public CryptoWallet(string accountName)
    {
        AccountName = accountName;
    }
    public List<CryptoHolding> CryptoHoldings { get; set; } = [];


}

public class CryptoHolding
{

    public CryptoCoin Coin { get; set; }
    public decimal Amount { get; set; }
}


public enum CryptoCoin
{
    [Description("Bitcoin")]
    BTC,
    [Description("Etherium")]
    ETH,
    [Description("Solana")]
    SOL,
    [Description("Dodge Coin")]
    DOGE,
    [Description("Pepe")]
    PEPE,
    [Description("XRP")]
    XRP
}





public class Transaction
{

    public string Type { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string? Origin { get; set; }
    public string? Destination { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string ID { get; set; }
    public string AccountId { get; set; }
    public bool IsContract { get; set; }
    public BillingCycle? Cycle { get; set; }
    public string? ContractId { get; set; }
    public string? Ticker { get; set; }  // For stock transactions
    public CryptoCoin? Coin { get; set; }    // For crypto transactions


    public Transaction()
    {
        Type = "null";
        Date = DateTime.Now;
        Amount = 0;
        Origin = null;
        Destination = null;
        Description = null;
        Category = null;
        ID = "null";
        AccountId = "null";
        // IsContract = false;
        Cycle = (BillingCycle)1;
        ContractId = null;
    }
    //Constructor for everything
    public Transaction(string type, DateTime date, decimal amount, string? origin, string? destination, string? description, string? category, string id, string accountId, bool iscontract, BillingCycle cycle, string? contractId = null)
    {
        Type = type;
        Category = category;
        Amount = amount;
        Origin = origin;
        Destination = destination;
        Description = description;
        Date = date;
        ID = id;
        AccountId = accountId;
        IsContract = iscontract;
        Cycle = cycle;
        ContractId = contractId;
    }

    // Constructor for stock transactions
    public Transaction(string type, DateTime date, decimal amount, string ticker, string accountId)
    {
        Type = type;
        Date = date;
        Amount = amount;
        Ticker = ticker;
        AccountId = accountId;
    }

    // Constructor for crypto transactions
    public Transaction(string type, DateTime date, decimal amount, CryptoCoin coin, string accountId)
    {
        Type = type;
        Date = date;
        Amount = amount;
        Coin = coin;
        AccountId = accountId;
    }
}

public class Contract
{
    public string ContractId { get; set; }
    public decimal Amount { get; set; }
    public string Category { get; set; }
    public BillingCycle Cycle { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string AccountID { get; set; }
    public string Type { get; set; }

    public string? Origin { get; set; }
    public string? Destination { get; set; }


    public Contract(string type, string category, decimal amount, string accountID, BillingCycle cycle, DateTime startDate, string? origin, string? destination, DateTime? endDate = null)
    {
        ContractId = ContractId = Guid.NewGuid().ToString();
        Type = type;
        Category = category;
        AccountID = accountID;
        Amount = amount;
        Cycle = cycle;
        StartDate = startDate;
        EndDate = endDate;
        Origin = origin;
        Destination = destination;
    }
}

public enum BillingCycle
{
    Biannually,
    Annually,
    Semiannually,
    Quarterly,
    Bimonthly,
    Monthly,
    Biweekly,
    Weekly,
    Daily
}


public class SharedServices
{
    public void AddFinanceAccount(WebUser? webUser, string AccountType, CurrencyType currency, string AccountName)
    {


        if (AccountType == "BankAccount")
        {
            var newBankAccount = new BankAccount
            (
                AccountName,
                currency
            );

            newBankAccount.ID = System.Guid.NewGuid().ToString();

            webUser?.BankAccounts.Add(newBankAccount);
        }
        else if (AccountType == "Cash")
        {
            var newCashAccount = new CashAccount
            (
                AccountName,
                currency
            );

            newCashAccount.ID = System.Guid.NewGuid().ToString();

            webUser?.CashAccounts.Add(newCashAccount);

        }
        else if (AccountType == "Portfolio")
        {
            var newPortfolio = new PortfolioAccount
            (
                AccountName
            );

            newPortfolio.ID = System.Guid.NewGuid().ToString();

            webUser?.PortfolioAccounts.Add(newPortfolio);
        }
        else if (AccountType == "CryptoWallet")
        {
            var newCryptoWallet = new CryptoWallet
            (
                AccountName
            );

            newCryptoWallet.ID = System.Guid.NewGuid().ToString();

            webUser?.CryptoWallets.Add(newCryptoWallet);
        }
    }






}




public static class ExchangeRateProvider
{
    // Define static exchange rates (example rates, you should use actual rates)
    private static readonly Dictionary<(CurrencyType, CurrencyType), decimal> ExchangeRates = new Dictionary<(CurrencyType, CurrencyType), decimal>
    {
        {(CurrencyType.USD, CurrencyType.EUR), 0.85m},
        {(CurrencyType.EUR, CurrencyType.USD), 1.18m},
        {(CurrencyType.USD, CurrencyType.GBP), 0.74m},
        {(CurrencyType.GBP, CurrencyType.USD), 1.35m},
        // Add more exchange rates as needed
    };

    private static readonly Dictionary<(CurrencyType, CryptoCoin), decimal> CurrencyToCryptoRates = new Dictionary<(CurrencyType, CryptoCoin), decimal>
    {
        {(CurrencyType.USD, CryptoCoin.BTC), 0.000021m},
        {(CurrencyType.USD, CryptoCoin.ETH), 0.00031m},
        // Add more currency to crypto rates as needed
    };

    private static readonly Dictionary<(CryptoCoin, CurrencyType), decimal> CryptoToCurrencyRates = new Dictionary<(CryptoCoin, CurrencyType), decimal>
    {
        {(CryptoCoin.BTC, CurrencyType.USD), 47000m},
        {(CryptoCoin.ETH, CurrencyType.USD), 3200m},
        // Add more crypto to currency rates as needed
    };

    public static decimal GetExchangeRate(CurrencyType fromCurrency, CurrencyType toCurrency)
    {
        if (fromCurrency == toCurrency)
        {
            return 1;
        }

        return ExchangeRates[(fromCurrency, toCurrency)];
    }

    public static decimal GetCurrencyToCryptoRate(CurrencyType fromCurrency, CryptoCoin toCrypto)
    {
        return CurrencyToCryptoRates[(fromCurrency, toCrypto)];
    }

    public static decimal GetCryptoToCurrencyRate(CryptoCoin fromCrypto, CurrencyType toCurrency)
    {
        return CryptoToCurrencyRates[(fromCrypto, toCurrency)];
    }
}



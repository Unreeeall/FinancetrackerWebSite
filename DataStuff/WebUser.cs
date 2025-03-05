using System.Text.Json;
using System.Globalization;
using Database;
using Transactions;
using SuperUsefullOrSo;





//Very Importaint Comment
namespace FinanceUser
{
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

        public static List<Transaction> GetCurrentUserTransactions(WebUser? webUser)
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

        public Transaction? GetTransactionByID(string? transactionID)
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


        public List<Transaction> GetTransactionsByAccID(string? accountID)
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


        public Contract? GetContractByID(string? contractId)
        {
            foreach (var contract in Contracts)
            {
                if (contract.ContractId == contractId) return contract;
            }
            return null;
        }
        public void UpdateUser(string? email = null, string? name = null, string? phonenumber = null, string? password = null)
        {
            if (!string.IsNullOrEmpty(email)) Email = email;
            if (!string.IsNullOrEmpty(name)) Name = name;
            if (!string.IsNullOrEmpty(phonenumber)) Phonenumber = phonenumber;
            if (!string.IsNullOrEmpty(password)) Password = password;
            saveJson();
        }

        public void UpdateBankAccount(BankAccount? bankAccount, string? name)
        {
            if (bankAccount != null)
                if (!string.IsNullOrEmpty(name)) bankAccount.AccountName = name;
        }

        public void UpdateCashAccount(CashAccount? cashAccount, string? name)
        {
            if (cashAccount != null)
                if (!string.IsNullOrEmpty(name)) cashAccount.AccountName = name;
        }

        public void UpdatePortfolioAccount(PortfolioAccount? portfolioAccount, string? name)
        {
            if (portfolioAccount != null)
                if (!string.IsNullOrEmpty(name)) portfolioAccount.AccountName = name;
        }

        public void UpdateCryptoWallet(CryptoWallet? cryptoWallet, string? name)
        {
            if (cryptoWallet == null)
            {
                Console.WriteLine("UpdateCryptoWallet -> CryptoWallet Is emtpy");
            }
            else
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

        public BankAccount? GetBankAccountByID(string? accountID)
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

        public CashAccount? GetCashAccountByID(string? accountID)
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

        public PortfolioAccount? GetPortfolioAccountByID(string? accountID)
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

        public CryptoWallet? GetCryptoWalletByID(string? accountID)
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


        public string GetAccountNameById(string? accountID)
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


            foreach (var transaction in sortedTransactions)
            {
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


        public decimal CalculateAccIncomeForTimeframe(DateTime date, string accID, string timeframe)
        {
            decimal totalIncome = 0;

            long weekNumber = date.Ticks / 6048000000000;

            foreach (var transaction in Transactions)
            {

                bool isInTimeframe = false;

                switch (timeframe.ToLower())
                {
                    case "week":
                        long transactionWeekNumber = transaction.Date.Ticks / 6048000000000;
                        isInTimeframe = transactionWeekNumber == weekNumber && transaction.AccountId == accID;
                        break;
                    case "month":
                        isInTimeframe = transaction.Date.Month == date.Month && transaction.Date.Year == date.Year && transaction.AccountId == accID;
                        break;
                    case "year":
                        isInTimeframe = transaction.Date.Year == date.Year && transaction.AccountId == accID;
                        break;
                    default:
                        Console.WriteLine("Invalid timeframe specified.");
                        return 0;
                }

                if (isInTimeframe && transaction.Type == "Income")
                {
                    totalIncome += transaction.Amount;
                    Console.WriteLine("TotalIncome for timeframe: " + timeframe + " " + totalIncome);
                }
            }
            return totalIncome;
        }

        public decimal CalculateAccExpenseForTimeframe(DateTime date, string accID, string timeframe)
        {
            decimal totalIncome = 0;

            long weekNumber = date.Ticks / 6048000000000;

            foreach (var transaction in Transactions)
            {
                bool isInTimeframe = false;

                switch (timeframe.ToLower())
                {
                    case "week":
                        long transactionWeekNumber = transaction.Date.Ticks / 6048000000000;
                        isInTimeframe = transactionWeekNumber == weekNumber && transaction.AccountId == accID;
                        break;
                    case "month":
                        isInTimeframe = transaction.Date.Month == date.Month && transaction.Date.Year == date.Year && transaction.AccountId == accID;
                        break;
                    case "year":
                        isInTimeframe = transaction.Date.Year == date.Year && transaction.AccountId == accID;
                        break;
                    default:
                        Console.WriteLine("Invalid timeframe specified.");
                        return 0;
                }

                if (isInTimeframe && transaction.Type == "Expense")
                {
                    totalIncome += transaction.Amount;
                    Console.WriteLine("TotalExpense for timeframe: " + timeframe + " " + totalIncome);
                }
            }
            return totalIncome;
        }


        public decimal CalculatAccTransferIncomeForTimeFrame(DateTime date, string accID, string timeframe)
        {
            decimal TotalTransferAmount = 0;

            long weekNumber = date.Ticks / 6048000000000;

            foreach (var transaction in Transactions)
            {
                bool isInTimeframe = false;

                switch (timeframe.ToLower())
                {
                    case "week":
                        long transactionWeekNumber = transaction.Date.Ticks / 6048000000000;
                        isInTimeframe = transactionWeekNumber == weekNumber && transaction.AccountId == accID;
                        break;
                    case "month":
                        isInTimeframe = transaction.Date.Month == date.Month && transaction.Date.Year == date.Year && transaction.AccountId == accID;
                        break;
                    case "year":
                        isInTimeframe = transaction.Date.Year == date.Year && transaction.AccountId == accID;
                        break;
                    default:
                        Console.WriteLine("Invalid timeframe specified.");
                        return 0;
                }
                if (isInTimeframe && transaction.Type == "Transfer")
                {
                    if (transaction.Destination == accID)
                    {
                        TotalTransferAmount += transaction.Amount;
                    }
                }
            }
            return TotalTransferAmount;
        }

        public decimal CalculatAccTransferExpenseForTimeFrame(DateTime date, string accID, string timeframe)
        {
            decimal TotalTransferAmount = 0;

            long weekNumber = date.Ticks / 6048000000000;

            foreach (var transaction in Transactions)
            {
                bool isInTimeframe = false;

                switch (timeframe.ToLower())
                {
                    case "week":
                        long transactionWeekNumber = transaction.Date.Ticks / 6048000000000;
                        isInTimeframe = transactionWeekNumber == weekNumber && transaction.AccountId == accID;
                        break;
                    case "month":
                        isInTimeframe = transaction.Date.Month == date.Month && transaction.Date.Year == date.Year && transaction.AccountId == accID;
                        break;
                    case "year":
                        isInTimeframe = transaction.Date.Year == date.Year && transaction.AccountId == accID;
                        break;
                    default:
                        Console.WriteLine("Invalid timeframe specified.");
                        return 0;
                }
                if (isInTimeframe && transaction.Type == "Transfer")
                {
                    if (transaction.Origin == accID)
                    {
                        TotalTransferAmount += transaction.Amount;
                    }
                }
            }
            return TotalTransferAmount;
        }

        public decimal CalculateMonthlyTotalPlus(DateTime date, string accID, string timeFrame)
        {
            decimal Income = CalculateAccIncomeForTimeframe(date, accID, timeFrame);
            decimal Expense = CalculateAccExpenseForTimeframe(date, accID, timeFrame);
            decimal TransferInc = CalculatAccTransferIncomeForTimeFrame(date, accID, timeFrame);
            decimal TransferExp = CalculatAccTransferExpenseForTimeFrame(date, accID, timeFrame);

            decimal TotalPlus = Income + TransferInc - (Expense + TransferExp);

            return TotalPlus;
        }

        public decimal CalculateMonthlyTotalMinus(DateTime date, string accID, string timeFrame)
        {
            decimal Expense = CalculateAccExpenseForTimeframe(date, accID, timeFrame);
            decimal TransferExp = CalculatAccTransferExpenseForTimeFrame(date, accID, timeFrame);

            decimal TotalPlus = Expense + TransferExp;

            return TotalPlus;
        }


        public decimal[]? GetDailyAccIncome(DateTime date, string accID)
        {
            Console.WriteLine($"Calculating WeeklyDaily Account balance for Account: {accID} with starting date: {date}");

            List<Transaction> sortedTransactions = Transactions.OrderBy(o => o.Date).ToList();
            decimal[] dailyIncome = new decimal[7];
            int dayOfWeek;
            TimeSpan daySinceFirstDayOfWeek;

            foreach (var transaction in sortedTransactions)
            {
                daySinceFirstDayOfWeek = transaction.Date.Date - date.Date;
                dayOfWeek = (int)daySinceFirstDayOfWeek.TotalDays;

                if (dayOfWeek >= 0 && dayOfWeek < 7)
                {
                    if (transaction.AccountId == accID && transaction.Type == "Income")
                    {
                        dailyIncome[dayOfWeek] += transaction.Amount;
                    }
                    else if (transaction.Type == "Transfer" && transaction.Destination == accID)
                    {
                        dailyIncome[dayOfWeek] += transaction.Amount;
                    }
                }
            }

            return dailyIncome;
        }

        public decimal[]? GetDailyAccExpense(DateTime date, string accID)
        {
            Console.WriteLine($"Calculating WeeklyDaily Account balance for Account: {accID} with starting date: {date}");

            List<Transaction> sortedTransactions = Transactions.OrderBy(o => o.Date).ToList();
            decimal[] dailyExpense = new decimal[7];
            int dayOfWeek;
            TimeSpan daySinceFirstDayOfWeek;

            foreach (var transaction in sortedTransactions)
            {
                daySinceFirstDayOfWeek = transaction.Date.Date - date.Date;
                dayOfWeek = (int)daySinceFirstDayOfWeek.TotalDays;

                if (dayOfWeek >= 0 && dayOfWeek < 7)
                {
                    if (transaction.AccountId == accID && transaction.Type == "Expense")
                    {
                        dailyExpense[dayOfWeek] += transaction.Amount;
                    }
                    else if (transaction.Type == "Transfer" && transaction.Origin == accID)
                    {
                        dailyExpense[dayOfWeek] += transaction.Amount;
                    }
                }
            }

            return dailyExpense;
        }

        public decimal[] GetMonthlyAccIncome(DateTime date, string accID)
        {
            DateTime firstDateOfMonth = new DateTime(date.Year, date.Month, 1);
            Console.WriteLine($"Calculating MonthlyDaily Account balance for Account: {accID} with starting date: {firstDateOfMonth}");

            List<Transaction> sortedTransactions = Transactions.OrderBy(o => o.Date).ToList();
            int daysInMonth = DateTime.DaysInMonth(firstDateOfMonth.Year, firstDateOfMonth.Month);
            decimal[] dailyIncome = new decimal[daysInMonth];
            int dayOfMonth;

            foreach (var transaction in sortedTransactions)
            {
                dayOfMonth = (transaction.Date.Date - firstDateOfMonth.Date).Days;

                if (dayOfMonth >= 0 && dayOfMonth < daysInMonth)
                {
                    if (transaction.AccountId == accID && transaction.Type == "Income")
                    {
                        dailyIncome[dayOfMonth] += transaction.Amount;
                    }
                    else if (transaction.Type == "Transfer" && transaction.Destination == accID)
                    {
                        dailyIncome[dayOfMonth] += transaction.Amount;
                    }
                }
            }
            return dailyIncome;
        }

        public decimal[] GetMonthlyAccExpense(DateTime date, string accID)
        {
            DateTime firstDateOfMonth = new DateTime(date.Year, date.Month, 1);
            Console.WriteLine($"Calculating MonthlyDaily Account balance for Account: {accID} with starting date: {firstDateOfMonth}");

            List<Transaction> sortedTransactions = Transactions.OrderBy(o => o.Date).ToList();
            int daysInMonth = DateTime.DaysInMonth(firstDateOfMonth.Year, firstDateOfMonth.Month);
            decimal[] dailyExpense = new decimal[daysInMonth];
            int dayOfMonth;

            foreach (var transaction in sortedTransactions)
            {
                dayOfMonth = (transaction.Date.Date - firstDateOfMonth.Date).Days;

                if (dayOfMonth >= 0 && dayOfMonth < daysInMonth)
                {
                    if (transaction.AccountId == accID && transaction.Type == "Expense")
                    {
                        dailyExpense[dayOfMonth] += transaction.Amount;
                    }
                    else if (transaction.Type == "Transfer" && transaction.Origin == accID)
                    {
                        dailyExpense[dayOfMonth] += transaction.Amount;
                    }
                }
            }
            return dailyExpense;
        }

        public decimal[] GetYearlyAccIncome(DateTime date, string accID)
        {
            Console.WriteLine($"Calculating YearlyMonthly Account balance for Account: {accID} with starting date: {date}");

            List<Transaction> sortedTransactions = Transactions.OrderBy(o => o.Date).ToList();
            decimal[] monthlyIncome = new decimal[12];
            int monthOfYear;

            foreach (var transaction in sortedTransactions)
            {
                monthOfYear = transaction.Date.Month - 1;

                if (transaction.Date.Year == date.Year && monthOfYear >= 0 && monthOfYear < 12)
                {
                    if (transaction.AccountId == accID && transaction.Type == "Income")
                    {
                        monthlyIncome[monthOfYear] += transaction.Amount;
                    }
                    else if (transaction.Type == "Transfer" && transaction.Destination == accID)
                    {
                        monthlyIncome[monthOfYear] += transaction.Amount;
                    }
                }
            }
            return monthlyIncome;
        }

        public decimal[] GetYearlyAccExpense(DateTime date, string accID)
        {
            Console.WriteLine($"Calculating YearlyMonthly Account balance for Account: {accID} with starting date: {date}");

            List<Transaction> sortedTransactions = Transactions.OrderBy(o => o.Date).ToList();
            decimal[] monthlyExpense = new decimal[12];
            int monthOfYear;

            foreach (var transaction in sortedTransactions)
            {
                monthOfYear = transaction.Date.Month - 1;

                if (transaction.Date.Year == date.Year && monthOfYear >= 0 && monthOfYear < 12)
                {
                    if (transaction.AccountId == accID && transaction.Type == "Expense")
                    {
                        monthlyExpense[monthOfYear] += transaction.Amount;
                    }
                    else if (transaction.Type == "Transfer" && transaction.Origin == accID)
                    {
                        monthlyExpense[monthOfYear] += transaction.Amount;
                    }
                }
            }
            return monthlyExpense;
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
                                var newTransaction = new Transaction
                                {
                                    Type = contract.Type,
                                    Date = nextDate,
                                    Amount = contract.Amount,
                                    Origin = contract.Origin,
                                    Destination = contract.Destination,
                                    Description = "Contract Payment",
                                    Category = contract.Category,
                                    ID = Guid.NewGuid().ToString(),
                                    AccountId = contract.AccountID,
                                    IsContract = true,
                                    Cycle = contract.Cycle,
                                    ContractId = contract.ContractId
                                };
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

        public void DeleteContract(string contractId)
        {
            var contractToRemove = GetContractByID(contractId);
            if (contractToRemove == null) Console.WriteLine("DeleteContract -> contractToRemove is null!");
            else
                Contracts.Remove(contractToRemove);
        }


        public void DeleteAllContractTransactions(string contractId, string contractAccId)
        {
            List<Transaction> transactionsToDelete = [];
            foreach (var transaction in Transactions)
            {
                if (transaction.ContractId == contractId)
                {
                    if (transaction.AccountId == contractAccId)
                    {
                        transactionsToDelete.Add(transaction);
                    }
                }
            }



            foreach (var transaction in transactionsToDelete)
            {
                Console.WriteLine(transaction.ID);
                Transactions.Remove(transaction);
            }
        }

        public void UpdateAllContractTransactions(string contractId, string contractAccId, string? type = null, string? category = null, decimal? amount = null, string? destination = null, string? origin = null, string? ticker = null, CryptoCoin? coin = null)
        {
            foreach (var transaction in Transactions)
            {
                if (transaction.ContractId == contractId)
                {
                    if (transaction.AccountId == contractAccId)
                    {
                        if (!string.IsNullOrEmpty(type)) transaction.Type = type;
                        if (!string.IsNullOrEmpty(category)) transaction.Category = category;
                        if (amount != null) transaction.Amount = (decimal)amount;
                        if (!string.IsNullOrEmpty(destination)) transaction.Destination = destination;
                        if (!string.IsNullOrEmpty(origin)) transaction.Origin = origin;
                        if (!string.IsNullOrEmpty(ticker)) transaction.Ticker = ticker;
                        if (coin != null) transaction.Coin = (CryptoCoin)coin;
                    }
                }
            }

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


            public static Dictionary<string, decimal>? GenerateAccountExpenseReport(string userEmail, string accID, DateTime date, string timeframe)
            {
                Dictionary<string, decimal> expensesByCategory = new Dictionary<string, decimal>();
                var webUser = WebUser.getUserByEmail(userEmail);
                if (webUser == null) return null;

                long weekNumber = date.Ticks / 6048000000000;


                foreach (var transaction in webUser.Transactions)
                {
                    bool isInTimeframe = false;

                    switch (timeframe.ToLower())
                    {
                        case "week":
                            long transactionWeekNumber = transaction.Date.Ticks / 6048000000000;
                            isInTimeframe = transactionWeekNumber == weekNumber && transaction.AccountId == accID;
                            break;
                        case "month":
                            isInTimeframe = transaction.Date.Month == date.Month && transaction.Date.Year == date.Year && transaction.AccountId == accID;
                            break;
                        case "year":
                            isInTimeframe = transaction.Date.Year == date.Year && transaction.AccountId == accID;
                            break;
                        default:
                            Console.WriteLine("Invalid timeframe specified.");
                            return null;
                    }
                    if (isInTimeframe && transaction.Type == "Expense")
                    {
                        if (transaction.Type == "Expense")
                        {
                            if (transaction.Category == null) continue;

                            if (!expensesByCategory.ContainsKey(transaction.Category))
                            {
                                expensesByCategory[transaction.Category] = 0;
                            }
                            expensesByCategory[transaction.Category] += transaction.Amount;
                        }
                    }
                }

                if (expensesByCategory == null)
                {
                    Console.WriteLine("EIER");
                }

                return expensesByCategory;
            }

            public static Dictionary<string, decimal>? GenerateAccountIncomeReport(string userEmail, string accID, DateTime date, string timeframe)
            {
                Dictionary<string, decimal> incomeByCategory = new Dictionary<string, decimal>();
                var webUser = WebUser.getUserByEmail(userEmail);
                if (webUser == null) return null;

                long weekNumber = date.Ticks / 6048000000000;

                Console.WriteLine($"Calculating Income by Category for Account: {accID} with starting date: {date} (Timeframe: {timeframe})");

                foreach (var transaction in webUser.Transactions)
                {
                    bool isInTimeframe = false;

                    switch (timeframe.ToLower())
                    {
                        case "week":
                            long transactionWeekNumber = transaction.Date.Ticks / 6048000000000;
                            isInTimeframe = transactionWeekNumber == weekNumber && transaction.AccountId == accID;
                            break;
                        case "month":
                            isInTimeframe = transaction.Date.Month == date.Month && transaction.Date.Year == date.Year && transaction.AccountId == accID;
                            break;
                        case "year":
                            isInTimeframe = transaction.Date.Year == date.Year && transaction.AccountId == accID;
                            break;
                        default:
                            Console.WriteLine("Invalid timeframe specified.");
                            return null;
                    }

                    if (isInTimeframe && transaction.Type == "Income")
                    {
                        if (transaction.Category == null) continue;

                        if (!incomeByCategory.ContainsKey(transaction.Category))
                        {
                            incomeByCategory[transaction.Category] = 0;
                        }
                        incomeByCategory[transaction.Category] += transaction.Amount;
                    }
                }

                Console.WriteLine($"Final categorized Income: {string.Join(", ", incomeByCategory.Select(kvp => $"{kvp.Key}: {kvp.Value}"))}");

                if (incomeByCategory.Count == 0)
                {
                    Console.WriteLine("EIER");
                }
                return incomeByCategory;
            }
        }
    }
}

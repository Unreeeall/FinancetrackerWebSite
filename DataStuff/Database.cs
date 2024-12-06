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

    public static WebUser? getUserByEmail(string email)
    {
        Console.WriteLine($"Email--: {email}");
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
                                null,
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
                        if (report.IncomeByCategory.ContainsKey(transaction.Category))

                            if (transaction.Category != null && !report.IncomeByCategory.ContainsKey(transaction.Category))
                            {
                                report.IncomeByCategory[transaction.Category] = 0;
                            }
                        report.IncomeByCategory[transaction.Category] += transaction.Amount;
                    }
                    else if (transaction.Type == "Expense")
                    {
                        report.TotalExpenses += transaction.Amount;

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
    public BillingCycle Cycle { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string AccountID { get; set; }
    public string Type { get; set; }

    public string? Origin { get; set; }
    public string? Destination { get; set; }


    public Contract(string type, decimal amount, string accountID, BillingCycle cycle, DateTime startDate, string? origin, string? destination, DateTime? endDate = null)
    {
        ContractId = ContractId = Guid.NewGuid().ToString();
        Type = type;
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



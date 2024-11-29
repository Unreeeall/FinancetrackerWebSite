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




public class WebUser
{
    public string Email { get; set; }
    public string Name { get; set; }
    public string? Phonenumber { get; set; }
    public string Password { get; set; }

    public List<Session> Sessions { get; set; } = [];

    static Dictionary<string, SessionUser> IdDict { get; set; } = [];

    public List<Transaction> Transactions { get; set; } = [];
    public List<BankAccount> BankAccounts { get; set; } = [];
    public List<CashAccount> CashAccounts { get; set; } = [];
    public List<PortfolioAccount> PortfolioAccounts { get; set; } = [];
    public List<CryptoWallet> CryptoWallets { get; set; } = [];

    private static string filePath = Path.Combine(Directory.GetCurrentDirectory(), "UserData.json");

    private static List<WebUser> userList = new List<WebUser>();


    // public void AddBankAccount(BankAccount account)
    // {
    //     BankAccounts.Add(account);
    // }

    // public void AddPortfolioAccount(PortfolioAccount account)
    // {
    //     PortfolioAccounts.Add(account);
    // }

    // public void AddCryptoWallet(CryptoWallet wallet)
    // {
    //     CryptoWallets.Add(wallet);
    // }

    // public List<IFinancialAccount> GetAllFinancialAccounts()
    // {
    //     List<IFinancialAccount> accounts = new List<IFinancialAccount>();
    //     accounts.AddRange(BankAccounts);
    //     accounts.AddRange(PortfolioAccounts);
    //     accounts.AddRange(CryptoWallets);
    //     return accounts;
    // }



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



    public bool HasFinancialAccounts()
    {
        if(BankAccounts.Count == 0 && PortfolioAccounts.Count == 0 && CryptoWallets.Count == 0)
        {
            return false;
        }
        else return true;
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

}

public class BankAccount : FinancialAccount
{
    public BankAccount(string accountName, CurrencyType currency)
    {
        AccountName = accountName;
        Currency = currency;
    }
    public CurrencyType Currency { get; set; }
}

public class CashAccount : FinancialAccount
{
    public CashAccount() {}

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

    public string? Type { get; set; }
    public DateTime Date { get; set; }
    public decimal? Amount { get; set; }

    public FinancialAccount? Origin { get; set; }
    public FinancialAccount? Destination { get; set; }
    public string? Description { get; set; }
    public string? UseCase { get; set; }
    public string? Category { get; set; }
    public string? SenderName { get; set; }
    public string? SenderAccount { get; set; }
    public bool IsIncoming { get; set; }
    public string? ID { get; set; }

    
    public Transaction()
    {
        Type = "EMPTY";
        Category = "EMPTY";
        UseCase = "EMPTY";
        Amount = 0;
        Origin = null;
        Destination = null;
        SenderName = "EMPTY";
        SenderAccount ="EMTPY";
        Description = "EMPTY";
        Date = DateTime.Now;
        ID = "EMPTY";
        IsIncoming = false;
        
    }

    public Transaction(string type, DateTime date, decimal amount, FinancialAccount origin, FinancialAccount destination, string? description, string useCase, string category, string senderName, string senderAccount,bool isincoming, string id)
    {
        Type = type;
        Category = category;
        UseCase = useCase;
        Amount = amount;
        Origin = origin;
        Destination = destination;
        SenderName = senderName;
        SenderAccount = senderAccount;
        Description = description;
        Date = date;
        IsIncoming = isincoming;
        ID = id;
    }
    /*
    public string GetDate() {
        return Date.ToString("s");
    }
    */

    

    


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

                webUser?.BankAccounts.Add(newBankAccount);
            }
            else if (AccountType == "Cash")
            {
                var newCashAccount = new CashAccount
                (
                    AccountName,
                    currency
                );

                webUser?.CashAccounts.Add(newCashAccount);

            }
            else if (AccountType == "Portfolio")
            {
                var newPortfolio = new PortfolioAccount
                (
                    AccountName
                );

                webUser?.PortfolioAccounts.Add(newPortfolio);
            }
            else if (AccountType == "CryptoWallet")
            {
                var newCryptoWallet = new CryptoWallet
                (
                    AccountName
                );

                webUser?.CryptoWallets.Add(newCryptoWallet);
            }
    }
}


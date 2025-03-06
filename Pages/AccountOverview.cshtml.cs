using Database;
using FinanceUser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;
using Transactions;

namespace FinanceTracker.Pages;

public class AccountOverviewModel : PageModel
{



    public WebUser? WebUser { get; set; }
    public FinancialAccount? financialAccount { get; set; }

    [BindProperty]
    public string? UuId { get; set; }

    private readonly string SessionCookieName = "SessionCookie";
    private readonly string IndexPage = "/Index";
    private readonly string ErrorPage = "/Error";

    private readonly string AccountOverviewPage = "/AccountOverview";



    [BindProperty]
    public required string AccountType { get; set; }

    [BindProperty]
    public required string AccountName { get; set; }

    [BindProperty]
    public CurrencyType Currency { get; set; }


    [BindProperty]
    public required string AccID { get; set; }

    [BindProperty]
    public required string Type { get; set; }

    [BindProperty]
    public required string Origin { get; set; } = "";

    [BindProperty]
    public required string Destination { get; set; } = "";

    [BindProperty]
    public required string Category { get; set; }

    [BindProperty]
    public required string Description { get; set; }

    [BindProperty]
    public required string Amount { get; set; }

    [BindProperty]
    public required DateTime Date { get; set; }

    [BindProperty]
    public DateTime? EndDate { get; set; }

    [BindProperty]
    public bool IsContract { get; set; }

    [BindProperty]
    public BillingCycle Cycle { get; set; }

    [BindProperty]
    public bool IsFinished { get; set; }


    [BindProperty]
    public required string TransID { get; set; }

    [BindProperty]
    public required string ContractID { get; set; }

    [BindProperty]
    public required string ContractAccID { get; set; }

    public string? Ticker { get; set; }

    public CryptoCoin Coin { get; set; }



    public IActionResult OnGet()
    {

        try
        {
            WebUser = null;
            if (!Request.Cookies.TryGetValue(SessionCookieName, out string? sessionId)) return RedirectToPage(IndexPage);
            if (sessionId == null) return RedirectToPage(IndexPage);
            WebUser = WebUser.GetUserBySession(sessionId);
            if (WebUser == null) return RedirectToPage(IndexPage);
            if (!WebUser.HasFinancialAccounts()) return RedirectToPage("/Dashboard");

            string accountID = HttpContext.Request.Query["uuid"].ToString();
            if (accountID == null)
            {
                Console.WriteLine("accountID from query is EMPTY!!");
                return RedirectToPage(ErrorPage);
            }
            Console.WriteLine("Account ID from query: " + accountID);


            financialAccount = WebUser.GetAccountByID(accountID);
            if (financialAccount == null)
            {
                Console.WriteLine("FinancialAccount is empty!!");
                return RedirectToPage(ErrorPage);
            }
            else
            {
                switch (financialAccount)
                {
                    case BankAccount bankAccount:
                        financialAccount = WebUser.GetBankAccountByID(accountID);
                        break;
                    case CashAccount cashAccount:
                        financialAccount = WebUser.GetCashAccountByID(accountID);
                        break;
                    case CryptoWallet cryptoWallet:
                        financialAccount = WebUser.GetCryptoWalletByID(accountID);
                        break;
                    case PortfolioAccount portfolioAccount:
                        financialAccount = WebUser.GetPortfolioAccountByID(accountID);
                        break;
                    default:
                        return RedirectToPage(ErrorPage);
                }
            }





            Console.WriteLine("Account ID: " + accountID);
            return Page();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in OnGet: {ex.Message}");
            return RedirectToPage(ErrorPage);
        }


    }


    public IActionResult OnPostAddTransaction()
    {
        try
        {
            WebUser = null;
            if (!Request.Cookies.TryGetValue(SessionCookieName, out string? sessionId)) return RedirectToPage(IndexPage);
            if (sessionId == null) return RedirectToPage(IndexPage);
            WebUser = WebUser.GetUserBySession(sessionId);
            if (WebUser == null) return RedirectToPage(IndexPage);

            if (decimal.TryParse(Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal parsedAmount))
            {

                if (IsContract)
                {

                    var newContract = new Contract
                    {
                        ContractId = Guid.NewGuid().ToString(),
                        Type = Type,
                        Category = Category,
                        Amount = parsedAmount,
                        AccountID = AccID,
                        Cycle = Cycle,
                        StartDate = Date,
                        Origin = Origin,
                        Destination = Destination,
                        EndDate = EndDate
                    };
                    WebUser.Contracts.Add(newContract);
                }
                else
                {
                    var newTransaction = new Transaction
                    {
                    Type = Type,
                    Date = Date,
                    Amount = parsedAmount,
                    Origin = Origin,
                    Destination = Destination,
                    Description = Description,
                    Category = Category,
                    ID = System.Guid.NewGuid().ToString(),
                    AccountId = AccID,
                    IsContract = IsContract,
                    Cycle = Cycle
                    };
                    WebUser.Transactions.Add(newTransaction);
                }
            }
            return RedirectToPage(AccountOverviewPage, new { uuid = UuId });

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding Transaction: {ex.Message}");
            return RedirectToPage(ErrorPage);
        }
    }

    public IActionResult OnPostEditTransaction()
    {
        WebUser = null;
        if (!Request.Cookies.TryGetValue(SessionCookieName, out string? sessionId)) return RedirectToPage(IndexPage);
        if (sessionId == null) return RedirectToPage(IndexPage);
        WebUser = WebUser.GetUserBySession(sessionId);
        if (WebUser == null) return RedirectToPage(IndexPage);


        if (TransID != null)
        {

            var currentTransaction = WebUser.GetTransactionByID(TransID);
            if (currentTransaction == null)
            {
                Console.WriteLine("CurrentTransaction is empty!");
                return RedirectToPage(AccountOverviewPage, new { uuid = UuId });
            }
            else
            {
                if (decimal.TryParse(Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal parsedAmount))
                {
                                       
                    currentTransaction.Type = Type;
                    currentTransaction.Date = Date;
                    currentTransaction.Amount = parsedAmount;
                    currentTransaction.Origin = Origin;
                    currentTransaction.Destination = Destination;
                    currentTransaction.Description = Description;
                    currentTransaction.Category = Category;
                    currentTransaction.ID = TransID;
                    currentTransaction.AccountId = AccID;
                    currentTransaction.IsContract = IsContract;
                    currentTransaction.Cycle = Cycle;

                    var transactionToRemove = WebUser.GetTransactionByID(TransID);
                    if(transactionToRemove == null)Console.WriteLine("OnPostEditTransaction -> transactionToRemove is null!");
                    else
                    {
                        WebUser.Transactions.Remove(transactionToRemove);
                        WebUser.Transactions.Add(currentTransaction);
                        Console.WriteLine($"Transaction with ID: {TransID} deleted!");
                    }   
                    
                }
            }
        }
        return RedirectToPage(AccountOverviewPage, new { uuid = UuId });
    }


    public IActionResult OnPostDeleteTransaction()
    {
        try
        {
            WebUser = null;
            if (!Request.Cookies.TryGetValue(SessionCookieName, out string? sessionId)) return RedirectToPage(IndexPage);
            if (sessionId == null) return RedirectToPage(IndexPage);
            WebUser = WebUser.GetUserBySession(sessionId);
            if (WebUser == null) return RedirectToPage(IndexPage);

            Console.WriteLine("UUID: " + UuId);


            if (TransID == null)
            {
                Console.WriteLine("TransID is null");
                return RedirectToPage(ErrorPage);
            }
            else
            {
                
                var transactionToRemove = WebUser.GetTransactionByID(TransID);
                if(transactionToRemove == null)Console.WriteLine("OnPostDeleteTransaction -> transactionToRemove is null!");
                else
                {
                    WebUser.Transactions.Remove(transactionToRemove);
                    Console.WriteLine($"Transaction with ID: {TransID} deleted!");
                }
            }
            return RedirectToPage(AccountOverviewPage, new { uuid = UuId });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting transaction: {ex.Message}");
            return RedirectToPage(ErrorPage);
        }
    }


    public IActionResult OnPostDeleteContract()
    {
        try
        {
            WebUser = null;
            if (!Request.Cookies.TryGetValue(SessionCookieName, out string? sessionId)) return RedirectToPage(IndexPage);
            if (sessionId == null) return RedirectToPage(IndexPage);
            WebUser = WebUser.GetUserBySession(sessionId);
            if (WebUser == null) return RedirectToPage(IndexPage);

            if (ContractID == null)
            {
                Console.WriteLine("ContractID is null");
                return RedirectToPage(ErrorPage);
            }
            else
            {
                var contractToRemove = WebUser.GetContractByID(ContractID);
                if(contractToRemove == null)Console.WriteLine("OnPostDeleteContract -> contractToRemove is null!");
                else
                {
                    WebUser.DeleteAllContractTransactions(ContractID, ContractAccID);
                    WebUser.Contracts.Remove(contractToRemove);
                    Console.WriteLine($"Contract with ID: {ContractID} deleted!");
                }
            }
            return RedirectToPage(AccountOverviewPage, new { uuid = UuId });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting contract: {ex.Message}");
            return RedirectToPage(ErrorPage);
        }
    }

    public IActionResult OnPostEditContract()
    {
        try
        {
            WebUser = null;
            if (!Request.Cookies.TryGetValue(SessionCookieName, out string? sessionId)) return RedirectToPage(IndexPage);
            if (sessionId == null) return RedirectToPage(IndexPage);
            WebUser = WebUser.GetUserBySession(sessionId);
            if (WebUser == null) return RedirectToPage(IndexPage);


            if (decimal.TryParse(Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal parsedAmount))
            {
                var currentContract = WebUser.GetContractByID(ContractID);
                if (currentContract == null)
                {
                    Console.WriteLine("CurrentContract is empty!");
                    return RedirectToPage(AccountOverviewPage, new { uuid = UuId });
                }
                if(IsFinished) EndDate = DateTime.Now;
                currentContract.Type = Type;
                currentContract.Category = Category;
                currentContract.Amount = parsedAmount;
                currentContract.Origin = Origin;
                currentContract.Destination = Destination;
                currentContract.Cycle = Cycle;
                currentContract.StartDate = Date;
                currentContract.EndDate = EndDate;
                currentContract.ContractId = ContractID;
                currentContract.AccountID = ContractAccID;

                WebUser.DeleteContract(currentContract.ContractId);
                WebUser.Contracts.Add(currentContract);
                WebUser.UpdateAllContractTransactions(ContractID, ContractAccID, Type, Category, parsedAmount, Destination, Origin, Ticker, Coin);
            }
            return RedirectToPage(AccountOverviewPage, new { uuid = UuId });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting contract: {ex.Message}");
            return RedirectToPage(ErrorPage);
        }
    }
}
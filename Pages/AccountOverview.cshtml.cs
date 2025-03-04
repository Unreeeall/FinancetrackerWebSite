using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;

namespace FinanceTracker.Pages;

public class AccountOverviewModel : PageModel
{



    public WebUser? WebUser { get; set; }
    public FinancialAccount? financialAccount { get; set; }

    [BindProperty]
    public string? UuId { get; set; }





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
            if (!Request.Cookies.TryGetValue("SessionCookie", out string? sessionId)) return RedirectToPage("/Index");
            if (sessionId == null) return RedirectToPage("/Index");
            WebUser = WebUser.GetUserBySession(sessionId);
            if (WebUser == null) return RedirectToPage("/Index");
            if (!WebUser.HasFinancialAccounts()) return RedirectToPage("/Dashboard");

            string accountID = HttpContext.Request.Query["uuid"].ToString();
            if (accountID == null)
            {
                Console.WriteLine("accountID from query is EMPTY!!");
                return RedirectToPage("/Error");
            }
            //UuId = accountID;
            Console.WriteLine("Account ID from query: " + accountID);
            // UuId = accountID;

            financialAccount = WebUser.GetAccountByID(accountID);
            if (financialAccount == null)
            {
                Console.WriteLine("FinancialAccount is empty!!");
                return RedirectToPage("/Error");
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
                        return RedirectToPage("/Error");
                }
            }





            Console.WriteLine("Account ID: " + accountID);
            return Page();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in OnGet: {ex.Message}");
            return RedirectToPage("/Error");
        }


    }


    public IActionResult OnPostAddTransaction()
    {
        try
        {
            WebUser = null;
            if (!Request.Cookies.TryGetValue("SessionCookie", out string? sessionId)) return RedirectToPage("/Index");
            if (sessionId == null) return RedirectToPage("/Index");
            WebUser = WebUser.GetUserBySession(sessionId);
            if (WebUser == null) return RedirectToPage("/Index");

            // string accountID = HttpContext.Request.Query["uuid"].ToString();
            // Console.WriteLine("Account ID from query in OnPostAddTransaction: " + accountID);


            if (decimal.TryParse(Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal parsedAmount))
            {
                // Console.WriteLine($"AccID: {AccID}");

                // Console.WriteLine($"TransactionType: {TransactionType}");
                // Console.WriteLine($"Origin: {Origin}");
                // Console.WriteLine($"Destination: {Destination}");
                // Console.WriteLine($"Category: {Category}");
                // Console.WriteLine($"Description: {Description}");
                // Console.WriteLine($"Date: {Date}");
                // Console.WriteLine($"Amount: {Amount}");
                // Console.WriteLine($"IsContract: {IsContract}");
                // Console.WriteLine($"Cycle: {Cycle}");



                if (IsContract)
                {

                    var newContract = new Contract
                    (
                        Type,
                        Category,
                        parsedAmount,
                        AccID,
                        Cycle,
                        Date,
                        Origin,
                        Destination,
                        EndDate
                    );
                    WebUser.Contracts.Add(newContract);
                }
                else
                {
                    var newTransaction = new Transaction
                    (
                    Type,
                    Date,
                    parsedAmount,
                    Origin,
                    Destination,
                    Description,
                    Category,
                    System.Guid.NewGuid().ToString(),
                    AccID,
                    IsContract,
                    Cycle
                    );
                    WebUser.Transactions.Add(newTransaction);
                }
            }
            return RedirectToPage("/AccountOverview", new { uuid = UuId });

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding Transaction: {ex.Message}");
            return RedirectToPage("/Error");
        }
    }

    public IActionResult OnPostEditTransaction()
    {
        WebUser = null;
        if (!Request.Cookies.TryGetValue("SessionCookie", out string? sessionId)) return RedirectToPage("/Index");
        if (sessionId == null) return RedirectToPage("/Index");
        WebUser = WebUser.GetUserBySession(sessionId);
        if (WebUser == null) return RedirectToPage("/Index");

        // string accountID = HttpContext.Request.Query["uuid"].ToString();
        // Console.WriteLine("Account ID from query in OnPostEditTransaction: " + accountID);

        if (TransID != null)
        {

            var currentTransaction = WebUser.GetTransactionByID(TransID);
            if (currentTransaction == null)
            {
                Console.WriteLine("CurrentTransaction is empty!");
                return RedirectToPage("/AccountOverview", new { uuid = UuId });
            }
            else
            {
                if (decimal.TryParse(Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal parsedAmount))
                {
                    
                        // var currentContract = WebUser.GetContractByID(currentTransaction.ContractId);
                        // if (currentContract == null)
                        // {
                        //     Console.WriteLine("CurrentContract is empty!");
                        //     return RedirectToPage("/AccountOverview", new { uuid = UuId });
                        // }
                        // currentContract.Type = Type;
                        // currentContract.Category = Category;
                        // currentContract.Amount = parsedAmount;
                        // currentContract.AccountID = AccID;
                        // currentContract.Cycle = Cycle;
                        // currentContract.StartDate = Date;
                        // currentContract.Origin = Origin;
                        // currentContract.Destination = Destination;
                        // currentContract.ContractId = currentContract.ContractId;

                        // WebUser.DeleteContract(currentContract.ContractId);
                        // WebUser.Contracts.Add(currentContract);
                    
                    
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
        return RedirectToPage("/AccountOverview", new { uuid = UuId });
    }


    public IActionResult OnPostDeleteTransaction()
    {
        try
        {
            WebUser = null;
            if (!Request.Cookies.TryGetValue("SessionCookie", out string? sessionId)) return RedirectToPage("/Index");
            if (sessionId == null) return RedirectToPage("/Index");
            WebUser = WebUser.GetUserBySession(sessionId);
            if (WebUser == null) return RedirectToPage("/Index");

            Console.WriteLine("UUID: " + UuId);

            // string accountID = HttpContext.Request.Query["uuid"].ToString();
            // if(accountID == null)
            // {
            //     Console.WriteLine("account ID from query is EMPTY!");
            //     if(UuId == null)
            //     {
            //         Console.WriteLine("account ID from query is EMPTY!");
            //         return RedirectToPage("/Error");
            //     }
            //     else
            //     {
            //         accountID = UuId;
            //         Console.WriteLine("Account ID from UuId in OnPostDeleteTransaction: " + accountID);
            //     }
            // }
            // else
            // {
            //     Console.WriteLine("Account ID from query in OnPostDeleteTransaction: " + accountID);
            // }



            if (TransID == null)
            {
                Console.WriteLine("TransID is null");
                return RedirectToPage("/Error");
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
            return RedirectToPage("/AccountOverview", new { uuid = UuId });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting transaction: {ex.Message}");
            return RedirectToPage("/Error");
        }
    }


    public IActionResult OnPostDeleteContract()
    {
        try
        {
            WebUser = null;
            if (!Request.Cookies.TryGetValue("SessionCookie", out string? sessionId)) return RedirectToPage("/Index");
            if (sessionId == null) return RedirectToPage("/Index");
            WebUser = WebUser.GetUserBySession(sessionId);
            if (WebUser == null) return RedirectToPage("/Index");

            // string accountID = HttpContext.Request.Query["uuid"].ToString();
            // Console.WriteLine("Account ID from query in OnPostDeleteContract: " + accountID);

            if (ContractID == null)
            {
                Console.WriteLine("ContractID is null");
                return RedirectToPage("/Error");
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
            return RedirectToPage("/AccountOverview", new { uuid = UuId });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting contract: {ex.Message}");
            return RedirectToPage("/Error");
        }
    }

    public IActionResult OnPostEditContract()
    {
        try
        {
            WebUser = null;
            if (!Request.Cookies.TryGetValue("SessionCookie", out string? sessionId)) return RedirectToPage("/Index");
            if (sessionId == null) return RedirectToPage("/Index");
            WebUser = WebUser.GetUserBySession(sessionId);
            if (WebUser == null) return RedirectToPage("/Index");


            if (decimal.TryParse(Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal parsedAmount))
            {
                var currentContract = WebUser.GetContractByID(ContractID);
                if (currentContract == null)
                {
                    Console.WriteLine("CurrentContract is empty!");
                    return RedirectToPage("/AccountOverview", new { uuid = UuId });
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

                //if(!string.IsNullOrEmpty(Type))
                WebUser.DeleteContract(currentContract.ContractId);
                WebUser.Contracts.Add(currentContract);
                WebUser.UpdateAllContractTransactions(ContractID, ContractAccID, Type, Category, parsedAmount, Destination, Origin, Ticker, Coin);
            }
            return RedirectToPage("/AccountOverview", new { uuid = UuId });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting contract: {ex.Message}");
            return RedirectToPage("/Error");
        }
    }
}
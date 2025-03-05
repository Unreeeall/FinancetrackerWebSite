
using Database;

namespace Transactions
{
    public class Transaction
    {

        public string Type { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string? Origin { get; set; }
        public string? Destination { get; set; }
        public string? Description { get; set; }
        public string Category { get; set; }
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
            Category = "null";
            ID = "null";
            AccountId = "null";
            // IsContract = false;
            Cycle = (BillingCycle)1;
            ContractId = null;
            Ticker = "null";
        }
        //Constructor for everything
        public Transaction(string type, DateTime date, decimal amount, string? origin, string? destination, string? description, string category, string id, string accountId, bool iscontract, BillingCycle cycle, string? contractId = null)
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
        // public Transaction(string type, DateTime date, decimal amount, string ticker, string accountId)
        // {
        //     Type = type;
        //     Date = date;
        //     Amount = amount;
        //     Ticker = ticker;
        //     AccountId = accountId;
        // }

        // Constructor for crypto transactions
        // public Transaction(string type, DateTime date, decimal amount, CryptoCoin coin, string accountId)
        // {
        //     Type = type;
        //     Date = date;
        //     Amount = amount;
        //     Coin = coin;
        //     AccountId = accountId;
        // }
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

    public class TransactionRequest
    {
        public required string UserEmail { get; set; }
        public required string Amount { get; set; }
        public bool IsContract { get; set; }
        public required string TransactionType { get; set; }
        public required string Category { get; set; }
        public required string AccID { get; set; }
        public BillingCycle Cycle { get; set; }
        public DateTime Date { get; set; }
        public string? Origin { get; set; }
        public string? Destination { get; set; }
        public DateTime? EndDate { get; set; }
        public required string Description { get; set; }
    }
}

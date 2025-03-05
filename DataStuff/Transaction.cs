
using Database;

namespace Transactions
{
    public class Transaction
    {

        public required string Type { get; set; }
        public required DateTime Date { get; set; }
        public required decimal Amount { get; set; }
        public string? Origin { get; set; }
        public string? Destination { get; set; }
        public string? Description { get; set; }
        public required string Category { get; set; }
        public required string ID { get; set; }
        public required string AccountId { get; set; }
        public required bool IsContract { get; set; }
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
            IsContract = false;
            Cycle = (BillingCycle)1;
            ContractId = null;
            Ticker = "null";
        }
        //Constructor for everything
        public Transaction(string type, DateTime date, decimal amount, string? origin, string? destination, string? description, string category, string id, string accountId, bool iscontract, BillingCycle? cycle, string? contractId = null)
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
    }

    public class Contract
    {
        public required string ContractId { get; set; }
        public required decimal Amount { get; set; }
        public required string Category { get; set; }
        public required BillingCycle Cycle { get; set; }
        public required DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public required string AccountID { get; set; }
        public required string Type { get; set; }

        public string? Origin { get; set; }
        public string? Destination { get; set; }


        public Contract() { }
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
        public required bool IsContract { get; set; }
        public required string TransactionType { get; set; }
        public required string Category { get; set; }
        public required string AccID { get; set; }
        public BillingCycle? Cycle { get; set; }
        public required DateTime Date { get; set; }
        public string? Origin { get; set; }
        public string? Destination { get; set; }
        public DateTime? EndDate { get; set; }
        public required string Description { get; set; }
    }
}

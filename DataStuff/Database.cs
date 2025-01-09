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



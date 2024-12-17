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
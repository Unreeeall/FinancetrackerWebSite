


using Database;
using FinanceUser;

namespace Services
{
    public class SharedServices
    {
        public static void AddFinanceAccount(WebUser? webUser, string AccountType, CurrencyType currency, string AccountName)
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
                WebUser.AccountLookup.Add(newBankAccount.ID, newBankAccount);

                WebUser.saveJson();
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
                WebUser.AccountLookup.Add(newCashAccount.ID, newCashAccount);

                WebUser.saveJson();

            }
            else if (AccountType == "Portfolio")
            {
                var newPortfolio = new PortfolioAccount
                (
                    AccountName
                );

                newPortfolio.ID = System.Guid.NewGuid().ToString();

                webUser?.PortfolioAccounts.Add(newPortfolio);
                WebUser.AccountLookup.Add(newPortfolio.ID, newPortfolio);

                WebUser.saveJson();
            }
            else if (AccountType == "CryptoWallet")
            {
                var newCryptoWallet = new CryptoWallet
                (
                    AccountName
                );

                newCryptoWallet.ID = System.Guid.NewGuid().ToString();

                webUser?.CryptoWallets.Add(newCryptoWallet);
                WebUser.AccountLookup.Add(newCryptoWallet.ID, newCryptoWallet);

                WebUser.saveJson();
            }


        }
    }
}

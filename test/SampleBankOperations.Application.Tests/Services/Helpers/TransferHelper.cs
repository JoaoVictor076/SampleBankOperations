using SampleBankOperations.Core.Entities;
using SampleBankOperations.Core.Interfaces;
using System;

namespace SampleBankOperations.Application.Tests.Application.Services.Helpers
{
    public class TransferHelper
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger _logger;

        public TransferHelper(IAccountRepository accountRepository, ILogger logger)
        {
            _accountRepository = accountRepository;
            _logger = logger;
        }

        public Account GetValidAccount(Account account, string accountType)
        {
            var existingAccount = _accountRepository.GetById(account.AccountId);
            if (existingAccount == null)
            {
                throw new InvalidOperationException($"{accountType} account not found.");
            }
            return existingAccount;
        }

        public bool HasSufficientBalance(Account account, decimal amount, Predicate<decimal> canWithdraw, Predicate<decimal> canTransfer)
        {
            return canWithdraw(account.Balance) && canTransfer(account.Balance);
        }

        public void ExecuteTransfer(Account fromAccount, Account toAccount, decimal amount, Func<decimal, bool> withdraw, Action<decimal> deposit)
        {
            if (withdraw(amount))
            {
                deposit(amount);
                _logger.Log($"Transferred {amount:C} from {fromAccount.AccountNumber} to {toAccount.AccountNumber}");
            }
            else
            {
                _logger.Log($"Failed to transfer {amount:C} from {fromAccount.AccountNumber} to {toAccount.AccountNumber}");
            }
        }
    }
}

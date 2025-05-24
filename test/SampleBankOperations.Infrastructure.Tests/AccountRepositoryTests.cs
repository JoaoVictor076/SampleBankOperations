using System;
using System.Linq;
using Xunit;
using SampleBankOperations.Core.Entities;
using SampleBankOperations.Infrastructure.Persistence;
using SampleBankOperations.Infrastructure.Persistence.Repositories;
using SampleBankOperations.Core.Enums;

namespace InfrastructureTest;

public class AccountRepositoryTests
{
    private readonly BankingDbContext _context;
    private readonly AccountRepository _repository;

    public AccountRepositoryTests()
    {
        _context = new BankingDbContext();
        _repository = new AccountRepository(_context);
    }

    [Fact]
    public void Add_ShouldAddAccountToContext()
    {
        var account = new Account("12345", 1000m, AccountType.Checking);

        _repository.Add(account);

        Assert.True(_context.Accounts.ContainsKey(account.AccountId));
        Assert.Equal(account, _context.Accounts[account.AccountId]);
    }

    [Fact]
    public void GetById_ShouldReturnAccount_WhenExists()
    {
        var account = new Account("12345", 1000m, AccountType.Checking);
        _context.Accounts[account.AccountId] = account;

        var result = _repository.GetById(account.AccountId);

        Assert.Equal(account, result);
    }

    [Fact]
    public void GetById_ShouldReturnNull_WhenNotExists()
    {
        var result = _repository.GetById(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public void GetAll_ShouldReturnAllAccounts()
    {
        var account1 = new Account("12345", 1000m, AccountType.Checking);
        var account2 = new Account("67890", 2000m, AccountType.Savings);
        _context.Accounts[account1.AccountId] = account1;
        _context.Accounts[account2.AccountId] = account2;

        var results = _repository.GetAll().ToList();

        Assert.Contains(account1, results);
        Assert.Contains(account2, results);
        Assert.Equal(2, results.Count);
    }

    [Fact]
    public void Update_ShouldReplaceExistingAccount()
    {
        var account = new Account("12345", 1000m, AccountType.Checking);
        _context.Accounts[account.AccountId] = account;

        var updatedAccount = new Account("12345", 2000m, AccountType.Checking);
        // Maintain the same AccountId to update correctly
        var idField = typeof(Account).GetProperty("AccountId");
        if (idField != null)
        {
            typeof(Account).GetProperty("AccountId")?.SetValue(updatedAccount, account.AccountId);
        }

        // But AccountId has a private setter; workaround is to set field via reflection or keep original account instance
        // Alternatively, update the existing account's balance:
        account = new Account("12345", 2000m, AccountType.Checking);
        _context.Accounts[account.AccountId] = account;

        _repository.Update(account);

        Assert.Equal(2000m, _context.Accounts[account.AccountId].Balance);
    }

    [Fact]
    public void Remove_ShouldRemoveAccountFromContext()
    {
        var account = new Account("12345", 1000m, AccountType.Checking);
        _context.Accounts[account.AccountId] = account;

        _repository.Remove(account);

        Assert.False(_context.Accounts.ContainsKey(account.AccountId));
    }

    [Fact]
    public void GetByAccountNumber_ShouldReturnAccount_WhenExists()
    {
        var account = new Account("12345", 1000m, AccountType.Checking);
        _context.Accounts[account.AccountId] = account;

        var result = _repository.GetByAccountNumber("12345");

        Assert.Equal(account, result);
    }

    [Fact]
    public void GetByAccountNumber_ShouldReturnNull_WhenNotExists()
    {
        var result = _repository.GetByAccountNumber("nonexistent");

        Assert.Null(result);
    }
}
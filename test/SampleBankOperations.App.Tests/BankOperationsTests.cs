using System;
using Moq;
using Xunit;
using SampleBankOperations.App.Services.Operations;
using SampleBankOperations.Application.Interfaces;
using SampleBankOperations.Core.Entities;
using SampleBankOperations.Core.Enums;
using SampleBankOperations.Core.Interfaces;

namespace AppTests;

public class BankOperationsTests
{
    private readonly Mock<IAccountService> _accountServiceMock;
    private readonly Mock<IAccountRepository> _accountRepositoryMock;
    private readonly Mock<ILogger> _loggerMock;
    private readonly BankOperations _bankOperations;

    public BankOperationsTests()
    {
        _accountServiceMock = new Mock<IAccountService>();
        _accountRepositoryMock = new Mock<IAccountRepository>();
        _loggerMock = new Mock<ILogger>();

        _bankOperations = new BankOperations(
            _accountServiceMock.Object,
            _accountRepositoryMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public void OpenAccount_ValidInput_ShouldAddAccountAndLog()
    {
        // Arrange
        var input = "12345\n1000\n";
        var stringReader = new System.IO.StringReader(input);
        Console.SetIn(stringReader);

        // Act
        _bankOperations.OpenAccount();

        // Assert
        _accountRepositoryMock.Verify(repo => repo.Add(It.Is<Account>(a =>
            a.AccountNumber == "12345" &&
            a.Balance == 1000m &&
            a.AccountType == AccountType.Checking
        )), Times.Once);

        _loggerMock.Verify(logger => logger.Log(It.Is<string>(s => s.Contains("Conta 12345 criada"))), Times.Once);
    }

    [Fact]
    public void OpenAccount_InvalidBalance_ShouldNotAddAccount()
    {
        var input = "12345\nabc\n";
        Console.SetIn(new System.IO.StringReader(input));

        _bankOperations.OpenAccount();

        _accountRepositoryMock.Verify(repo => repo.Add(It.IsAny<Account>()), Times.Never);
    }

    [Fact]
    public void GetAccountByNumber_ShouldReturnAccount()
    {
        var account = new Account("12345", 500, AccountType.Checking);
        _accountRepositoryMock.Setup(repo => repo.GetByAccountNumber("12345")).Returns(account);

        var result = _bankOperations.GetAccountByNumber("12345");

        Assert.Equal(account, result);
    }

    [Fact]
    public void ViewBalance_ShouldCallGetBalance()
    {
        var account = new Account("12345", 500, AccountType.Checking);
        _accountServiceMock.Setup(svc => svc.GetBalance(account)).Returns(500);

        using (var sw = new System.IO.StringWriter())
        {
            Console.SetOut(sw);

            _bankOperations.ViewBalance(account);

            var output = sw.ToString();
            Assert.Contains("Saldo atual da conta 12345", output);
        }
    }

    [Fact]
    public void Deposit_ValidAmount_ShouldCallDeposit()
    {
        var account = new Account("12345", 500, AccountType.Checking);
        Console.SetIn(new System.IO.StringReader("200\n"));

        using (var sw = new System.IO.StringWriter())
        {
            Console.SetOut(sw);

            _bankOperations.Deposit(account);

            _accountServiceMock.Verify(svc => svc.Deposit(account, 200m, It.IsAny<Action<decimal>>()), Times.Once);
        }
    }

    [Fact]
    public void Withdraw_ValidAmount_ShouldCallWithdraw()
    {
        var account = new Account("12345", 500, AccountType.Checking);
        Console.SetIn(new System.IO.StringReader("100\n"));

        using (var sw = new System.IO.StringWriter())
        {
            Console.SetOut(sw);

            _accountServiceMock.Setup(svc => svc.Withdraw(account, 100m, It.IsAny<Predicate<decimal>>())).Returns(true);

            _bankOperations.Withdraw(account);

            var output = sw.ToString();
            Assert.Contains("Sacado", output);
        }
    }

    [Fact]
    public void Transfer_ValidAmount_ShouldCallTransfer()
    {
        var fromAccount = new Account("111", 1000, AccountType.Checking);
        var toAccount = new Account("222", 500, AccountType.Checking);

        Console.SetIn(new System.IO.StringReader("300\n"));

        using (var sw = new System.IO.StringWriter())
        {
            Console.SetOut(sw);

            _accountServiceMock.Setup(svc => svc.Transfer(fromAccount, toAccount, 300m,
                It.IsAny<Predicate<decimal>>(), It.IsAny<Predicate<decimal>>())).Returns(true);

            _bankOperations.Transfer(fromAccount, toAccount);

            var output = sw.ToString();
            Assert.Contains("Transferido: R$ 300,00", output); 
        }
    }

    [Fact]
    public void CalculateInterest_ValidRate_ShouldCallCalculateInterest()
    {
        var account = new Account("12345", 1000, AccountType.Checking);
        Console.SetIn(new System.IO.StringReader("10\n"));

        using (var sw = new System.IO.StringWriter())
        {
            Console.SetOut(sw);

            _accountServiceMock.Setup(svc => svc.CalculateInterest(account, It.IsAny<Func<decimal, decimal, decimal>>(), 10m))
                .Returns(100);

            _bankOperations.CalculateInterest(account);

            var output = sw.ToString();
            Assert.Contains("Juros calculado: R$ 100,00", output);
        }
    }

    [Fact]
    public void ViewBalance_NullAccount_ShouldThrowException()
    {
        Assert.Throws<ArgumentNullException>(() => _bankOperations.ViewBalance(null));
    }

    [Fact]
    public void Deposit_NullAccount_ShouldThrowException()
    {
        Assert.Throws<ArgumentNullException>(() => _bankOperations.Deposit(null));
    }

    [Fact]
    public void Withdraw_NullAccount_ShouldThrowException()
    {
        Assert.Throws<ArgumentNullException>(() => _bankOperations.Withdraw(null));
    }

    [Fact]
    public void Transfer_NullFromAccount_ShouldThrowException()
    {
        var toAccount = new Account("222", 500, AccountType.Checking);
        Assert.Throws<ArgumentNullException>(() => _bankOperations.Transfer(null, toAccount));
    }

    [Fact]
    public void Transfer_NullToAccount_ShouldThrowException()
    {
        var fromAccount = new Account("111", 1000, AccountType.Checking);
        Assert.Throws<ArgumentNullException>(() => _bankOperations.Transfer(fromAccount, null));
    }
}

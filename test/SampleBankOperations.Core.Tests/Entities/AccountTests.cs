using Bogus;
using FluentAssertions;
using SampleBankOperations.Core.Entities;
using SampleBankOperations.Core.Enums;
using SampleBankOperations.Core.Services.Validations;

namespace SampleBankOperations.Core.Tests.Entities;

public class AccountTests
{
    private readonly Faker _faker = new();

    [Theory]
    [InlineData(100, 50)]
    [InlineData(200, 0)]
    [InlineData(0, 0)]
    public void Deposit_ShouldIncreaseBalance_WhenAmountIsPositive(decimal initialBalance, decimal depositAmount)
    {
        // Arrange
        var account = new Account(_faker.Finance.Account(), initialBalance, AccountType.Checking);

        // Act
        account.Deposit(depositAmount);

        // Assert
        account.Balance.Should().Be(initialBalance + depositAmount);
    }

    [Theory]
    [InlineData(100, 50, true)]
    [InlineData(100, 100, true)]
    [InlineData(100, 150, false)]
    [InlineData(0, 1, false)]
    public void Withdraw_ShouldReturnExpectedResult_AndUpdateBalanceCorrectly(decimal initialBalance, decimal withdrawAmount, bool expectedSuccess)
    {
        // Arrange
        var account = new Account(_faker.Finance.Account(), initialBalance, AccountType.Savings);

        // Act
        var result = account.Withdraw(withdrawAmount);

        // Assert
        result.Should().Be(expectedSuccess);

        if (expectedSuccess)
            account.Balance.Should().Be(initialBalance - withdrawAmount);
        else
            account.Balance.Should().Be(initialBalance);
    }

    [Fact]
    public void Constructor_ShouldInitializePropertiesCorrectly()
    {
        // Arrange
        var accountNumber = _faker.Finance.Account();
        var initialBalance = _faker.Random.Decimal(0, 1000);
        var accountType = AccountType.Savings;

        // Act
        var account = new Account(accountNumber, initialBalance, accountType);

        // Assert
        account.AccountId.Should().NotBeEmpty();
        account.AccountNumber.Should().Be(accountNumber);
        account.Balance.Should().Be(initialBalance);
        account.AccountType.Should().Be(accountType);
    }
    [Theory]
    [InlineData(-100)]
    [InlineData(0)]
    public void Deposit_ShouldThrowException_WhenAmountIsNonPositive(decimal depositAmount)
    {
        // Arrange
        var initialBalance = 500;
        var account = new Account(_faker.Finance.Account(), initialBalance, AccountType.Checking);

        // Act
        Action act = () => account.Deposit(depositAmount);

        // Assert
        act.Should()
            .Throw<Exception>()
            .WithMessage("Valor inválido");

        account.Balance.Should().Be(initialBalance);
    }
    [Fact]
    public void Withdraw_ShouldSucceed_WhenBalanceIsSufficient()
    {
        // Arrange
        var initialBalance = 1000;
        var withdrawAmount = 200;
        var account = new Account(_faker.Finance.Account(), initialBalance, AccountType.Checking);

        // Act
        var result = account.Withdraw(withdrawAmount);

        // Assert
        result.Should().BeTrue();
        account.Balance.Should().Be(initialBalance - withdrawAmount);
    }
    [Theory]
    [InlineData(100, 50, true)]
    [InlineData(100, 150, false)]
    public void HasSufficientBalance_ShouldReturnExpectedResult(decimal balance, decimal amount, bool expected)
    {
        var result = AccountPredicate.HasSufficientBalance(balance, amount);
        Assert.Equal(expected, result);
    }
    [Theory]
    [InlineData(100, 50, true)]
    [InlineData(100, 150, false)]
    public void IsBalanceAboveMinimum_ShouldReturnExpectedResult(decimal balance, decimal minimum, bool expected)
    {
        var result = AccountPredicate.IsBalanceAboveMinimum(balance, minimum);
        Assert.Equal(expected, result);
    }
}

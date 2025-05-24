using Xunit;
using SampleBankOperations.Core.Services.Validations;

public class AccountPredicateTests
{
    [Theory]
    [InlineData(1000, 500, true)]
    [InlineData(500, 500, true)]
    [InlineData(400, 500, false)]
    public void HasSufficientBalance_ShouldReturnExpectedResult(decimal balance, decimal amount, bool expected)
    {
        // Act
        var result = AccountPredicate.HasSufficientBalance(balance, amount);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(1000, 200, true)]
    [InlineData(200, 200, true)]
    [InlineData(100, 200, false)]
    public void IsBalanceAboveMinimum_ShouldReturnExpectedResult(decimal balance, decimal minimumBalance, bool expected)
    {
        // Act
        var result = AccountPredicate.IsBalanceAboveMinimum(balance, minimumBalance);

        // Assert
        Assert.Equal(expected, result);
    }
}

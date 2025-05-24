using SampleBankOperations.Application.Validations;
using Xunit;

namespace SampleBankOperations.Application.Tests.Validations
{
    public class AccountValidatorTests
    {
        [Theory]
        [InlineData(1000, 500, true)]   // balance >= minimumBalance
        [InlineData(1000, 1000, true)]  // balance == minimumBalance
        [InlineData(1000, 1500, true)]  // balance >= minimumBalance (corrigido para true)
        [InlineData(1000, 2000, true)]  // balance >= minimumBalance
        [InlineData(1000, 250, false)]  // balance < minimumBalance
        public void MinimumBalanceValidator_ShouldReturnExpectedResult(decimal minimumBalance, decimal balance, bool expected)
        {
            var validator = AccountValidator.MinimumBalanceValidator(minimumBalance);

            bool result = validator(balance);

            // Verifica se o resultado está correto
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(500, 1000, true)]    // balance >= requestedAmount
        [InlineData(1000, 1000, true)]   // balance == requestedAmount
        [InlineData(1500, 1000, false)]  // balance < requestedAmount
        public void RequestedAmountValidator_ShouldReturnExpectedResult(decimal requestedAmount, decimal balance, bool expected)
        {
            var validator = AccountValidator.RequestedAmountValidator(requestedAmount);

            bool result = validator(balance);

            Assert.Equal(expected, result);
        }
    }
}

using FluentAssertions;
using Xunit;
using Bogus;
using SampleBankOperations.Application.Validations;

namespace SampleBankOperations.Application.Tests.Validations
{
    public class AccountValidatorTests
    {
        private readonly Faker _faker = new();

        [Theory]
        [InlineData(100, 50, false)]
        [InlineData(100, 100, true)]
        [InlineData(100, 150, true)]
        public void MinimumBalanceValidator_ShouldReturnExpectedResult(decimal minimumBalance, decimal actualBalance, bool expected)
        {
            var validator = AccountValidator.MinimumBalanceValidator(minimumBalance);
            var result = validator(actualBalance);
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData(200, 250, true)]
        [InlineData(200, 200, true)]
        [InlineData(200, 150, false)]
        public void RequestedAmountValidator_ShouldReturnExpectedResult(decimal requestedAmount, decimal balanceAvailable, bool expected)
        {
            var validator = AccountValidator.RequestedAmountValidator(requestedAmount);
            var result = validator(balanceAvailable);
            result.Should().Be(expected);
        }

        [Fact]
        public void MinimumBalanceValidator_ShouldReturnFalse_WhenBalanceLessThanMinimum()
        {
            var minimumBalance = _faker.Random.Decimal(50, 1000);
            var balance = minimumBalance - _faker.Random.Decimal(1, minimumBalance);
            var predicate = AccountValidator.MinimumBalanceValidator(minimumBalance);

            var result = predicate(balance);

            result.Should().BeFalse();
        }

        [Fact]
        public void RequestedAmountValidator_ShouldReturnTrue_WhenBalanceGreaterThanRequested()
        {
            var requestedAmount = _faker.Random.Decimal(1, 999);
            var balance = requestedAmount + _faker.Random.Decimal(1, 1000);
            var predicate = AccountValidator.RequestedAmountValidator(requestedAmount);

            var result = predicate(balance);

            result.Should().BeTrue();
        }
    }
}

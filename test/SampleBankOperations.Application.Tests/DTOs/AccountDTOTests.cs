using System;
using FluentAssertions;
using Xunit;
using SampleBankOperations.Application.DTOs;

namespace SampleBankOperations.Application.Tests.DTOs
{
    public class AccountDTOTests
    {
        [Fact]
        public void AccountDTO_ShouldSetAndGetProperties()
        {
            var accountId = Guid.NewGuid();
            var accountNumber = "123456";
            var balance = 1000m;

            var dto = new AccountDTO
            {
                AccountId = accountId,
                AccountNumber = accountNumber,
                Balance = balance
            };

            dto.AccountId.Should().Be(accountId);
            dto.AccountNumber.Should().Be(accountNumber);
            dto.Balance.Should().Be(balance);
        }
    }
}

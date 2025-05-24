using Xunit;
using Moq;
using SampleBankOperations.Application.Services;
using SampleBankOperations.Core.Entities;
using SampleBankOperations.Core.Interfaces;
using SampleBankOperations.Core.Enums;
using System;

namespace SampleBankOperations.Application.Tests.Application.Services
{
    public class AccountServiceTests
    {
        private readonly Mock<IAccountRepository> _accountRepositoryMock;
        private readonly Mock<ILogger> _loggerMock;
        private readonly AccountService _accountService;
        private readonly Account _testAccount;

        public AccountServiceTests()
        {
            _accountRepositoryMock = new Mock<IAccountRepository>();
            _loggerMock = new Mock<ILogger>();

            _accountService = new AccountService(_accountRepositoryMock.Object, _loggerMock.Object);
            _testAccount = new Account("123", 1000m, AccountType.Checking);
        }

        [Fact]
        public void CalculateInterest_ShouldReturnCorrectInterest()
        {
            decimal result = _accountService.CalculateInterest(_testAccount, (balance, rate) => balance * (rate / 100), 10);
            Assert.Equal(100, result);
            _loggerMock.Verify(log => log.Log(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void Deposit_ShouldUpdateAccountAndLog()
        {
            decimal depositAmount = 500;

            _accountService.Deposit(_testAccount, depositAmount, amt => { });

            Assert.Equal(1500, _testAccount.Balance);
            _accountRepositoryMock.Verify(repo => repo.Update(_testAccount), Times.Once);
            _loggerMock.Verify(log => log.Log(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void Withdraw_WithSufficientBalance_ShouldUpdateAccountAndLog()
        {
            bool result = _accountService.Withdraw(_testAccount, 500, balance => balance >= 500);

            Assert.True(result);
            Assert.Equal(500, _testAccount.Balance);
            _accountRepositoryMock.Verify(repo => repo.Update(_testAccount), Times.Once);
            _loggerMock.Verify(log => log.Log(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void Withdraw_WithInsufficientBalance_ShouldReturnFalse()
        {
            bool result = _accountService.Withdraw(_testAccount, 1500, balance => balance >= 1500);

            Assert.False(result);
            _accountRepositoryMock.Verify(repo => repo.Update(It.IsAny<Account>()), Times.Never);
            _loggerMock.Verify(log => log.Log(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetBalance_ShouldReturnExistingBalance()
        {
            _accountRepositoryMock.Setup(r => r.GetById(_testAccount.AccountId)).Returns(_testAccount);

            decimal balance = _accountService.GetBalance(_testAccount);

            Assert.Equal(1000, balance);
            _loggerMock.Verify(log => log.Log(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetBalance_WhenAccountNotFound_ShouldReturnZero()
        {
            _accountRepositoryMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((Account)null);

            decimal balance = _accountService.GetBalance(_testAccount);

            Assert.Equal(0, balance);
            _loggerMock.Verify(log => log.Log(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void Transfer_ShouldExecuteTransferWhenValid()
        {
            // Arrange
            var toAccount = new Account("456", 500m, AccountType.Checking);

            _accountRepositoryMock
                .Setup(r => r.GetById(_testAccount.AccountId))
                .Returns(_testAccount);

            _accountRepositoryMock
                .Setup(r => r.GetById(toAccount.AccountId))
                .Returns(toAccount);

            var service = new AccountService(_accountRepositoryMock.Object, _loggerMock.Object);

            bool result = service.Transfer(_testAccount, toAccount, 200m, balance => true, balance => true);

            Assert.True(result);
        }
    }
}

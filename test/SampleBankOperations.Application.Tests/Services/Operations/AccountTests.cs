using Xunit;
using Moq;
using SampleBankOperations.Core.Entities;
using SampleBankOperations.Core.Enums;
using SampleBankOperations.App.Services.Operations;
using SampleBankOperations.Application.Interfaces;
using SampleBankOperations.Core.Interfaces;
using SampleBankOperations.Application.Tests.Application.Utils;

namespace SampleBankOperations.Application.Tests.Application.Services.Operations
{
    public class AccountTests
    {
        private readonly Mock<IAccountService> _accountServiceMock;
        private readonly Mock<IAccountRepository> _accountRepositoryMock;
        private readonly Mock<ILogger> _loggerMock;
        private readonly BankOperations _bankOperations;

        public AccountTests()
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
        public void Deposit_ShouldHandleInvalidInputGracefully()
        {
            var account = new Account("123", 1000, AccountType.Checking);

            var output = Helper.WithSimulatedOutput(() =>
            {
                Helper.WithSimulatedInput(() =>
                {
                    _bankOperations.Deposit(account);
                }, "abc");
            });

            Assert.Contains("Valor inválido", output);
        }

        [Fact]
        public void Deposit_ShouldThrowException_WhenAccountIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                Helper.WithSimulatedInput(() =>
                {
                    _bankOperations.Deposit(null);
                }, "500");
            });
        }

        [Fact]
        public void Transfer_ShouldHandleInvalidInputGracefully()
        {
            // Arrange
            var account = new Account("123", 1000, AccountType.Checking);
            decimal invalidAmount = -1000;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => account.Deposit(invalidAmount));
            Assert.Contains("Valor inválido", exception.Message);
        }

    }
}

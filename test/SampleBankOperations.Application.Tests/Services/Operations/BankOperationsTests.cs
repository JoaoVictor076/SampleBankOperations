using System;
using FluentAssertions;
using Moq;
using Xunit;
using SampleBankOperations.Application.Services;
using SampleBankOperations.Core.Entities;
using SampleBankOperations.Core.Enums;
using SampleBankOperations.Core.Interfaces;
using SampleBankOperations.App.Services.Operations;
using SampleBankOperations.Application.Interfaces;
using SampleBankOperations.Application.Tests.Application.Utils;

namespace SampleBankOperations.Application.Tests.Services
{
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

            output.Should().Contain("Valor inválido");
        }

        [Fact]
        public void Deposit_ShouldThrowException_WhenAccountIsNull()
        {
            Action act = () =>
            {
                Helper.WithSimulatedInput(() =>
                {
                    _bankOperations.Deposit(null);
                }, "500");
            };

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Transfer_ShouldHandleInvalidInputGracefully()
        {
            var fromAccount = new Account("123", 1000, AccountType.Checking);
            var toAccount = new Account("456", 500, AccountType.Checking);

            var output = Helper.WithSimulatedOutput(() =>
            {
                Helper.WithSimulatedInput(() =>
                {
                    _bankOperations.Transfer(fromAccount, toAccount);
                }, "abc");
            });

            output.Should().Contain("Valor inválido");
        }
    }
}

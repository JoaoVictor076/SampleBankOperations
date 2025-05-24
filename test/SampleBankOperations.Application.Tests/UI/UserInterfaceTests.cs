using System;
using Moq;
using SampleBankOperations.App.Interfaces;
using SampleBankOperations.App.Services.UI;
using SampleBankOperations.Application.Tests.Application.Utils;
using SampleBankOperations.Core.Entities;
using SampleBankOperations.Core.Enums;
using Xunit;

namespace SampleBankOperations.Application.Tests.Application.UI
{
    public class UserInterfaceTests
    {
        private readonly Mock<IBankOperations> _bankOperationsMock;
        private readonly UserInterface _userInterface;
        private readonly Account _testAccount;

        public UserInterfaceTests()
        {
            _bankOperationsMock = new Mock<IBankOperations>();
            _userInterface = new UserInterface(_bankOperationsMock.Object);
            _testAccount = new Account("123", 1000, AccountType.Checking);

            _bankOperationsMock.Setup(b => b.GetAccountByNumber(It.IsAny<string>())).Returns(_testAccount);
        }

        [Fact]
        public void ExitApplication_ShouldWriteExitMessage()
        {
            var method = _userInterface.GetType().GetMethod("ExitApplication", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var output = Helper.WithSimulatedOutput(() =>
            {
                method.Invoke(_userInterface, new object[] { null });
            });

            Assert.Contains("Obrigado por utilizar o SampleBankOperations", output.Replace("\r", "").Replace("\n", ""), StringComparison.InvariantCultureIgnoreCase);
        }
    }
}

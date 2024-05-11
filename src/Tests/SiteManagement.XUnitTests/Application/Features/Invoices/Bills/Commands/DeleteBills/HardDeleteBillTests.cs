using Moq;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Features.Commands.Invoices.Bills.DeleteBill.HardDelete;
using SiteManagement.Domain.Constants.Invoices.Bills;
using SiteManagement.Domain.Entities.Invoices;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Invoices;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Invoices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.XUnitTests.Application.Features.Invoices.Bills.Commands.DeleteBills
{
    public class HardDeleteBillTests : BillMockRepository
    {
        private readonly HardDeleteBillCommand _command;
        private readonly HardDeleteBillCommandHandler _handler;
        public HardDeleteBillTests(BillFakeDatas fakeData, HardDeleteBillCommand command) : base(fakeData)
        {
            _command = command;
            _handler = new(MockRepository.Object, BusinessRules);
        }

        [Fact]
        public async Task BillDoesNotExistDatabase_ShouldReturn_BusinessException()
        {
            //Arrange
            _command.Id = BillFakeDatas.NotInDbId;
            //Act
            async Task Action()  => await _handler.Handle(_command, CancellationToken.None);
            //Assert
            var exception = await Assert.ThrowsAsync<BusinessException>(Action);
            Assert.Equal(BillsMessages.RuleMessages.BillCannotBeFoundInDb, exception.Message);
        }

        [Fact]
        public async Task HardDeleteBillSuccessfully_Should_CallDeleteAsyncOnce()
        {
            //Arrange
            _command.Id = BillFakeDatas.InDbId;

            //Act
            var response = await _handler.Handle(_command, CancellationToken.None);

            //Assert
            MockRepository.Verify(x => x.DeleteAsync(It.IsAny<Bill>(), true, CancellationToken.None), Times.Once());
            Assert.Equal(_command.Id, response);
        }


    }
}

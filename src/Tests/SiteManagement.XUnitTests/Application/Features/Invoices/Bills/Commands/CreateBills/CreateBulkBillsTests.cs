using SiteManagement.Application.Features.Commands.Invoices.Bills.CreateBill;
using SiteManagement.Application.Features.Commands.Invoices.Bills.CreateBulkBills;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Invoices;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Invoices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.XUnitTests.Application.Features.Invoices.Bills.Commands.CreateBills
{
    public class CreateBulkBillsTests : BillMockRepository
    {
        private readonly CreateBulkBillsCommand _command;
        private readonly CreateBulkBillsCommandHandler _handler;
        public CreateBulkBillsTests(BillFakeDatas fakeData, CreateBulkBillsCommand command) : base(fakeData)
        {
            _command = command;
            _handler = new(MockRepository.Object, Mapper);
        }

        //    //TODO -- complete here later
        //[Fact]
        //public void BillFeeLessThanOrEqualToZero_ShouldReturn_ValidationError()
        //{
        //    //Arrange
        //    _command.Bills = new List<CreateBillCommand>()
        //    {
        //        new()
        //        {
        //            Fee = 0,


        //        }
        //     };
        //    //Act
            
        //    //Assert
        //}

    }
}

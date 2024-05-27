using SiteManagement.Application.Features.Commands.Residents.UpdateResident.UpdateInformation;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Residents;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Residents;

namespace SiteManagement.XUnitTests.Application.Features.Residents.Commands.UpdateResident;

public class UpdateResidentInformationTests : ResidentMockRepository
{
    private readonly UpdateResidentCommand _command;
    private readonly UpdateResidentCommandHandler _handler;
    private readonly UpdateResidentCommandValidator _validator;
    public UpdateResidentInformationTests(ResidentFakeDatas fakeData, UpdateResidentCommand command, UpdateResidentCommandValidator validator) : base(fakeData)
    {
        _command = command;
        _validator = validator;
        _handler = new(MockRepository.Object, Mapper, BusinessRules);
    }
 
}

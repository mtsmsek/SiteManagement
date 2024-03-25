using MediatR;
using SiteManagement.Application.Pagination.Responses;

namespace SiteManagement.Application.Features.Queries.Residents.GetListResidentByBlockName;

public class GetListResidentsByBlockNameQuery: IRequest<PagedViewModel<GetListResidentsByBlockNameResponse>>
{
    private string _blockName;
    //TODO - add validation to assure that value is not emmpty
    public string BlockName
    {
        get
        {
            return _blockName;
        }
        set
        {
            _blockName = value.ToUpper();
        }
    }
}

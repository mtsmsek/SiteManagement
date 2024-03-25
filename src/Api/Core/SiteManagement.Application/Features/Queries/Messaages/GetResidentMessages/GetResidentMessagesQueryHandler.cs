using AutoMapper;
using MediatR;
using SiteManagement.Application.Pagination.Responses;
using SiteManagement.Application.Services.Repositories.Residents;

namespace SiteManagement.Application.Features.Queries.Messaages.GetResidentMessages;

public class GetResidentMessagesQueryHandler : IRequestHandler<GetResidentMessagesQuery, PagedViewModel<GetResidentMessagesResponse>>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;

    public GetResidentMessagesQueryHandler(IMessageRepository messageRepository, IMapper mapper)
    {
        _messageRepository = messageRepository;
        _mapper = mapper;
    }

    public async Task<PagedViewModel<GetResidentMessagesResponse>> Handle(GetResidentMessagesQuery request, CancellationToken cancellationToken)
    {
        var  adminConversationMessages = await _messageRepository.GetListAsync(predicate: message => message.SenderId == request.UserId || 
                                                                                     message.ReceiverId == request.UserId,
                                                                           orderBy: messages => messages.OrderBy(message => message.CreatedDate),
                                                                           cancellationToken: cancellationToken,
                                                                           includes: message => message.Resident);


        return _mapper.Map<PagedViewModel<GetResidentMessagesResponse>>(adminConversationMessages);
    }
}

using AutoMapper;
using MediatR;
using SiteManagement.Application.Pagination.Responses;
using SiteManagement.Application.Rules.Residents;
using SiteManagement.Application.Services.Repositories.Residents;

namespace SiteManagement.Application.Features.Queries.Messaages.GetResidentMessages;

public class GetResidentMessagesQueryHandler : IRequestHandler<GetResidentMessagesQuery, PagedViewModel<GetResidentMessagesResponse>>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;
    private readonly ResidentBusinessRules _residentBusinessRules;

    public GetResidentMessagesQueryHandler(IMessageRepository messageRepository, IMapper mapper, ResidentBusinessRules residentBusinessRules)
    {
        _messageRepository = messageRepository;
        _mapper = mapper;
        _residentBusinessRules = residentBusinessRules;
    }

    public async Task<PagedViewModel<GetResidentMessagesResponse>> Handle(GetResidentMessagesQuery request, CancellationToken cancellationToken)
    {
        //TODO -- resident can only send message to admin but also admin can send all residents
        //TODO -- control that business rules is required or not may be we can control of count of get list method result
        await _residentBusinessRules.CheckIfResidentExistById(request.UserId, cancellationToken);
        var adminConversationMessages = await _messageRepository.GetListAsync(predicate: message => message.SenderId == request.UserId ||
                                                                           message.ReceiverId == request.UserId,
                                                                           orderBy: messages => messages.OrderBy(message => message.CreatedDate),
                                                                           cancellationToken: cancellationToken,
                                                                           includes: [message => message.Receiver, message => message.Sender]);
        

        return _mapper.Map<PagedViewModel<GetResidentMessagesResponse>>(adminConversationMessages);
    }
}

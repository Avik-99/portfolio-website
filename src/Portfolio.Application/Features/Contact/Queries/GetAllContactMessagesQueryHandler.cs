using MapsterMapper;
using MediatR;
using Portfolio.Application.DTOs;
using Portfolio.Domain.Interfaces;

namespace Portfolio.Application.Features.Contact.Queries;

public class GetAllContactMessagesQueryHandler : IRequestHandler<GetAllContactMessagesQuery, List<ContactMessageDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllContactMessagesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<ContactMessageDto>> Handle(GetAllContactMessagesQuery request, CancellationToken cancellationToken)
    {
        var messages = await _unitOfWork.ContactMessages.GetAllAsync(cancellationToken);

        return _mapper.Map<List<ContactMessageDto>>(
            messages.OrderByDescending(m => m.SubmittedAtUtc).ToList());
    }
}

using MediatR;
using Portfolio.Application.DTOs;

namespace Portfolio.Application.Features.Contact.Queries;

public record GetAllContactMessagesQuery : IRequest<List<ContactMessageDto>>;

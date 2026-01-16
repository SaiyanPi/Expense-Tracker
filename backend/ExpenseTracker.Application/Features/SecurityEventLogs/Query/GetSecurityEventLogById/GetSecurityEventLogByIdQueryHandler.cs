using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.DTOs.SecurityEventLog;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.SecurityEventLogs.Query.GetSecurityEventLogById;

public record GetSecurityEventLogByIdQueryHandler :  IRequestHandler<GetSecurityEventLogByIdQuery, SecurityEventLogDto>
{
    private readonly ISecurityEventLogRepository _securityEventLogRepository;
    private readonly IMapper _mapper;

    public GetSecurityEventLogByIdQueryHandler(
        ISecurityEventLogRepository securityEventLogRepository,
        IMapper mapper)
    {
        _securityEventLogRepository = securityEventLogRepository;
        _mapper = mapper;
    }

    public async Task<SecurityEventLogDto> Handle(GetSecurityEventLogByIdQuery request, CancellationToken cancellationToken)
    {
        var securityEventLog = await _securityEventLogRepository.GetById(request.Id);
        if(securityEventLog is null)
            throw new NotFoundException(nameof(SecurityEventLog), request.Id);

        return _mapper.Map<SecurityEventLogDto>(securityEventLog);
    }
}
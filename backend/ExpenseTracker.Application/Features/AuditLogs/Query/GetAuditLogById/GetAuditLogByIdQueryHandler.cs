using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.DTOs.AuditLog;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.AuditLogs.Query.GetAuditLogById;

public record GetAuditLogByIdQueryHandler :  IRequestHandler<GetAuditLogByIdQuery, AuditLogDto>
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IMapper _mapper;

    public GetAuditLogByIdQueryHandler(
        IAuditLogRepository auditLogRepository,
        IMapper mapper)
    {
        _auditLogRepository = auditLogRepository;
        _mapper = mapper;
    }

    public async Task<AuditLogDto> Handle(GetAuditLogByIdQuery request, CancellationToken cancellationToken)
    {
        var auditLog = await _auditLogRepository.GetById(request.Id);
        if(auditLog is null)
            throw new NotFoundException(nameof(AuditLog), request.Id);
        
        return _mapper.Map<AuditLogDto>(auditLog);
    }
}
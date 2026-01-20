using GTMS.Application.Common.Interfaces;
using GTMS.Domain.Entities.System;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GTMS.Application.Features.System.AccessLogs.Queries.GetAccessLogs;

public class AccessLogDto
{
    public Guid Id { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Details { get; set; } = string.Empty;
}

public class GetAccessLogsQuery : IRequest<List<AccessLogDto>>
{
}

public class GetAccessLogsQueryHandler : IRequestHandler<GetAccessLogsQuery, List<AccessLogDto>>
{
    private readonly IGtmsDbContext _context;

    public GetAccessLogsQueryHandler(IGtmsDbContext context)
    {
        _context = context;
    }

    public async Task<List<AccessLogDto>> Handle(GetAccessLogsQuery request, CancellationToken cancellationToken)
    {
        var logs = await _context.ActivityLogs
            .OrderByDescending(l => l.Timestamp)
            .Take(100) // Limit to last 100 for now
            .ToListAsync(cancellationToken);

        // Fetch User details efficiently
        var userIds = logs.Select(l => l.UserId).Distinct().ToList();
        var users = await _context.Users
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, cancellationToken);

        return logs.Select(l =>
        {
            var user = users.GetValueOrDefault(l.UserId);
            return new AccessLogDto
            {
                Id = l.Id,
                UserEmail = user?.Email ?? "Unknown",
                UserName = user != null ? $"{user.FirstName} {user.LastName}" : "Unknown",
                Action = l.Action,
                EntityName = l.EntityName,
                Timestamp = l.Timestamp,
                Details = l.DetailJson
            };
        }).ToList();
    }
}

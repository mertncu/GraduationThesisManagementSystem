using GTMS.Application.Common.Interfaces;
using GTMS.Domain.Entities.System;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GTMS.Application.Features.System.Configuration.Queries.GetSystemSettings;

public class SystemSettingDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class GetSystemSettingsQuery : IRequest<List<SystemSettingDto>>
{
}

public class GetSystemSettingsQueryHandler : IRequestHandler<GetSystemSettingsQuery, List<SystemSettingDto>>
{
    private readonly IGtmsDbContext _context;

    public GetSystemSettingsQueryHandler(IGtmsDbContext context)
    {
        _context = context;
    }

    public async Task<List<SystemSettingDto>> Handle(GetSystemSettingsQuery request, CancellationToken cancellationToken)
    {
        var settings = await _context.SystemSettings.ToListAsync(cancellationToken);

        if (!settings.Any())
        {
            // Seed some default settings if none exist
            // (Ideally this should be in DbSeeder, but specific request for "Configuration" feature logic)
            // Or just return empty list. Seeding logic is better in Seeder. 
            // I'll return empty for now, and maybe display "No settings defined" in View.
        }

        return settings.Select(s => new SystemSettingDto
        {
            Id = s.Id,
            Key = s.SettingKey,
            Value = s.SettingValue,
            Description = s.Description
        }).ToList();
    }
}

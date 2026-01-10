using GTMS.Domain.Common;

namespace GTMS.Domain.Entities.System;

public class SystemSetting : BaseEntity
{
    public string SettingKey { get; set; } = null!;
    public string SettingValue { get; set; } = null!;
    public string Description { get; set; } = null!;
}
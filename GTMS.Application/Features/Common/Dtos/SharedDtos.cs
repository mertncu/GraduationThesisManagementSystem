namespace GTMS.Application.Features.Common.Dtos;

public class DepartmentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class ProgramDto
{
    public Guid Id { get; set; }
    public Guid DepartmentId { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class SelectListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid? DepartmentId { get; set; }
}

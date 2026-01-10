using GTMS.Application.Features.Identity.Users.Queries.GetCreateUserFormData;
using GTMS.Application.Features.Common.Dtos;

namespace GTMS.Application.Features.Identity.Users.Dtos;

public class EditUserPageVm
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Role { get; set; } = "Student";
    
    public Guid? DepartmentId { get; set; }
    public Guid? ProgramId { get; set; }
    public string? StudentNumber { get; set; }
    
    public List<SelectListItemDto> Departments { get; set; } = new();
    public List<SelectListItemDto> Programs { get; set; } = new();
    public List<string> Roles { get; set; } = new() { "Student", "Advisor", "Admin" };
}

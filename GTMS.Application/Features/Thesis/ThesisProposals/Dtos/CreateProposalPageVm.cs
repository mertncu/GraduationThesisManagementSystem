using GTMS.Application.Features.Thesis.ThesisProposals.Queries.GetCreateProposalFormData;
using GTMS.Application.Features.Common.Dtos;

namespace GTMS.Application.Features.Thesis.ThesisProposals.Dtos;

public class CreateProposalPageVm
{
    public string Title { get; set; } = string.Empty;
    public string Abstract { get; set; } = string.Empty;
    public Guid AdvisorId { get; set; }
    public Guid TermId { get; set; }
    
    // Page Data
    public bool CanCreate { get; set; }
    public string WarningMessage { get; set; } = string.Empty;
    public List<SelectListItemDto> Advisors { get; set; } = new();
    public List<SelectListItemDto> Terms { get; set; } = new();
}

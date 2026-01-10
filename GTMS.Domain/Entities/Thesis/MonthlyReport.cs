using GTMS.Domain.Common;
using GTMS.Domain.Entities.Academic;
using GTMS.Domain.Entities.Thesis;
using System;

namespace GTMS.Domain.Entities.Thesis;

public class MonthlyReport : BaseEntity
{
    public Guid ThesisId { get; set; }
    public ThesisProject Thesis { get; set; } = null!;

    public Guid StudentId { get; set; }
    public Student Student { get; set; } = null!;

    public DateTime ReportDate { get; set; }
    public string Content { get; set; } = string.Empty;
    
    public string? AdvisorComment { get; set; }
    public DateTime? ReviewedAt { get; set; }
    
    // Status: Submitted, Reviewed (Approved/Rejected logic can be simple or Enum. Let's start simple with "IsReviewed").
    // Or better, use a Status string or Enum. Let's use string for consistency with other entities if no specific Enum exists.
    // Actually, let's create a specific Enum or use "SubmissionStatus" if applicable? 
    // Monthly Report is slightly different. Let's add a Status string for flexibility: "Submitted", "Approved", "NeedRevision".
    public string Status { get; set; } = "Submitted"; 
}

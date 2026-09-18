using System.ComponentModel.DataAnnotations;
using EHS.Domain.Enums;

namespace EHS.Application.DTOs
{
    // ---- Requests (what the frontend sends in) ----

    public class CreateIncidentDto
    {
        [Required, StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required, StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required, StringLength(200)]
        public string Location { get; set; } = string.Empty;

        [Range(1, 5)]
        public int SeverityLevel { get; set; }
    }

    public class AssignInvestigatorDto
    {
        public int InvestigatorId { get; set; }
    }

    public class CompleteInvestigationDto
    {
        public string Findings { get; set; } = string.Empty;
        public string RootCause { get; set; } = string.Empty;
    }

    public class ApprovalDecisionDto
    {
        public bool Approved { get; set; }
        public string? Comments { get; set; }
    }

    // ---- Responses (what the API sends back) ----

    public class IncidentResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int SeverityLevel { get; set; }
        public IncidentStatus Status { get; set; }
        public string ReportedByName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
    }
}

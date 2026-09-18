using EHS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHS.Domain.Entities
{
    public class Incident
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int SeverityLevel { get; set; } // 1 (minor) - 5 (critical)
        public IncidentStatus Status { get; set; } = IncidentStatus.Submitted;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ClosedAt { get; set; } // nullable: only set once closed

        // Foreign key + navigation property (Many Incidents -> One User)
        public int ReportedById { get; set; }
        public User ReportedBy { get; set; } = null!;

        // One Incident -> at most one Investigation (One-to-One in practice)
        public Investigation? Investigation { get; set; }

        // One Incident -> at most one Approval decision
        public Approval? Approval { get; set; }

        // One Incident -> many audit history rows
        public ICollection<IncidentStatusHistory> StatusHistory { get; set; } = new List<IncidentStatusHistory>();
    }
}

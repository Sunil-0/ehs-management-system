using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHS.Domain.Entities
{
    public enum ApprovalDecision
    {
        Approved = 0,
        Rejected = 1
    }

    public class Approval
    {
        public int Id { get; set; }

        public int IncidentId { get; set; }
        public Incident Incident { get; set; } = null!;

        public int ApprovedById { get; set; }
        public User ApprovedBy { get; set; } = null!;

        public ApprovalDecision Decision { get; set; }
        public string? Comments { get; set; }
        public DateTime ApprovedAt { get; set; } = DateTime.UtcNow;
    }
}

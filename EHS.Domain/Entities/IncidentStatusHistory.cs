using EHS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHS.Domain.Entities
{
    // Every status change gets logged here.
    // This is what lets you show "audit trail" / "timeline" views later,
    // and it's a very common thing interviewers ask about ("how would you
    // track history of a record over time?").
    public class IncidentStatusHistory
    {
        public int Id { get; set; }

        public int IncidentId { get; set; }
        public Incident Incident { get; set; } = null!;

        public IncidentStatus FromStatus { get; set; }
        public IncidentStatus ToStatus { get; set; }

        public int ChangedById { get; set; }
        public User ChangedBy { get; set; } = null!;

        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHS.Domain.Entities
{
    public class Investigation
    {
        public int Id { get; set; }

        public int IncidentId { get; set; }
        public Incident Incident { get; set; } = null!;

        public int InvestigatorId { get; set; }
        public User Investigator { get; set; } = null!;

        public string? Findings { get; set; }
        public string? RootCause { get; set; }

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
    }
}

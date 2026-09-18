using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHS.Domain.Enums
{
    // This enum IS your workflow diagram, expressed as code.
    // Submitted -> Assigned -> InvestigationInProgress -> PendingApproval -> Closed
    public enum IncidentStatus
    {
        Submitted = 0,
        Assigned = 1,
        InvestigationInProgress = 2,
        PendingApproval = 3,
        Closed = 4,
        Rejected = 5   // manager sends investigation back instead of approving
    }
}

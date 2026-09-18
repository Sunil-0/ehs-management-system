// Mirrors EHS.Domain.Enums.IncidentStatus exactly (as numbers, since that's
// how the backend serializes a C# enum to JSON by default).
export enum IncidentStatus {
  Submitted = 0,
  Assigned = 1,
  InvestigationInProgress = 2,
  PendingApproval = 3,
  Closed = 4,
  Rejected = 5
}
 
// Human-readable labels for the UI — the enum values above are for logic,
// this map is purely for display.
export const IncidentStatusLabels: Record<IncidentStatus, string> = {
  [IncidentStatus.Submitted]: 'Submitted',
  [IncidentStatus.Assigned]: 'Assigned',
  [IncidentStatus.InvestigationInProgress]: 'Investigation In Progress',
  [IncidentStatus.PendingApproval]: 'Pending Approval',
  [IncidentStatus.Closed]: 'Closed',
  [IncidentStatus.Rejected]: 'Rejected'
};
 
// Mirrors IncidentResponseDto from the backend
export interface Incident {
  id: number;
  title: string;
  description: string;
  location: string;
  severityLevel: number;
  status: IncidentStatus;
  reportedByName: string;
  createdAt: string;
  closedAt: string | null;
}
 
// Mirrors CreateIncidentDto
export interface CreateIncidentRequest {
  title: string;
  description: string;
  location: string;
  severityLevel: number;
}
 
// Mirrors AssignInvestigatorDto
export interface AssignInvestigatorRequest {
  investigatorId: number;
}
 
// Mirrors CompleteInvestigationDto
export interface CompleteInvestigationRequest {
  findings: string;
  rootCause: string;
}
 
// Mirrors ApprovalDecisionDto
export interface ApprovalDecisionRequest {
  approved: boolean;
  comments?: string;
}
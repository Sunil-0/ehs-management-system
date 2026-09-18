using EHS.Application.DTOs;
using EHS.Application.Exceptions;
using EHS.Application.Interfaces;
using EHS.Domain.Entities;
using EHS.Domain.Enums;

namespace EHS.Application.Services
{
    public class IncidentService : IIncidentService
    {
        private readonly IIncidentRepository _incidentRepository;

        public IncidentService(IIncidentRepository incidentRepository)
        {
            _incidentRepository = incidentRepository;
        }

        public async Task<IncidentResponseDto> SubmitIncidentAsync(CreateIncidentDto dto, int reportedByUserId)
        {
            var incident = new Incident
            {
                Title = dto.Title,
                Description = dto.Description,
                Location = dto.Location,
                SeverityLevel = dto.SeverityLevel,
                ReportedById = reportedByUserId,
                Status = IncidentStatus.Submitted,
                CreatedAt = DateTime.UtcNow
            };

            await _incidentRepository.AddAsync(incident);
            await _incidentRepository.SaveChangesAsync();

            return ToDto(incident);
        }

        public async Task<List<IncidentResponseDto>> GetAllAsync()
        {
            var incidents = await _incidentRepository.GetAllAsync();
            return incidents.Select(ToDto).ToList();
        }

        public async Task<IncidentResponseDto> AssignInvestigatorAsync(int incidentId, AssignInvestigatorDto dto, int changedByUserId)
        {
            var incident = await _incidentRepository.GetByIdAsync(incidentId)
                ?? throw new NotFoundException($"Incident {incidentId} not found.");

            // ---- THE RULE: can only assign from Submitted status ----
            if (incident.Status != IncidentStatus.Submitted)
                throw new InvalidWorkflowTransitionException(
                    $"Cannot assign an investigator while incident is in '{incident.Status}' status.");

            incident.Investigation = new Investigation
            {
                IncidentId = incident.Id,
                InvestigatorId = dto.InvestigatorId,
                AssignedAt = DateTime.UtcNow
            };

            TransitionStatus(incident, IncidentStatus.Assigned, changedByUserId);

            await _incidentRepository.SaveChangesAsync();
            return ToDto(incident);
        }

        public async Task<IncidentResponseDto> CompleteInvestigationAsync(int incidentId, CompleteInvestigationDto dto, int changedByUserId)
        {
            var incident = await _incidentRepository.GetByIdAsync(incidentId)
                ?? throw new NotFoundException($"Incident {incidentId} not found.");

            if (incident.Status != IncidentStatus.Assigned && incident.Status != IncidentStatus.InvestigationInProgress)
                throw new InvalidWorkflowTransitionException(
                    $"Cannot complete investigation while incident is in '{incident.Status}' status.");

            if (incident.Investigation is null)
                throw new InvalidWorkflowTransitionException("No investigator has been assigned yet.");

            // Only the assigned investigator should be able to do this --
            // that check happens in the controller via [Authorize], this is
            // the *data* check that the right investigation record exists.
            incident.Investigation.Findings = dto.Findings;
            incident.Investigation.RootCause = dto.RootCause;
            incident.Investigation.CompletedAt = DateTime.UtcNow;

            TransitionStatus(incident, IncidentStatus.PendingApproval, changedByUserId);

            await _incidentRepository.SaveChangesAsync();
            return ToDto(incident);
        }

        public async Task<IncidentResponseDto> DecideApprovalAsync(int incidentId, ApprovalDecisionDto dto, int changedByUserId)
        {
            var incident = await _incidentRepository.GetByIdAsync(incidentId)
                ?? throw new NotFoundException($"Incident {incidentId} not found.");

            if (incident.Status != IncidentStatus.PendingApproval)
                throw new InvalidWorkflowTransitionException(
                    $"Cannot approve/reject while incident is in '{incident.Status}' status.");

            incident.Approval = new Approval
            {
                IncidentId = incident.Id,
                ApprovedById = changedByUserId,
                Decision = dto.Approved ? ApprovalDecision.Approved : ApprovalDecision.Rejected,
                Comments = dto.Comments,
                ApprovedAt = DateTime.UtcNow
            };

            if (dto.Approved)
            {
                // ---- "System -> Close Incident" from your diagram happens
                // automatically, right here, as a direct consequence of approval.
                // This is a business rule, so it belongs in the service, not
                // in the controller and not left to the frontend to trigger.
                TransitionStatus(incident, IncidentStatus.Closed, changedByUserId);
                incident.ClosedAt = DateTime.UtcNow;
            }
            else
            {
                TransitionStatus(incident, IncidentStatus.Rejected, changedByUserId);
            }

            await _incidentRepository.SaveChangesAsync();
            return ToDto(incident);
        }

        // ---- Helpers ----

        private static void TransitionStatus(Incident incident, IncidentStatus newStatus, int changedByUserId)
        {
            var oldStatus = incident.Status;
            incident.Status = newStatus;

            incident.StatusHistory.Add(new IncidentStatusHistory
            {
                IncidentId = incident.Id,
                FromStatus = oldStatus,
                ToStatus = newStatus,
                ChangedById = changedByUserId,
                ChangedAt = DateTime.UtcNow
            });
        }

        private static IncidentResponseDto ToDto(Incident incident) => new()
        {
            Id = incident.Id,
            Title = incident.Title,
            Description = incident.Description,
            Location = incident.Location,
            SeverityLevel = incident.SeverityLevel,
            Status = incident.Status,
            ReportedByName = incident.ReportedBy?.Name ?? string.Empty,
            CreatedAt = incident.CreatedAt,
            ClosedAt = incident.ClosedAt
        };
    }
}
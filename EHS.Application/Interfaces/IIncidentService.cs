using EHS.Application.DTOs;

namespace EHS.Application.Interfaces
{
    public interface IIncidentService
    {
        Task<IncidentResponseDto> SubmitIncidentAsync(CreateIncidentDto dto, int reportedByUserId);
        Task<List<IncidentResponseDto>> GetAllAsync();
        Task<IncidentResponseDto> AssignInvestigatorAsync(int incidentId, AssignInvestigatorDto dto, int changedByUserId);
        Task<IncidentResponseDto> CompleteInvestigationAsync(int incidentId, CompleteInvestigationDto dto, int changedByUserId);
        Task<IncidentResponseDto> DecideApprovalAsync(int incidentId, ApprovalDecisionDto dto, int changedByUserId);
    }
}
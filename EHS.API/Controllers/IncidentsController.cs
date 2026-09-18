using System.Security.Claims;
using EHS.Application.DTOs;
using EHS.Application.Exceptions;
using EHS.Application.Interfaces;
using EHS.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EHS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // every endpoint in this controller requires a valid JWT by default
    public class IncidentsController : ControllerBase
    {
        private readonly IIncidentService _incidentService;

        public IncidentsController(IIncidentService incidentService)
        {
            _incidentService = incidentService;
        }

        // Reads the real logged-in user's id out of the JWT's claims --
        // populated by UseAuthentication() from the "Authorization: Bearer <token>" header.
        // Replaces the hardcoded "CurrentUserId => 1" from before auth existed.
        private int CurrentUserId =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<ActionResult<List<IncidentResponseDto>>> GetAll()
        {
            var incidents = await _incidentService.GetAllAsync();
            return Ok(incidents);
        }

        // Any authenticated user can submit an incident -- matches your diagram's
        // "Employee -> Submit Incident" step; no role restriction needed beyond being logged in.
        [HttpPost]
        public async Task<ActionResult<IncidentResponseDto>> Submit(CreateIncidentDto dto)
        {
            var result = await _incidentService.SubmitIncidentAsync(dto, CurrentUserId);
            return CreatedAtAction(nameof(GetAll), new { id = result.Id }, result);
        }

        // Only EHS Managers can assign an investigator
        [HttpPut("{id}/assign")]
        [Authorize(Roles = nameof(UserRole.EHSManager))]
        public async Task<ActionResult<IncidentResponseDto>> Assign(int id, AssignInvestigatorDto dto)
        {
            try
            {
                var result = await _incidentService.AssignInvestigatorAsync(id, dto, CurrentUserId);
                return Ok(result);
            }
            catch (NotFoundException ex) { return NotFound(ex.Message); }
            catch (InvalidWorkflowTransitionException ex) { return Conflict(ex.Message); }
        }

        // Only Investigators can complete an investigation
        [HttpPut("{id}/complete-investigation")]
        [Authorize(Roles = nameof(UserRole.Investigator))]
        public async Task<ActionResult<IncidentResponseDto>> CompleteInvestigation(int id, CompleteInvestigationDto dto)
        {
            try
            {
                var result = await _incidentService.CompleteInvestigationAsync(id, dto, CurrentUserId);
                return Ok(result);
            }
            catch (NotFoundException ex) { return NotFound(ex.Message); }
            catch (InvalidWorkflowTransitionException ex) { return Conflict(ex.Message); }
        }

        // Only Managers can approve/reject
        [HttpPut("{id}/approval")]
        [Authorize(Roles = nameof(UserRole.Manager))]
        public async Task<ActionResult<IncidentResponseDto>> DecideApproval(int id, ApprovalDecisionDto dto)
        {
            try
            {
                var result = await _incidentService.DecideApprovalAsync(id, dto, CurrentUserId);
                return Ok(result);
            }
            catch (NotFoundException ex) { return NotFound(ex.Message); }
            catch (InvalidWorkflowTransitionException ex) { return Conflict(ex.Message); }
        }
    }
}
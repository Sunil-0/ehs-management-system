using EHS.Application.DTOs;
using EHS.Application.Exceptions;
using EHS.Application.Interfaces;
using EHS.Application.Services;
using EHS.Domain.Entities;
using EHS.Domain.Enums;
using Moq;
using Xunit;

namespace EHS.Application.Tests
{
    public class IncidentServiceTests
    {
        // A small helper that builds the service with a mocked repository
        // "seeded" with one incident, so each test starts from a known state.
        private static (IncidentService service, Mock<IIncidentRepository> repoMock, Incident incident)
            BuildServiceWithIncident(IncidentStatus initialStatus)
        {
            var incident = new Incident
            {
                Id = 1,
                Title = "Slip near loading dock",
                Description = "Wet floor, no signage",
                Location = "Warehouse B",
                SeverityLevel = 3,
                Status = initialStatus,
                ReportedById = 1
            };

            var repoMock = new Mock<IIncidentRepository>();
            // Whenever the service calls GetByIdAsync(1), hand back our fake incident
            repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(incident);
            repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            var service = new IncidentService(repoMock.Object);
            return (service, repoMock, incident);
        }

        [Fact]
        public async Task AssignInvestigator_FromSubmitted_TransitionsToAssigned()
        {
            // Arrange
            var (service, _, _) = BuildServiceWithIncident(IncidentStatus.Submitted);

            // Act
            var result = await service.AssignInvestigatorAsync(1, new AssignInvestigatorDto { InvestigatorId = 3 }, changedByUserId: 2);

            // Assert
            Assert.Equal(IncidentStatus.Assigned, result.Status);
        }

        [Fact]
        public async Task AssignInvestigator_WhenAlreadyAssigned_ThrowsInvalidWorkflowTransition()
        {
            // This is the test that directly protects the bug we'd otherwise only
            // catch by manually clicking through the UI twice -- exactly the
            // 409 Conflict behavior we saw for real in the browser earlier.
            var (service, _, _) = BuildServiceWithIncident(IncidentStatus.Assigned);

            await Assert.ThrowsAsync<InvalidWorkflowTransitionException>(() =>
                service.AssignInvestigatorAsync(1, new AssignInvestigatorDto { InvestigatorId = 3 }, changedByUserId: 2));
        }

        [Fact]
        public async Task DecideApproval_WhenApproved_ClosesIncidentAndSetsClosedAt()
        {
            var (service, _, _) = BuildServiceWithIncident(IncidentStatus.PendingApproval);

            var result = await service.DecideApprovalAsync(1, new ApprovalDecisionDto { Approved = true }, changedByUserId: 4);

            Assert.Equal(IncidentStatus.Closed, result.Status);
            Assert.NotNull(result.ClosedAt);
        }

        [Fact]
        public async Task DecideApproval_WhenRejected_SetsRejectedStatus_AndDoesNotSetClosedAt()
        {
            var (service, _, _) = BuildServiceWithIncident(IncidentStatus.PendingApproval);

            var result = await service.DecideApprovalAsync(1, new ApprovalDecisionDto { Approved = false }, changedByUserId: 4);

            Assert.Equal(IncidentStatus.Rejected, result.Status);
            Assert.Null(result.ClosedAt);
        }

        [Fact]
        public async Task DecideApproval_WhenNotPendingApproval_ThrowsInvalidWorkflowTransition()
        {
            // Can't approve something still sitting in Submitted -- skipping straight
            // from "reported" to "approved" should never be possible.
            var (service, _, _) = BuildServiceWithIncident(IncidentStatus.Submitted);

            await Assert.ThrowsAsync<InvalidWorkflowTransitionException>(() =>
                service.DecideApprovalAsync(1, new ApprovalDecisionDto { Approved = true }, changedByUserId: 4));
        }

        [Fact]
        public async Task AssignInvestigator_IncidentNotFound_ThrowsNotFoundException()
        {
            var repoMock = new Mock<IIncidentRepository>();
            repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Incident?)null);
            var service = new IncidentService(repoMock.Object);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                service.AssignInvestigatorAsync(999, new AssignInvestigatorDto { InvestigatorId = 3 }, changedByUserId: 2));
        }
    }
}
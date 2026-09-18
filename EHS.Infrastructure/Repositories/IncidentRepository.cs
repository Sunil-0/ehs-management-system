using EHS.Application.Interfaces;
using EHS.Domain.Entities;
using EHS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EHS.Infrastructure.Repositories
{
    public class IncidentRepository : IIncidentRepository
    {
        private readonly AppDbContext _context;

        public IncidentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Incident?> GetByIdAsync(int id)
        {
            // Include() eager-loads related data in the same query,
            // otherwise Investigation/Approval/ReportedBy would be null.
            return await _context.Incidents
                .Include(i => i.ReportedBy)
                .Include(i => i.Investigation)
                    .ThenInclude(inv => inv!.Investigator)
                .Include(i => i.Approval)
                .Include(i => i.StatusHistory)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<List<Incident>> GetAllAsync()
        {
            return await _context.Incidents
                .Include(i => i.ReportedBy)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(Incident incident)
        {
            await _context.Incidents.AddAsync(incident);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
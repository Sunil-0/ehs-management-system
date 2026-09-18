using EHS.Domain.Entities;

namespace EHS.Application.Interfaces
{
    public interface IIncidentRepository
    {
        Task<Incident?> GetByIdAsync(int id);
        Task<List<Incident>> GetAllAsync();
        Task AddAsync(Incident incident);
        Task<int> SaveChangesAsync();
    }
}

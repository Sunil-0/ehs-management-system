using EHS.Domain.Entities;

namespace EHS.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
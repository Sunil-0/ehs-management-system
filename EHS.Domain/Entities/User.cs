using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EHS.Domain.Enums;


namespace EHS.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty; // never store plain text passwords
        public UserRole Role { get; set; }

        // Navigation properties: EF Core uses these to know
        // "a User can have many Incidents they reported"
        public ICollection<Incident> ReportedIncidents { get; set; } = new List<Incident>();
    }

}

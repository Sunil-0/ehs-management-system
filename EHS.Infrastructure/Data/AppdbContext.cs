using EHS.Domain.Entities;
using EHS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EHS.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Incident> Incidents => Set<Incident>();
        public DbSet<Investigation> Investigations => Set<Investigation>();
        public DbSet<Approval> Approvals => Set<Approval>();
        public DbSet<IncidentStatusHistory> IncidentStatusHistories => Set<IncidentStatusHistory>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // --- User -> Incidents (One-to-Many) ---
            modelBuilder.Entity<Incident>()
                .HasOne(i => i.ReportedBy)
                .WithMany(u => u.ReportedIncidents)
                .HasForeignKey(i => i.ReportedById)
                .OnDelete(DeleteBehavior.Restrict); // don't cascade-delete incidents if a user is removed

            // --- Incident -> Investigation (One-to-One) ---
            modelBuilder.Entity<Investigation>()
                .HasOne(inv => inv.Incident)
                .WithOne(i => i.Investigation)
                .HasForeignKey<Investigation>(inv => inv.IncidentId);

            modelBuilder.Entity<Investigation>()
                .HasOne(inv => inv.Investigator)
                .WithMany()
                .HasForeignKey(inv => inv.InvestigatorId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- Incident -> Approval (One-to-One) ---
            modelBuilder.Entity<Approval>()
                .HasOne(a => a.Incident)
                .WithOne(i => i.Approval)
                .HasForeignKey<Approval>(a => a.IncidentId);

            modelBuilder.Entity<Approval>()
                .HasOne(a => a.ApprovedBy)
                .WithMany()
                .HasForeignKey(a => a.ApprovedById)
                .OnDelete(DeleteBehavior.Restrict);

            // --- Incident -> StatusHistory (One-to-Many) ---
            modelBuilder.Entity<IncidentStatusHistory>()
                .HasOne(h => h.Incident)
                .WithMany(i => i.StatusHistory)
                .HasForeignKey(h => h.IncidentId);

            modelBuilder.Entity<IncidentStatusHistory>()
                .HasOne(h => h.ChangedBy)
                .WithMany()
                .HasForeignKey(h => h.ChangedById)
                .OnDelete(DeleteBehavior.Restrict);

            // --- Constraints & indexes ---
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Incident>()
                .HasIndex(i => i.Status); // dashboards will filter by status constantly

            modelBuilder.Entity<Incident>()
                .HasIndex(i => i.ReportedById);

            modelBuilder.Entity<Incident>()
                .ToTable(t => t.HasCheckConstraint("CK_Incident_SeverityLevel", "[SeverityLevel] BETWEEN 1 AND 5"));

            // --- Seed data ---
            // HasData() bakes these rows into a migration, so `dotnet ef database update`
            // inserts them automatically. This is the standard EF Core way to seed
            // reference/test data (as opposed to writing manual INSERT scripts).
            // Password for ALL seeded users: Password123!
            // (bcrypt hash below is real and verifiable via BCrypt.Net-Next -- not a placeholder)
            const string seedPasswordHash = "$2b$11$gBehB2FbUIFA2nZGbUrcqOjORV28Re5pA290nViD.baurAEtDAf1W";

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Name = "Alice Employee", Email = "alice@ehs.local", PasswordHash = seedPasswordHash, Role = UserRole.Employee },
                new User { Id = 2, Name = "Bob EHSManager", Email = "bob@ehs.local", PasswordHash = seedPasswordHash, Role = UserRole.EHSManager },
                new User { Id = 3, Name = "Carol Investigator", Email = "carol@ehs.local", PasswordHash = seedPasswordHash, Role = UserRole.Investigator },
                new User { Id = 4, Name = "Dave Manager", Email = "dave@ehs.local", PasswordHash = seedPasswordHash, Role = UserRole.Manager }
            );
        }
    }
}
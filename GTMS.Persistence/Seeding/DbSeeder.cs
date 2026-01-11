using GTMS.Domain.Entities.Identity;
using GTMS.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GTMS.Persistence.Seeding;

public class DbSeeder
{
    private readonly GtmsDbContext _context;

    public DbSeeder(GtmsDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        // 1. Ensure Roles Exist
        await SeedRoleAsync("Admin", "System Administrator");
        await SeedRoleAsync("Advisor", "Academic Advisor");
        await SeedRoleAsync("Student", "Student User");

        // 2. Ensure Admin User Exists
        var adminEmail = "admin@gtms.com";
        var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");

        if (adminRole != null && !await _context.Users.AnyAsync(u => u.Email == adminEmail))
        {
            var adminUser = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "System",
                LastName = "Admin",
                Email = adminEmail,
                PhoneNumber = "5551234567", // Optional now, but good to have
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                PasswordHash = HashPassword("Admin123!") // Simple hash for demo
            };

            _context.Users.Add(adminUser);
            await _context.SaveChangesAsync();

            // Assign Admin Role
            var userRole = new UserRole
            {
                Id = Guid.NewGuid(),
                UserId = adminUser.Id,
                RoleId = adminRole.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();
        }


        // 3. Ensure Application Statuses Exist
        var appStatuses = new[] { "Pending", "Approved", "Rejected" };
        foreach (var status in appStatuses)
        {
            if (!await _context.ApplicationStatuses.AnyAsync(s => s.Name == status))
            {
                _context.ApplicationStatuses.Add(new GTMS.Domain.Entities.Thesis.ApplicationStatus { Name = status });
            }
        }

        // 4. Ensure Thesis Statuses Exist
        var thesisStatuses = new[] { "Ongoing", "Completed", "Frozen", "Cancelled", "DefenseRequested", "DefenseScheduled", "DefenseFailed", "RevisionRequired" };
        foreach (var status in thesisStatuses)
        {
            if (!await _context.ThesisStatuses.AnyAsync(s => s.Name == status))
            {
                _context.ThesisStatuses.Add(new GTMS.Domain.Entities.Thesis.ThesisStatus { Name = status });
            }
        }
        
        // 5. Ensure Submission Statuses Exist
        var subStatuses = new[] { "Submitted", "Reviewed", "Approved", "NeedsRevision" };
        foreach (var status in subStatuses)
        {
            if (!await _context.SubmissionStatuses.AnyAsync(s => s.Name == status))
            {
                _context.SubmissionStatuses.Add(new GTMS.Domain.Entities.Submission.SubmissionStatus { Name = status });
            }
        }
        


        // 6. Ensure Milestone Types Exist
        var mileTypes = new[] { "Proposal", "Progress Report", "Draft", "Final Thesis", "Defense" };
        foreach (var type in mileTypes)
        {
             if (!await _context.MilestoneTypes.AnyAsync(t => t.Name == type))
             {
                 _context.MilestoneTypes.Add(new GTMS.Domain.Entities.Thesis.MilestoneType { Name = type, Description = "Standard milestone type" });
             }
        }
        
        await _context.SaveChangesAsync();
    }

    private async Task SeedRoleAsync(string roleName, string description)
    {
        if (!await _context.Roles.AnyAsync(r => r.Name == roleName))
        {
            _context.Roles.Add(new Role
            {
                Id = Guid.NewGuid(),
                Name = roleName,
                Description = description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
        }
    }

    // NOTE: In production, use a proper PasswordHasher (e.g., from Identity service)
    // For this simple seeder without DI complexity, we'll manually hash or use a placeholder if the Auth logic allows plain checks (it shouldn't).
    // Better: Helper method mimicking the BCrypt/Argon2 logic used in RegisterCommandHandler.
    private string HashPassword(string password)
    {
        // IMPORTANT: existing handlers use BCrypt.Net.BCrypt.HashPassword(password);
        // We should replicate that.
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}

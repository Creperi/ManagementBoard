using Microsoft.EntityFrameworkCore;

namespace BasicManagementBoard.Models;

public class ProjectContext : DbContext
{
    public ProjectContext(DbContextOptions<ProjectContext> options)
        : base(options)
    {
    }

    public DbSet<ProjectItem> ProjectItems { get; set; } = null!;
}
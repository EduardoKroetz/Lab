using Lab.Api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Lab.Api.Data;

public class LabDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public LabDbContext(DbContextOptions<LabDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}



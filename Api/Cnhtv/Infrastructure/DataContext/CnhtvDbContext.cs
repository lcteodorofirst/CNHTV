namespace Cnhtv.Infrastructure.DataContext;

using Microsoft.EntityFrameworkCore;
using Cnhtv.Domain.Models;

public class CnhtvDbContext : DbContext
{
    public virtual DbSet<TvDevice> TvDevices { get; set; }
    public virtual DbSet<TvConnectionHistory> TvConnectionHistory { get; set; }

    public CnhtvDbContext()
        : base() { }

    public CnhtvDbContext(DbContextOptions<CnhtvDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CnhtvDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}

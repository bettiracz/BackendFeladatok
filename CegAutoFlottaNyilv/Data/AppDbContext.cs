using Microsoft.EntityFrameworkCore;
using CegAutoFlottaNyilv.Models;

namespace CegAutoFlottaNyilv.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Ember> Ember => Set<Ember>();
    public DbSet<Auto> Auto => Set<Auto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Auto>().HasIndex(x => x.Rendszam).IsUnique();
        modelBuilder.Entity<Auto>().HasOne(x => x.Ember).WithMany(x => x.AutoLista)
            .HasForeignKey(x => x.EmberId).OnDelete(DeleteBehavior.SetNull);
    }
}

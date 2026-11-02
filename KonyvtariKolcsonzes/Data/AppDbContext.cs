using Microsoft.EntityFrameworkCore;
using KonyvtariKolcsonzes.Models;

namespace KonyvtariKolcsonzes.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Olvaso> Olvaso => Set<Olvaso>();
    public DbSet<Konyv> Konyv => Set<Konyv>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Konyv>().HasIndex(x => x.ISBN).IsUnique();
        modelBuilder.Entity<Konyv>().HasOne(x => x.Olvaso).WithMany(x => x.KonyvLista)
            .HasForeignKey(x => x.OlvasoId).OnDelete(DeleteBehavior.SetNull);
    }
}

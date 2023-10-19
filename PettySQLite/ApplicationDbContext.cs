using Microsoft.EntityFrameworkCore;
using PettySQLite.Models;
namespace PettySQLite;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
        Database.EnsureDeleted();
        //Database.EnsureCreated();

        if (!Initialized)
        {
            Initialized = true;
            SQLitePCL.Batteries_V2.Init();
            Database.Migrate();
        }
    }

    public static bool Initialized { get; protected set; }
    public DbSet<Settings> Settings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "petty.db");
            optionsBuilder.UseLazyLoadingProxies().UseSqlite($"Data Source={dbPath}");
            //Todo: add smart logging
            //optionsBuilder.LogTo(message => System.Diagnostics.Debug.WriteLine(message));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}

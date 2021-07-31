using DataAccessLib.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLib.External
{
    public class AppDbContext : IdentityDbContext
    {
        public static bool Initialized = false;
        public AppDbContext()
        {
            if (!Initialized)
            {
                Database.IsSqlite();
                Database.EnsureCreated();
                Initialized = true;
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("DataSource=app.db", sl => sl.MigrationsAssembly("WobigTools"));

        public DbSet<WatcherEvent> WatcherEvents { get; set; }
        public DbSet<WatcherAudit> WatcherAudits { get; set; }
    }
}

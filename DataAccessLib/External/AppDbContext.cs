using DataAccessLib.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace DataAccessLib.External
{
    public class AppDbContext : IdentityDbContext
    {
        public static bool Initialized = false;
        //public event EventHandler WatcherEventsChanged;
        //public event EventHandler WatcherAuditsChanged;

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

        //public void WatcherEventChanged(EventArgs e)
        //{
        //    EventHandler handler = WatcherEventsChanged;
        //    handler?.Invoke(this, e);
        //}

        //public void WatcherAuditChanged(EventArgs e)
        //{
        //    EventHandler handler = WatcherAuditsChanged;
        //    handler?.Invoke(this, e);
        //}

        public DbSet<WatcherEvent> WatcherEvents { get; set; }
        public DbSet<WatcherAudit> WatcherAudits { get; set; }
    }
}

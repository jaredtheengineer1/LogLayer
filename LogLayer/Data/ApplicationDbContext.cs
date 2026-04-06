using LogLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace LogLayer.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<LogEvent> Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<LogEvent>().Property(e => e.Metadata).HasColumnType("jsonb");
        }
    }
}
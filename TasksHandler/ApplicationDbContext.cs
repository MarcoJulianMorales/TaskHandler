using Microsoft.EntityFrameworkCore;
using TasksHandler.Entities;

namespace TasksHandler
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { 
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<Tasks>().Property(t => t.Tittle).HasMaxLength(250).IsRequired();
        }

        public DbSet<Tasks> Tasks { get; set; }
        public DbSet<Steps> Steps { get; set; }
        public DbSet<AttachedFile> AttachedFiles { get; set; }
    }
}

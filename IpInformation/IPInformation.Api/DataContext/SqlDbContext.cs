using IPInformation.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace IPInformation.Api.DataContext
{
    public class SqlDbContext : DbContext
    {
        public SqlDbContext(DbContextOptions<SqlDbContext> options) : base(options) { }

        public DbSet<IPDetailsExtended> IPDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var ipBuilder = modelBuilder.Entity<IPDetailsExtended>();

            ipBuilder.HasKey(h => h.Id);

            //ipBuilder.HasIndex(i => i.Ip);

        }
    }
}

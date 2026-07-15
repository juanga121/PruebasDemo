using Microsoft.EntityFrameworkCore;
using PruebasDemo.Domain;
using PruebasDemo.Domain.Entities;

namespace PruebasDemo.Infrastructure.Data
{
    public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
    {
        public DbSet<Credit> Credits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Credit>(entity =>
            {
                entity.Property(e => e.Amount).HasPrecision(DomainConstants.DecimalPrecision, DomainConstants.DecimalScale);
                entity.Property(e => e.Balance).HasPrecision(DomainConstants.DecimalPrecision, DomainConstants.DecimalScale);
                entity.Property(e => e.InterestRate).HasPrecision(DomainConstants.DecimalPrecision, DomainConstants.DecimalScale);
            });
        }
    }
}

using Microsoft.EntityFrameworkCore;
using PruebasDemo.Domain;
using PruebasDemo.Domain.Entities;

namespace PruebasDemo.Infrastructure.Data
{
    public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
    {
        public DbSet<CreditoEntity> Creditos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CreditoEntity>(entity =>
            {
                entity.Property(e => e.Monto).HasPrecision(DomainConstants.DecimalPrecision, DomainConstants.DecimalScale);
                entity.Property(e => e.Saldo).HasPrecision(DomainConstants.DecimalPrecision, DomainConstants.DecimalScale);
                entity.Property(e => e.TasaInteres).HasPrecision(DomainConstants.DecimalPrecision, DomainConstants.DecimalScale);
            });
        }
    }
}

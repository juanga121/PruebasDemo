using Microsoft.EntityFrameworkCore;
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
                entity.Property(e => e.Monto).HasPrecision(18, 2);
                entity.Property(e => e.Saldo).HasPrecision(18, 2);
                entity.Property(e => e.TasaInteres).HasPrecision(18, 2);
            });
        }
    }
}

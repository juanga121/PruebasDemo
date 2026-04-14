using Microsoft.EntityFrameworkCore;
using PruebasDemo.Domain.Entities;

namespace PruebasDemo.Infrastructure.Data
{
    public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
    {
        public DbSet<CreditoEntity> Creditos { get; set; }
    }
}

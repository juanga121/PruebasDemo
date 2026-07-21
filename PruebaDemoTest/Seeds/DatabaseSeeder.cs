using PruebasDemo.Domain.Entities;
using PruebasDemo.Infrastructure.Data;

namespace PruebaDemoTest.Seeds;

public static class DatabaseSeeder
{
    public static void ResetDatabase(this DataContext db)
    {
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
    }

    public static Guid SeedCredit(this DataContext db, Credit credit)
    {
        db.Credits.Add(credit);
        db.SaveChanges();
        return credit.Id;
    }
}

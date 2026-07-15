using PruebasDemo.Domain.DTO;
using PruebasDemo.Domain.Entities;
using PruebasDemo.Domain.Enums;
using PruebasDemo.Infrastructure.Data;

namespace PruebaDemoTest.Seeds;

public static class Seeded
{
    public static readonly Guid CreditoId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890");
    public static readonly Guid PagarId = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901");

    public static CreditoEntity CreditoActivo => new()
    {
        Id = CreditoId, Monto = 100, Saldo = 100,
        TasaInteres = 10, Meses = 12, Estado = CreditoEstado.Activo
    };

    public static CreditoEntity CreditoSaldo50 => new()
    {
        Id = PagarId, Monto = 50, Saldo = 50,
        TasaInteres = 10, Meses = 12, Estado = CreditoEstado.Activo
    };

    public static CreditoDto CrearCredito => new()
    {
        Monto = 100, TasaInteres = 10, Meses = 12
    };

    public static CreditoDto ActualizarCredito => new()
    {
        Monto = 200, TasaInteres = 10, Meses = 24
    };

    public static CreditoEntity CreditoPersonalizado => new()
    {
        Id = Guid.NewGuid(), Monto = 200,
        TasaInteres = 5, Meses = 6,
        Saldo = 200, Estado = CreditoEstado.Activo
    };

    public static CreditoDto CreditoUpdateDto => new()
    {
        Monto = 500, TasaInteres = 8, Meses = 10
    };

    public static CreditoDto ConMontoCero => new() { Monto = 0 };
    public static CreditoDto ConTasaNegativa => new() { TasaInteres = -1 };
    public static CreditoDto ConMesesCero => new() { Meses = 0 };

    public static PagarCuotaDto PagoParcial => new()
    {
        Id = CreditoId, MontoPago = 30
    };

    public static PagarCuotaDto PagoExacto => new()
    {
        Id = PagarId, MontoPago = 50
    };

    public static PagarCuotaDto Pago(decimal monto, Guid id) => new()
    {
        Id = id, MontoPago = monto
    };

    public static List<CreditoEntity> ListaCreditos =>
    [
        new() { Id = Guid.NewGuid(), Monto = 100 },
        new() { Id = Guid.NewGuid(), Monto = 200 }
    ];

    public static void ResetDatabase(this DataContext db)
    {
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
    }

    public static Guid SeedCredito(this DataContext db, CreditoEntity credito)
    {
        db.Creditos.Add(credito);
        db.SaveChanges();
        return credito.Id;
    }
}

namespace PruebaDemoTest.Constants;

public static class ApiRoutes
{
    public const string Credito = "/api/credito";
    public static string CreditoPorId(Guid id) => $"/api/credito/{id}";
    public const string PagarCuota = "/api/credito/pagar";
}

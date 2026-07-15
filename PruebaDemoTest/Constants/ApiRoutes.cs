namespace PruebaDemoTest.Constants;

public static class ApiRoutes
{
    public const string Credit = "/api/credit";
    public static string CreditById(Guid id) => $"/api/credit/{id}";
    public const string PayInstallment = "/api/credit/pagar";
}

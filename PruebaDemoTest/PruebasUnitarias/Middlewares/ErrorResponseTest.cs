using PruebasDemo.Middlewares;

namespace PruebaDemoTest.PruebasUnitarias.Middlewares;

public class ErrorResponseTest
{
    [Fact]
    public void ErrorResponse_PuedeCrearseConValores()
    {
        var response = new ErrorResponse
        {
            Exito = false,
            Mensaje = "Error test",
            TraceId = "abc123",
            Errores = [new ErrorDetail { Campo = "Monto", Mensaje = "Debe ser mayor a 0" }]
        };

        Assert.False(response.Exito);
        Assert.Equal("Error test", response.Mensaje);
        Assert.Equal("abc123", response.TraceId);
        Assert.Single(response.Errores!);
        Assert.Equal("Monto", response.Errores!.First().Campo);
    }

    [Fact]
    public void ErrorDetail_PuedeCrearseConValores()
    {
        var detail = new ErrorDetail
        {
            Campo = "TasaInteres",
            Mensaje = "Debe ser positiva"
        };

        Assert.Equal("TasaInteres", detail.Campo);
        Assert.Equal("Debe ser positiva", detail.Mensaje);
    }
}

using PruebasDemo.Middlewares;

namespace PruebaDemoTest.UnitTests.Middlewares;

public class ErrorResponseTest
{
    [Fact]
    public void ErrorResponse_CanBeCreatedWithValues()
    {
        var response = new ErrorResponse
        {
            Success = false,
            Message = "Error test",
            TraceId = "abc123",
            Errors = [new ErrorDetail { Field = "Amount", Message = "Debe ser mayor a 0" }]
        };

        Assert.False(response.Success);
        Assert.Equal("Error test", response.Message);
        Assert.Equal("abc123", response.TraceId);
        Assert.Single(response.Errors!);
        Assert.Equal("Amount", response.Errors!.First().Field);
    }

    [Fact]
    public void ErrorDetail_CanBeCreatedWithValues()
    {
        var detail = new ErrorDetail
        {
            Field = "InterestRate",
            Message = "Debe ser positiva"
        };

        Assert.Equal("InterestRate", detail.Field);
        Assert.Equal("Debe ser positiva", detail.Message);
    }
}

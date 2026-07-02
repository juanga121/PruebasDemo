namespace PruebasDemo.Middlewares
{
    public class ErrorResponse
    {
        public bool Exito { get; set; }
        public string? Mensaje { get; set; }
        public string? TraceId { get; set; }
        public IEnumerable<ErrorDetail>? Errores { get; set; }
    }

    public class ErrorDetail
    {
        public string? Campo { get; set; }
        public string? Mensaje { get; set; }
    }
}

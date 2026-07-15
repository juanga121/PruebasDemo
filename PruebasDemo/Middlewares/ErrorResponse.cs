namespace PruebasDemo.Middlewares
{
    public class ErrorResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? TraceId { get; set; }
        public IEnumerable<ErrorDetail>? Errors { get; set; }
    }

    public class ErrorDetail
    {
        public string? Field { get; set; }
        public string? Message { get; set; }
    }
}

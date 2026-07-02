using System.Net;
using System.Text.Json;
 using FluentValidation;
using PruebasDemo.Application.Resources.Constants;

namespace PruebasDemo.Middlewares
{
    public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ExceptionMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex) when (!IsCatastrophic(ex))
            {
                var traceId = httpContext.TraceIdentifier;
                var method = httpContext.Request.Method;
                var path = httpContext.Request.Path;

                _logger.LogError(ex, LogTemplates.ErrorNoControlado, method, path, ex.Message);

                await HandleExceptionAsync(httpContext, ex, traceId);
            }
        }

        private static bool IsCatastrophic(Exception ex) =>
            ex is OutOfMemoryException or ThreadAbortException;

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception, string traceId)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, response) = exception switch
            {
                ValidationException validationException => (
                    (int)HttpStatusCode.BadRequest,
                    (object)new ErrorResponse
                    {
                        Exito = false,
                        Mensaje = "Errores de validación",
                        TraceId = traceId,
                        Errores = validationException.Errors.Select(e => new ErrorDetail
                        {
                            Campo = e.PropertyName,
                            Mensaje = e.ErrorMessage
                        })
                    }),

                ArgumentException => (
                    (int)HttpStatusCode.BadRequest,
                    new ErrorResponse { Exito = false, Mensaje = exception.Message, TraceId = traceId }),

                InvalidOperationException => (
                    (int)HttpStatusCode.BadRequest,
                    new ErrorResponse { Exito = false, Mensaje = exception.Message, TraceId = traceId }),

                KeyNotFoundException => (
                    (int)HttpStatusCode.NotFound,
                    new ErrorResponse { Exito = false, Mensaje = exception.Message, TraceId = traceId }),

                UnauthorizedAccessException => (
                    (int)HttpStatusCode.Unauthorized,
                    new ErrorResponse { Exito = false, Mensaje = exception.Message, TraceId = traceId }),

                _ => (
                    (int)HttpStatusCode.InternalServerError,
                    new ErrorResponse { Exito = false, Mensaje = exception.Message, TraceId = traceId })
            };

            context.Response.StatusCode = statusCode;

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }
}

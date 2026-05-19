using System.Net;
using System.Text.Json;
using FluentValidation;

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error no controlado: {Message}", ex.Message);
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            int statusCode = (int)HttpStatusCode.InternalServerError;
            object response;

            switch (exception)
            {
                case ValidationException validationException:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    response = new
                    {
                        exito = false,
                        mensaje = "Errores de validación",
                        errores = validationException.Errors.Select(e => new
                        {
                            campo = e.PropertyName,
                            mensaje = e.ErrorMessage
                        })
                    };
                    break;

                case ArgumentException:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    response = new
                    {
                        exito = false,
                        mensaje = exception.Message
                    };
                    break;

                case KeyNotFoundException:
                    statusCode = (int)HttpStatusCode.NotFound;
                    response = new
                    {
                        exito = false,
                        mensaje = exception.Message
                    };
                    break;

                case UnauthorizedAccessException:
                    statusCode = (int)HttpStatusCode.Unauthorized;
                    response = new
                    {
                        exito = false,
                        mensaje = exception.Message
                    };
                    break;

                case InvalidOperationException:
                    statusCode = (int)HttpStatusCode.Conflict;
                    response = new
                    {
                        exito = false,
                        mensaje = exception.Message
                    };
                    break;

                default:
                    response = new
                    {
                        exito = false,
                        mensaje = "Ha ocurrido un error interno del servidor"
                    };
                    break;
            }

            context.Response.StatusCode = statusCode;

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }
}

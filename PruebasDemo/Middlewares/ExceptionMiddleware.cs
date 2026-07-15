using System.Net;
using System.Text.Json;
using FluentValidation;
using PruebasDemo.Application.Resources;
using PruebasDemo.Application.Resources.Constants;
using PruebasDemo.Constants;

namespace PruebasDemo.Middlewares
{
    public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ExceptionMiddleware> _logger = logger;
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex) when (!IsCatastrophic(ex))
            {
                string traceId = httpContext.TraceIdentifier;
                string method = httpContext.Request.Method;
                PathString path = httpContext.Request.Path;

                _logger.LogError(ex, LogTemplates.UnhandledError, method, path, ex.Message);

                await HandleExceptionAsync(httpContext, ex, traceId);
            }
        }

        private static bool IsCatastrophic(Exception ex) =>
            ex is OutOfMemoryException or ThreadAbortException;

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception, string traceId)
        {
            context.Response.ContentType = ApiConstants.ContentTypeJson;

            (int statusCode, object response) = exception switch
            {
                ValidationException validationException => (
                    (int)HttpStatusCode.BadRequest,
                    (object)new ErrorResponse
                    {
                        Success = false,
                        Message = Messages.ValidationErrors,
                        TraceId = traceId,
                        Errors = validationException.Errors.Select(failure => new ErrorDetail
                        {
                            Field = failure.PropertyName,
                            Message = failure.ErrorMessage
                        })
                    }),

                ArgumentException => (
                    (int)HttpStatusCode.BadRequest,
                    new ErrorResponse { Success = false, Message = exception.Message, TraceId = traceId }),

                InvalidOperationException => (
                    (int)HttpStatusCode.BadRequest,
                    new ErrorResponse { Success = false, Message = exception.Message, TraceId = traceId }),

                KeyNotFoundException => (
                    (int)HttpStatusCode.NotFound,
                    new ErrorResponse { Success = false, Message = exception.Message, TraceId = traceId }),

                UnauthorizedAccessException => (
                    (int)HttpStatusCode.Unauthorized,
                    new ErrorResponse { Success = false, Message = exception.Message, TraceId = traceId }),

                _ => (
                    (int)HttpStatusCode.InternalServerError,
                    new ErrorResponse { Success = false, Message = exception.Message, TraceId = traceId })
            };

            context.Response.StatusCode = statusCode;

            string json = JsonSerializer.Serialize(response, _jsonOptions);

            await context.Response.WriteAsync(json);
        }
    }
}

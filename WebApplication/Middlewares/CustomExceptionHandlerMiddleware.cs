using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using WebApplication.Dto;

namespace WebApplication.Middlewares
{
    public class CustomExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;

            var result = string.Empty;

            switch (exception)
            {
                case ValidationException validationException:
                    code = HttpStatusCode.Conflict;
                    result = JsonSerializer.Serialize(new ErrorResponse
                    {
                        StatusCode = HttpStatusCode.Conflict.ToString(),
                        Message = validationException.Message
                    });
                    break;
                case BadRequestException badRequestException:
                    code = HttpStatusCode.BadRequest;
                    result = JsonSerializer.Serialize(new ErrorResponse
                    {
                        StatusCode = HttpStatusCode.BadRequest.ToString(),
                        Message = badRequestException.Message
                    });
                    break;
                case NotFoundException notFoundException:
                    code = HttpStatusCode.NotFound;
                    result = JsonSerializer.Serialize(new ErrorResponse
                    {
                        StatusCode = HttpStatusCode.NotFound.ToString(),
                        Message = notFoundException.Message
                    });
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            if (result == string.Empty)
            {
                result = JsonSerializer.Serialize(new
                {
                    error = exception.Message,
                    stackTrace = exception.StackTrace
                });
            }

            return context.Response.WriteAsync(result);
        }
    }

    public static class CustomExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomExceptionHandlerMiddleware>();
        }
    }
}

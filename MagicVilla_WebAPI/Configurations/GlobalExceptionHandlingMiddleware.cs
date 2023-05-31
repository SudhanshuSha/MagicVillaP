using MagicVilla_WebAPI.Exceptions;
using System.Net;
using System.Text.Json;
using KeyNotFoundException = MagicVilla_WebAPI.Exceptions.KeyNotFoundException;
using NotImplementedException = MagicVilla_WebAPI.Exceptions.NotImplementedException;
using UnauthorizedAccessException = MagicVilla_WebAPI.Exceptions.UnauthorizedAccessException;

namespace MagicVilla_WebAPI.Configurations
{
    // once the request delegate jumps from one middeleware to another
    public class GlobalExceptionHandlingMiddleware
    {
        // we need to recive the request and pass it to further constructor
        private readonly RequestDelegate _next;

        // we will recive the request using the constructor
        public GlobalExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // middleware will excute some stuff and pass request to other middleware
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // if there is an exeption at any layer we passing it up the order and handling it here
                // this mehtod will identify what type of error it is and process it and return the respons eback
                await HandleExceptionAsync(context, ex);
            }

        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            HttpStatusCode status;
            var stackTrace = string.Empty;
            string message = string.Empty;

            var exceptionType = ex.GetType();

            if (exceptionType == typeof(NotFoundException))
            {
                message = ex.Message;
                status = HttpStatusCode.NotFound;
                stackTrace = ex.StackTrace;
            }
            else if (exceptionType == typeof(BadRequestException))
            {
                message = ex.Message;
                status = HttpStatusCode.BadRequest;
                stackTrace = ex.StackTrace;
            }
            else if (exceptionType == typeof(NotImplementedException))
            {
                message = ex.Message;
                status = HttpStatusCode.NotImplemented;
                stackTrace = ex.StackTrace;
            }
            else if (exceptionType == typeof(KeyNotFoundException))
            {
                message = ex.Message;
                status = HttpStatusCode.NotImplemented;
                stackTrace = ex.StackTrace;
            }
            else if (exceptionType == typeof(UnauthorizedAccessException))
            {
                message = ex.Message;
                status = HttpStatusCode.Unauthorized;
                stackTrace = ex.StackTrace;
            }
            else
            {
                message = ex.Message;
                status = HttpStatusCode.InternalServerError;
                stackTrace = ex.StackTrace;
            }

            var exceptionResult = JsonSerializer.Serialize(new { error = message, stackTrace = stackTrace });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;

            return context.Response.WriteAsync(exceptionResult);


        }
    }
}

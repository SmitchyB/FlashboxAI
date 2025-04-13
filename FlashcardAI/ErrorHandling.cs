using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FitnessTracker.Exceptions
{
    // Base application exception
    public class AppException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public bool IsOperational { get; }

        public AppException(string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError, bool isOperational = true)
            : base(message)
        {
            StatusCode = statusCode;
            IsOperational = isOperational;
        }
    }

    // Not found exception
    public class NotFoundException : AppException
    {
        public NotFoundException(string message = "Resource not found")
            : base(message, HttpStatusCode.NotFound)
        {
        }
    }

    // Validation exception
    public class ValidationException : AppException
    {
        public ValidationException(string message = "Validation failed")
            : base(message, HttpStatusCode.BadRequest)
        {
        }
    }

    // Authentication exception
    public class AuthenticationException : AppException
    {
        public AuthenticationException(string message = "Authentication failed")
            : base(message, HttpStatusCode.Unauthorized)
        {
        }
    }

    // Authorization exception
    public class AuthorizationException : AppException
    {
        public AuthorizationException(string message = "Not authorized")
            : base(message, HttpStatusCode.Forbidden)
        {
        }
    }

    // Global exception handler for API controllers
    public class GlobalExceptionHandler : ControllerBase
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("/error")]
        public IActionResult HandleError()
        {
            var exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = exceptionHandlerFeature?.Error;

            if (exception == null)
                return Problem();

            _logger.LogError(exception, "An unhandled exception occurred");

            var statusCode = exception switch
            {
                NotFoundException => (int)HttpStatusCode.NotFound,
                ValidationException => (int)HttpStatusCode.BadRequest,
                AuthenticationException => (int)HttpStatusCode.Unauthorized,
                AuthorizationException => (int)HttpStatusCode.Forbidden,
                AppException appEx => (int)appEx.StatusCode,
                _ => (int)HttpStatusCode.InternalServerError
            };

            var title = exception switch
            {
                NotFoundException => "Resource Not Found",
                ValidationException => "Validation Error",
                AuthenticationException => "Authentication Failed",
                AuthorizationException => "Authorization Failed",
                _ => "An error occurred"
            };

            return Problem(
                detail: exception.Message,
                statusCode: statusCode,
                title: title
            );
        }
    }

    // Extension method for async operations with retry
    public static class AsyncExtensions
    {
        public static async Task<T> WithRetryAsync<T>(
            Func<Task<T>> operation,
            int maxRetries = 3,
            int initialDelayMs = 300,
            ILogger logger = null)
        {
            int retryCount = 0;
            int delay = initialDelayMs;

            while (true)
            {
                try
                {
                    return await operation();
                }
                catch (Exception ex)
                {
                    retryCount++;

                    // Don't retry for client errors (4xx)
                    if (ex is AppException appEx && (int)appEx.StatusCode >= 400 && (int)appEx.StatusCode < 500)
                    {
                        throw;
                    }

                    if (retryCount > maxRetries)
                    {
                        logger?.LogError(ex, "Operation failed after {RetryCount} retries", retryCount);
                        throw;
                    }

                    logger?.LogWarning(ex, "Operation failed, retrying ({RetryCount}/{MaxRetries})", retryCount, maxRetries);

                    // Exponential backoff
                    await Task.Delay(delay);
                    delay *= 2;
                }
            }
        }

        public static async Task WithRetryAsync(
            Func<Task> operation,
            int maxRetries = 3,
            int initialDelayMs = 300,
            ILogger logger = null)
        {
            await WithRetryAsync(async () =>
            {
                await operation();
                return true;
            }, maxRetries, initialDelayMs, logger);
        }
    }

    // Extension methods for setting up global error handling
    public static class ErrorHandlingExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler("/error");

            return app;
        }
    }
}
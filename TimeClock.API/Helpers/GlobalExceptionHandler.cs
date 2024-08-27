using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Text;

namespace TimeClock.Api.Helpers
{
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger _logger;
        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            this._logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            string seperator = new('*', 50);
            string divider = new('-', 50);
            StringBuilder builder = new(seperator);
            Exception? current = exception;

            if ((exception as OperationCanceledException) != null || (exception as TaskCanceledException) != null)
            {
                builder.AppendLine(seperator);
                builder.AppendLine(seperator);
                builder.AppendLine();
                builder.AppendLine($"Message: Operation was cancelled by CancellationToken causing proper Exception");
                builder.AppendLine();
                builder.AppendLine(seperator);
                builder.AppendLine(seperator);
                this._logger.LogError(EventIds.ActionCancelled, "{Message}", builder);
                httpContext.Response.StatusCode = StatusCodes.Status499ClientClosedRequest;

                return true;
            }

            builder.AppendLine(seperator);

            while (current != null)
            {
                builder.AppendLine();
                builder.AppendLine($"Message: {current.Message}");
                builder.AppendLine();
                builder.AppendLine(divider);
                builder.AppendLine();
                builder.AppendLine($"Data: ");

                foreach(DictionaryEntry data in current.Data)
                    builder.AppendLine($"{data.Key} : {data.Value}");

                builder.AppendLine();
                builder.AppendLine(divider);
                builder.AppendLine();
                builder.AppendLine("Stack: ");
                builder.AppendLine(current.StackTrace);

                builder.AppendLine();
                builder.AppendLine(seperator);

                builder.AppendLine();

                current = current.InnerException;
            }

            builder.AppendLine(seperator);
            builder.AppendLine(seperator);

            this._logger.LogError(EventIds.UnknownException, "{Message}", builder);

            var problemDetails = new ProblemDetails()
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Server Error",
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
                Detail = "Exceptional Case"
            };

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}

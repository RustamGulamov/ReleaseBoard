using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ReleaseBoard.Domain.Core.Exceptions;

namespace ReleaseBoard.Web.Middlewares
{
    /// <summary>
    /// Middleware для отлова исключений валидации.
    /// </summary>
    public class ValidationExceptionsHandler
    {
        private readonly RequestDelegate requestDelegate;

        /// <summary>
        /// .ctor.
        /// </summary>
        /// <param name="requestDelegate">Следующий обработчик запроса <see cref="RequestDelegate"/>.</param>
        public ValidationExceptionsHandler(RequestDelegate requestDelegate)
        {
            this.requestDelegate = requestDelegate;
        }

        /// <summary>
        /// Отлавливает необработанные исключения валидации и возвращает в ответ BadRequest.
        /// </summary>
        /// <param name="httpContext">Контекст http-запроса.</param>
        /// <returns><see cref="Task"/>.</returns>
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await requestDelegate.Invoke(httpContext);
            }
            catch (ValidationException exception)
            {
                await WriteBadRequestResponse(httpContext, exception);
            }
        }

        private async Task WriteBadRequestResponse(HttpContext httpContext, ValidationException exception)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            if (exception.ValidationError != null)
            {
                await httpContext.Response.WriteAsJsonAsync(exception.ValidationError);
            }
            else
            {
                await httpContext.Response.WriteAsync(exception.Message);
            }
        }
    }
}

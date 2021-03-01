using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ReleaseBoard.Web.Middlewares
{
    /// <summary>
    /// Middleware для отлова исключений валидации.
    /// </summary>
    public class HttpErrorResponseExceptionsHandler
    {
        private readonly RequestDelegate requestDelegate;

        /// <summary>
        /// .ctor.
        /// </summary>
        /// <param name="requestDelegate">Следующий обработчик запроса <see cref="RequestDelegate"/>.</param>
        public HttpErrorResponseExceptionsHandler(RequestDelegate requestDelegate)
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
            catch (HttpErrorResponseException e) when (e.StatusCode == HttpStatusCode.BadRequest)
            {
                string jsonMessage = e.Message.Replace("BadRequest: ", string.Empty);

                await WriteBadRequestResponse(httpContext, jsonMessage);
            }
        }

        private async Task WriteBadRequestResponse(HttpContext httpContext, string message)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            await httpContext.Response.WriteAsync(message);
        }
    }
}

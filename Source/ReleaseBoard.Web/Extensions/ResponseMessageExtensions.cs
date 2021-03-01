using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace ReleaseBoard.Web.Extensions
{
    /// <summary>
    /// Методы расширений для Controller'ов.
    /// </summary>
    public static class ControllerExtensions
    {
        /// <summary>
        /// Конвертирует <see cref="IResponseMessage"/> в <see cref="ActionResult"/> с помощью указанной лямбда-функции, либо возвращает BadRequest.
        /// </summary>
        /// <typeparam name="TResposeType">Тип ответа, передаваемый в лямбда функцию.</typeparam>
        /// <typeparam name="TResult">Желаемый тип для <see cref="ActionResult"/>.</typeparam>
        /// <param name="response">Пришедший ответ.</param>
        /// <param name="continueWith">Лямбла-функция.</param>
        /// <returns>Ответ веб сервера.</returns>
        public static ActionResult<TResult> ToActionResult<TResposeType, TResult>(this IResponseMessage response, Func<TResposeType, ActionResult<TResult>> continueWith)
        {
            return response switch
            {
                ErrorResponseMessage e => new ObjectResult(e.Message) { StatusCode = (int)HttpStatusCode.BadRequest },
                _ => continueWith((TResposeType)response)
            };
        }
    }
}

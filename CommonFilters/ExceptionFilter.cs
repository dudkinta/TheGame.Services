using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace CommonLibs
{
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            // Логируем ошибку, если нужно
            // log.Error(context.Exception);

            var errorResponse = new ErrorResponse
            {
                Error = "Внутренняя ошибка сервера" // Можем добавить более специфичное сообщение
            };

            context.Result = new JsonResult(errorResponse)
            {
                StatusCode = (int)HttpStatusCode.InternalServerError // 500 код ошибки
            };

            context.ExceptionHandled = true; // Помечаем исключение как обработанное
        }
    }
}

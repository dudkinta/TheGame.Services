using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace CommonLibs
{
    public class BadRequestFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {

        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is BadRequestObjectResult badRequestResult)
            {
                var result = (BadRequestObjectResult)context.Result;
                if (result.Value is ErrorResponse)
                {
                    var error = (ErrorResponse)result.Value;
                    var errorResponse = new ErrorResponse { Error = error.Error };
                    context.Result = new JsonResult(errorResponse)
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest
                    };
                }
                else
                {
                    var errorResponse = new ErrorResponse { Error = badRequestResult.Value?.ToString() };
                    context.Result = new JsonResult(errorResponse)
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest
                    };
                }
            }
        }
    }
}

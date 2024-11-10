using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using DockScripter.Domain.Dtos.Responses;

public class GlobalExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var errorResponse = new ErrorResponseDto
        {
            Timestamp = DateTime.UtcNow,
            Status = (int)HttpStatusCode.InternalServerError,
            Error = HttpStatusCode.InternalServerError.ToString(),
            Message = context.Exception.Message,
            Path = context.HttpContext.Request.Path
        };

        if (context.Exception is UnauthorizedAccessException)
        {
            errorResponse.Status = (int)HttpStatusCode.Forbidden;
            errorResponse.Error = HttpStatusCode.Forbidden.ToString();
            errorResponse.Message = "Access Denied: " + context.Exception.Message;
            context.Result = new ObjectResult(errorResponse)
            {
                StatusCode = (int)HttpStatusCode.Forbidden
            };
        }
        else
        {
            context.Result = new ObjectResult(errorResponse)
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };
        }

        context.ExceptionHandled = true;
    }
}
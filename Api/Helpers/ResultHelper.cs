using Api.Common;
using Microsoft.AspNetCore.Mvc;

namespace Api.Helpers;

public static class ResultHelper
{
    /// <summary>
    /// Converts a failed Result to an appropriate ActionResult based on the error type
    /// </summary>
    public static ActionResult ToActionResult(Result result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException("Cannot convert successful result to error response");
        }

        var errorResponse = new { error = result.ErrorMessage };

        return result.ErrorType switch
        {
            ErrorTypeEnum.NotFound => new NotFoundObjectResult(errorResponse),
            ErrorTypeEnum.Unauthorized => new UnauthorizedObjectResult(errorResponse),
            ErrorTypeEnum.ValidationError => new BadRequestObjectResult(errorResponse),
            ErrorTypeEnum.BusinessRuleViolation => new BadRequestObjectResult(errorResponse),
            ErrorTypeEnum.InternalError => new ObjectResult(errorResponse) { StatusCode = 500 },
            _ => new BadRequestObjectResult(errorResponse)
        };
    }
}
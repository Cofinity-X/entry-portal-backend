using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;

namespace Org.Eclipse.TractusX.Portal.Backend.Framework.ErrorHandling.Web;

public class ModelBindingErrorHandler
{
    private static class ErrorMessageConstants
    {
        public const string BytePositionInLine = "BytePositionInLine";
        public const string LineNumber = "LineNumber";
        public const string InvalidInputData = "Invalid input data";
    }

    public static IActionResult CreateInvalidModelStateResponse(ActionContext context)
    {
        var problemDetailsFactory = context.HttpContext.RequestServices
            .GetRequiredService<ProblemDetailsFactory>();

        var problemDetails = problemDetailsFactory.CreateValidationProblemDetails(
            context.HttpContext,
            context.ModelState,
            statusCode: StatusCodes.Status400BadRequest
        );

        problemDetails.Errors = context.ModelState
            .Where(e => e.Value?.Errors.Count > 0)
            .ToDictionary(
                e => e.Key,
                e => TransformErrorMessages(e.Value!.Errors)
            );

        return new BadRequestObjectResult(problemDetails);
    }

    private static readonly (string Contains, string Replacement)[] ErrorRules =
    [
        (ErrorMessageConstants.BytePositionInLine, ErrorMessageConstants.InvalidInputData),
        (ErrorMessageConstants.LineNumber, ErrorMessageConstants.InvalidInputData)
    ];

    private static string[] TransformErrorMessages(ModelErrorCollection errors)
    {
        return errors.Select(err =>
        {
            foreach (var (contains, replacement) in ErrorRules)
            {
                if (err.ErrorMessage.Contains(contains))
                    return replacement;
            }
            return err.ErrorMessage;
        }).ToArray();
    }
}

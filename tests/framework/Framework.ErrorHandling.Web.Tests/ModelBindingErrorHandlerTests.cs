using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Xunit;

namespace Org.Eclipse.TractusX.Portal.Backend.Framework.ErrorHandling.Web.Tests;

public class ModelBindingErrorHandlerTests
{
    private readonly ProblemDetailsFactory _problemDetailsFactory;
    private readonly HttpContext _httpContext;

    public ModelBindingErrorHandlerTests()
    {
        // Mock HttpContext and ProblemDetailsFactory properly
        _httpContext = new DefaultHttpContext();
        _problemDetailsFactory = A.Fake<ProblemDetailsFactory>();

        // Ensure the HttpContext returns the mocked ProblemDetailsFactory
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(typeof(ProblemDetailsFactory)))
            .Returns(_problemDetailsFactory);
        _httpContext.RequestServices = serviceProvider;
    }

    [Fact]
    public void CreateInvalidModelStateResponse_ShouldReturnBadRequest_WithModifiedErrors()
    {
        // Arrange
        var modelState = new ModelStateDictionary();
        modelState.AddModelError("Field1", "BytePositionInLine error at position 5");
        modelState.AddModelError("Field2", "Some other validation error");

        var actionContext = new ActionContext(
            _httpContext,
            new Microsoft.AspNetCore.Routing.RouteData(),
            new Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor(),
            modelState
        );

        var validationProblemDetails = new ValidationProblemDetails(modelState)
        {
            Status = StatusCodes.Status400BadRequest
        };

        // Mock ProblemDetailsFactory to return expected ProblemDetails
        A.CallTo(() => _problemDetailsFactory.CreateValidationProblemDetails(
            _httpContext, modelState, StatusCodes.Status400BadRequest, null, null, null, null))
            .Returns(validationProblemDetails);

        // Act
        var result = ModelBindingErrorHandler.CreateInvalidModelStateResponse(actionContext);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();

        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().BeOfType<ValidationProblemDetails>();

        var problemDetails = badRequestResult.Value as ValidationProblemDetails;
        problemDetails!.Errors.Should().ContainKey("Field1");
        problemDetails.Errors["Field1"].Should().ContainSingle()
            .Which.Should().Be("Invalid input data");
        problemDetails.Errors["Field2"].Should().ContainSingle()
            .Which.Should().Be("Some other validation error");
    }
}

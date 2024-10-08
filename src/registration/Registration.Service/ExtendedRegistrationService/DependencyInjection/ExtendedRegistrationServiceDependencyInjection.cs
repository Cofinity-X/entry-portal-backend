using Microsoft.Extensions.Options;
using Org.Eclipse.TractusX.Portal.Backend.ExtendedRegistration.Service;
using Org.Eclipse.TractusX.Portal.Backend.ExtendedRegistration.Service.BusinessLogic;
using Org.Eclipse.TractusX.Portal.Backend.Framework.HttpClientExtensions;
using Org.Eclipse.TractusX.Portal.Backend.Framework.Token;

namespace Org.Eclipse.TractusX.Portal.Backend.ExtendedRegistration.DependencyInjection;

public static class ExtendedRegistrationServiceDependencyInjection
{
    public static IServiceCollection AddExtendedRegistrationService(this IServiceCollection services, IConfigurationSection section)
    {
        services.AddOptions<ExtendedRegistrationServiceSettings>()
            .Bind(section)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddTransient<LoggingHandler<HubspotService>>();
        var sp = services.BuildServiceProvider();
        var settings = sp.GetRequiredService<IOptions<ExtendedRegistrationServiceSettings>>();
        var baseAddress = settings.Value.BaseAddress;
        services.AddCustomHttpClientWithAuthentication<HubspotService>(baseAddress.EndsWith('/') ? baseAddress : $"{baseAddress}/");

        services
          .AddTransient<ITokenService, TokenService>()
          .AddTransient<IHubspotService, HubspotService>()
          .AddTransient<IHubspotBusinessLogic, HubspotBusinessLogic>();
        return services;
    }
}

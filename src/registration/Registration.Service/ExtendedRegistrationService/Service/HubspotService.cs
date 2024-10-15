using Microsoft.Extensions.Options;
using Org.Eclipse.TractusX.Portal.Backend.ExtendedRegistration.Service.Model;
using Org.Eclipse.TractusX.Portal.Backend.Framework.ErrorHandling;
using Org.Eclipse.TractusX.Portal.Backend.Framework.HttpClientExtensions;
using Org.Eclipse.TractusX.Portal.Backend.Framework.Token;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Org.Eclipse.TractusX.Portal.Backend.ExtendedRegistration.Service;

public class HubspotService : IHubspotService
{
    private readonly ITokenService _tokenService;
    private readonly ExtendedRegistrationServiceSettings _settings;
    private static readonly JsonSerializerOptions Options = new()
    {
        Converters =
        {
            new JsonStringEnumConverter(allowIntegerValues: false),
        },
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public HubspotService(ITokenService tokenService, IOptions<ExtendedRegistrationServiceSettings> options)
    {
        _tokenService = tokenService;
        _settings = options.Value;
    }

    public async Task<IEnumerable<HubspotProductResponse>> GetProductsByCompanyRoleAsync(string[] companyRoles, CancellationToken cancellationToken)
    {
        using var httpClient = await _tokenService.GetAuthorizedClient<HubspotService>(_settings, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);

        var request = new { companyRoles };
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var result = await httpClient.PostAsync($"api/get-products-by-role", content, cancellationToken)
            .CatchingIntoServiceExceptionFor("hubspot-get-products", HttpAsyncResponseMessageExtension.RecoverOptions.INFRASTRUCTURE).ConfigureAwait(false);
        try
        {
            var response = await result.Content
                .ReadFromJsonAsync<IEnumerable<HubspotProductResponse>>(Options, cancellationToken)
                .ConfigureAwait(ConfigureAwaitOptions.None) ?? throw new ServiceException("Access to external system Hubspot did not return a response");
            return response;
        }
        catch (JsonException je)
        {
            throw new ServiceException($"Access to external system Hubspot did not return a valid json response: {je.Message}");
        }
    }
    public async Task<HubspotCompanyResponse> GetHubspotCompanyDetailsAsync(string portalCompanyId, CancellationToken cancellationToken)
    {
        using var httpClient = await _tokenService.GetAuthorizedClient<HubspotService>(_settings, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);

        var request = new { portal_company_id = portalCompanyId };
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var result = await httpClient.PostAsync($"api/check-companies", content, cancellationToken)
            .CatchingIntoServiceExceptionFor("hubspot-get-company", HttpAsyncResponseMessageExtension.RecoverOptions.INFRASTRUCTURE).ConfigureAwait(false);
        try
        {
            var response = await result.Content
                .ReadFromJsonAsync<HubspotCompanyResponse>(Options, cancellationToken)
                .ConfigureAwait(ConfigureAwaitOptions.None) ?? throw new ServiceException($"Access to external system Hubspot did not return a response");
            return response;
        }
        catch (JsonException je)
        {
            throw new ServiceException($"Access to external system Hubspot did not return a valid json response: {je.Message}");
        }
    }

    public async Task<IEnumerable<HubspotAgreementResponse>> GetHubspotAgreementsAsync(CancellationToken cancellationToken)
    {
        using var httpClient = await _tokenService.GetAuthorizedClient<HubspotService>(_settings, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);

        var result = await httpClient.GetAsync($"api/get-agreements", cancellationToken)
            .CatchingIntoServiceExceptionFor("hubspot-get-agreements", HttpAsyncResponseMessageExtension.RecoverOptions.INFRASTRUCTURE).ConfigureAwait(false);
        try
        {
            var response = await result.Content
                .ReadFromJsonAsync<IEnumerable<HubspotAgreementResponse>>(Options, cancellationToken)
                .ConfigureAwait(ConfigureAwaitOptions.None) ?? throw new ServiceException("Access to external system Hubspot did not return a response");
            return response;
        }
        catch (JsonException je)
        {
            throw new ServiceException($"Access to external system Hubspot did not return a valid json response: {je.Message}");
        }
    }

    public async Task<HubspotCompanyUpdateResponse> CreateUpdateHubspotCompanyAsync(HubspotOutboundRequest hubspotRequest, CancellationToken cancellationToken)
    {
        using var httpClient = await _tokenService.GetAuthorizedClient<HubspotService>(_settings, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);

        var request = new { company = hubspotRequest.Company, contact = hubspotRequest.Contact };
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var result = await httpClient.PostAsync($"api/create-update-in-hubspot", content, cancellationToken)
            .CatchingIntoServiceExceptionFor("hubspot-create-update-company-contact", HttpAsyncResponseMessageExtension.RecoverOptions.INFRASTRUCTURE).ConfigureAwait(false);
        try
        {
            // First read the raw response content as a string
            var rawResponse = await result.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);

            var response = await result.Content
                .ReadFromJsonAsync<HubspotCompanyUpdateResponse>(Options, cancellationToken)
                .ConfigureAwait(ConfigureAwaitOptions.None) ?? throw new ServiceException($"Access to external system Hubspot did not return a response");
            return response;
        }
        catch (JsonException je)
        {
            throw new ServiceException($"Access to external system Hubspot did not return a valid json response: {je.Message}");
        }
    }

    public async Task<IEnumerable<HubspotDealCreateResponse>> CreateDealAsync(HubspotDealRequest[] hubspotRequest, CancellationToken cancellationToken)
    {
        using var httpClient = await _tokenService.GetAuthorizedClient<HubspotService>(_settings, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);

        var content = new StringContent(JsonSerializer.Serialize(hubspotRequest), Encoding.UTF8, "application/json");
        var result = await httpClient.PostAsync($"api/create-deal", content, cancellationToken)
            .CatchingIntoServiceExceptionFor("hubspot-create-deal", HttpAsyncResponseMessageExtension.RecoverOptions.INFRASTRUCTURE).ConfigureAwait(false);
        try
        {
            var rawResponse = await result.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);

            var response = await result.Content
                .ReadFromJsonAsync<IEnumerable<HubspotDealCreateResponse>>(Options, cancellationToken)
                .ConfigureAwait(ConfigureAwaitOptions.None) ?? throw new ServiceException($"Access to external system Hubspot did not return a response");
            return response;
        }
        catch (JsonException je)
        {
            throw new ServiceException($"Access to external system Hubspot did not return a valid json response: {je.Message}");
        }
    }
}

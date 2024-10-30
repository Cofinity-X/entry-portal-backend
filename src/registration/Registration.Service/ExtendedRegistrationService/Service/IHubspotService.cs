using Org.Eclipse.TractusX.Portal.Backend.ExtendedRegistration.Service.Model;

namespace Org.Eclipse.TractusX.Portal.Backend.ExtendedRegistration.Service;

public interface IHubspotService
{
    Task<IEnumerable<HubspotProductResponse>> GetProductsByCompanyRoleAsync(string[] companyRoles, CancellationToken cancellationToken);
    Task<HubspotCompanyResponse> GetHubspotCompanyDetailsAsync(string portalCompanyId, CancellationToken cancellationToken);
    Task<IEnumerable<HubspotAgreementResponse>> GetHubspotAgreementsAsync(CancellationToken cancellationToken);
    Task<HubspotCompanyUpdateResponse> CreateUpdateHubspotCompanyAsync(HubspotOutboundRequest hubspotRequest, CancellationToken cancellationToken);
    Task<IEnumerable<HubspotDealCreateResponse>> CreateDealAsync(HubspotDealRequest[] hubspotRequest, CancellationToken cancellationToken);
    Task<IEnumerable<HubspotQuoteResponse>> GetQuotesAsync(string portalCompanyId, CancellationToken cancellationToken);
    Task<MemoryStream> GetQuotePdfAsync(string pdfUrl, CancellationToken cancellationToken);
}

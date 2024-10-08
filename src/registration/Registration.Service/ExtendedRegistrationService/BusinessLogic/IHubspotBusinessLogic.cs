using Org.Eclipse.TractusX.Portal.Backend.ExtendedRegistration.Service.Model;

namespace Org.Eclipse.TractusX.Portal.Backend.ExtendedRegistration.Service.BusinessLogic
{
    public interface IHubspotBusinessLogic
    {
        Task<IEnumerable<HubspotProductResponse>> GetProductsByCompanyRoleAsync(string[] companyRoles, CancellationToken cancellationToken);
        Task<HubspotCompanyResponse> GetHubspotCompanyDetailsAsync(CancellationToken cancellationToken);
        Task<IEnumerable<HubspotAgreementResponse>> GetHubspotAgreementsAsync(CancellationToken cancellationToken);
        Task<HubspotCompanyUpdateResponse> CreateUpdateHubspotCompanyAsync(HubspotInboundRequest hubspotRequest, CancellationToken cancellationToken);
        Task<HubspotDealCreateResponse> CreateDealAsync(HubspotDealRequest hubspotDealRequest, CancellationToken cancellationToken);
    }
}

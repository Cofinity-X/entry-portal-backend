using Org.Eclipse.TractusX.Portal.Backend.ExtendedRegistration.Service.Model;
using Org.Eclipse.TractusX.Portal.Backend.Framework.ErrorHandling;
using Org.Eclipse.TractusX.Portal.Backend.Framework.Identity;

namespace Org.Eclipse.TractusX.Portal.Backend.ExtendedRegistration.Service.BusinessLogic;

public class HubspotBusinessLogic(
    IHubspotService hubspotService, IIdentityService identityService) : IHubspotBusinessLogic
{
    public async Task<IEnumerable<HubspotProductResponse>> GetProductsByCompanyRoleAsync(string[] companyRoles, CancellationToken cancellationToken)
    {
        var response = await hubspotService.GetProductsByCompanyRoleAsync(companyRoles, cancellationToken)
          .ConfigureAwait(ConfigureAwaitOptions.None);
        if (!response.Any())
        {
            throw new NotFoundException($"Access to external system Hubspot did not found the products for these company roles : {string.Join(",", companyRoles)}");
        }
        return response;
    }

    public async Task<HubspotCompanyResponse> GetHubspotCompanyDetailsAsync(CancellationToken cancellationToken)
    {
        return await hubspotService.GetHubspotCompanyDetailsAsync(identityService.IdentityData.CompanyId.ToString(), cancellationToken)
          .ConfigureAwait(ConfigureAwaitOptions.None);
    }

    public async Task<IEnumerable<HubspotAgreementResponse>> GetHubspotAgreementsAsync(CancellationToken cancellationToken)
    {
        return await hubspotService.GetHubspotAgreementsAsync(cancellationToken)
          .ConfigureAwait(ConfigureAwaitOptions.None);
    }

    public async Task<HubspotCompanyUpdateResponse> CreateUpdateHubspotCompanyAsync(HubspotInboundRequest hubspotRequest, CancellationToken cancellationToken)
    {
        var hubspotOutboundRequest = new HubspotOutboundRequest
        (
           hubspotRequest.Company != null ? new HubspotOutboundCompany
           {

               PortalCompanyId = identityService.IdentityData.CompanyId.ToString(),
               Agreements = hubspotRequest.Company.Agreements,
               Properties = hubspotRequest.Company.Properties

           } : null, hubspotRequest.Contact
        );
        var response = await hubspotService.CreateUpdateHubspotCompanyAsync(hubspotOutboundRequest, cancellationToken)
          .ConfigureAwait(ConfigureAwaitOptions.None);
        return response.CompanyId != null || response.ContactId != null ? response : throw new ServiceException("Access to external system Hubspot did not updated the company or contact");
    }

    public async Task<IEnumerable<HubspotDealCreateResponse>> CreateDealAsync(HubspotDealRequest[] hubspotDealRequest, CancellationToken cancellationToken)
    {
        var hsCompany = await hubspotService.GetHubspotCompanyDetailsAsync(identityService.IdentityData.CompanyId.ToString(), cancellationToken);
        if (hubspotDealRequest.Any(item => hsCompany.Id != item.CompanyId))
        {
            throw new ConflictException("This user is not associated with the Hubspot company sent in the request.");
        }
        return await hubspotService.CreateDealAsync(hubspotDealRequest, cancellationToken)
         .ConfigureAwait(ConfigureAwaitOptions.None);
    }
}


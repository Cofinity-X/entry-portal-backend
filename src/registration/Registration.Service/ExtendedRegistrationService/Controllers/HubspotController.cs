using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.Eclipse.TractusX.Portal.Backend.ExtendedRegistration.Service.BusinessLogic;
using Org.Eclipse.TractusX.Portal.Backend.ExtendedRegistration.Service.Model;
using Org.Eclipse.TractusX.Portal.Backend.Framework.ErrorHandling.Web;
using Org.Eclipse.TractusX.Portal.Backend.Framework.Web;
using Org.Eclipse.TractusX.Portal.Backend.Web.Identity;

namespace Org.Eclipse.TractusX.Portal.Backend.ExtendedRegistration.Service.Controllers
{
    [ApiController]
    [EnvironmentRoute("MVC_ROUTING_BASEPATH", "[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class HubspotController(IHubspotBusinessLogic hubspotBusinessLogic) : ControllerBase
    {

        /// <summary>
        /// Gets hubspot product by company roles
        /// </summary>
        /// <param name="companyRoles" example="App Provider">Role of the company to get the product</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Returns a List of products from Hubspot</returns>
        /// <remarks>Example: POST: /api/registration/Hubspot/productsByCompanyRole</remarks>
        /// <response code="200">Returns the products from hubspot</response>
        /// <response code="400"></response>
        /// <response code="503">The requested service responded with the given error.</response>
        [HttpPost]
        [Authorize(Roles = "view_registration")]
        [Route("productsByCompanyRole")]
        [ProducesResponseType(typeof(IEnumerable<HubspotProductResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        public Task<IEnumerable<HubspotProductResponse>> GetProductsByCompanyRoleAsync([FromBody] string[] companyRoles, CancellationToken cancellationToken) =>
            hubspotBusinessLogic.GetProductsByCompanyRoleAsync(companyRoles, cancellationToken);

        /// <summary>
        /// Gets hubspot company details for logged in user
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Returns the  hubspot company details</returns>
        /// <remarks>Example: GET: /api/registration/Hubspot/companyDetails</remarks>
        /// <response code="200">Returns the hubspot company</response>
        /// <response code="400"></response>
        /// <response code="404"></response>
        /// <response code="503">The requested service responded with the given error.</response>
        [HttpGet]
        [Authorize(Roles = "view_registration")]
        [Route("companyDetails")]
        [Authorize(Policy = PolicyTypes.ValidCompany)]
        [ProducesResponseType(typeof(HubspotCompanyResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        public Task<HubspotCompanyResponse> GetHubspotCompanyDetailsAsync(CancellationToken cancellationToken) =>
            hubspotBusinessLogic.GetHubspotCompanyDetailsAsync(cancellationToken);

        /// <summary>
        /// Gets hubspot agreements
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Returns a List of all agreements from Hubspot</returns>
        /// <remarks>Example: GET: /api/registration/Hubspot/agreements</remarks>
        /// <response code="200">Returns the agreements from hubspot</response>
        /// <response code="400"></response>
        /// <response code="404"></response>
        /// <response code="503">The requested service responded with the given error.</response>
        [HttpGet]
        [Authorize(Roles = "view_registration")]
        [Route("agreements")]
        [ProducesResponseType(typeof(IEnumerable<HubspotAgreementResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        public Task<IEnumerable<HubspotAgreementResponse>> GetHubspotAgreementsAsync(CancellationToken cancellationToken) =>
            hubspotBusinessLogic.GetHubspotAgreementsAsync(cancellationToken);

        /// <summary>
        /// Create or Update hubspot company or contact or both
        /// </summary>
        /// <param name="hubspotRequest">hubspotRequest</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Returns the contact/company id of successfully updated object </returns>
        /// <remarks>Example: POST: /api/registration/Hubspot/createUpdateHubspotCompany</remarks>
        /// <response code="200">Returns the hubspot company and contact id from hubspot</response>
        /// <response code="400"></response>
        /// <response code="503">The requested service responded with the given error.</response>
        [HttpPost]
        [Authorize(Roles = "submit_registration")]
        [Route("createUpdateHubspotCompany")]
        [Authorize(Policy = PolicyTypes.ValidCompany)]
        [ProducesResponseType(typeof(HubspotCompanyUpdateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        public Task<HubspotCompanyUpdateResponse> CreateUpdateHubspotCompanyAsync([FromBody] HubspotInboundRequest hubspotRequest, CancellationToken cancellationToken) =>
            hubspotBusinessLogic.CreateUpdateHubspotCompanyAsync(hubspotRequest, cancellationToken);

        /// <summary>
        /// Create deal in hubspot
        /// </summary>
        /// <param name="hubspotRequest">hubspotRequest</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Returns the id of deal created </returns>
        /// <remarks>Example: POST: /api/registration/Hubspot/createDeal</remarks>
        /// <response code="200">Returns the dealId, quoteId and quoteUrl from hubspot</response>
        /// <response code="400"></response>
        /// <response code="503">The requested service responded with the given error.</response>
        [HttpPost]
        [Authorize(Roles = "submit_registration")]
        [Route("createDeal")]
        [Authorize(Policy = PolicyTypes.ValidCompany)]
        [ProducesResponseType(typeof(IEnumerable<HubspotDealCreateResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        public Task<IEnumerable<HubspotDealCreateResponse>> CreateDealAsync([FromBody] HubspotDealRequest[] hubspotRequest, CancellationToken cancellationToken) =>
            hubspotBusinessLogic.CreateDealAsync(hubspotRequest, cancellationToken);

        /// <summary>
        /// Get Deal quote combined pdf from Hubspot for logged in company
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Returns the pdf of combined quote </returns>
        /// <remarks>Example: POST: /api/registration/Hubspot/quote</remarks>
        /// <response code="200">Returns the combined pdf of quote from hubspot</response>
        /// <response code="404">No quotes found for the logged in company in hubspot</response>
        /// <response code="503">The requested service responded with the given error.</response>
        [HttpPost]
        [Authorize(Roles = "view_registration")]
        [Route("quote")]
        [Authorize(Policy = PolicyTypes.ValidCompany)]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult> GetQuotesAsync(CancellationToken cancellationToken)
        {
            var pdfData = await hubspotBusinessLogic.GetQuoteCombinedPdfAsync(cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);
            return File(pdfData.Content, pdfData.MediaType, pdfData.FileName);
        }
    }
}

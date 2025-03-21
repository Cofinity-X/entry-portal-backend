/********************************************************************************
 * Copyright (c) 2022 BMW Group AG
 * Copyright (c) 2022 Contributors to the Eclipse Foundation
 *
 * See the NOTICE file(s) distributed with this work for additional
 * information regarding copyright ownership.
 *
 * This program and the accompanying materials are made available under the
 * terms of the Apache License, Version 2.0 which is available at
 * https://www.apache.org/licenses/LICENSE-2.0.
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 *
 * SPDX-License-Identifier: Apache-2.0
 ********************************************************************************/

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.Eclipse.TractusX.Portal.Backend.Apps.Service.BusinessLogic;
using Org.Eclipse.TractusX.Portal.Backend.Apps.Service.ViewModels;
using Org.Eclipse.TractusX.Portal.Backend.Framework.ErrorHandling.Web;
using Org.Eclipse.TractusX.Portal.Backend.Framework.Models;
using Org.Eclipse.TractusX.Portal.Backend.Framework.Web;
using Org.Eclipse.TractusX.Portal.Backend.Offers.Library.Models;
using Org.Eclipse.TractusX.Portal.Backend.PortalBackend.DBAccess.Models;
using Org.Eclipse.TractusX.Portal.Backend.PortalBackend.PortalEntities.Enums;
using Org.Eclipse.TractusX.Portal.Backend.Web.Identity;

namespace Org.Eclipse.TractusX.Portal.Backend.Apps.Service.Controllers;

/// <summary>
/// Controller providing actions for updating applications.
/// </summary>
[EnvironmentRoute("MVC_ROUTING_BASEPATH", "[controller]")]
[ApiController]
[Produces("application/json")]
[Consumes("application/json")]
public class AppReleaseProcessController : ControllerBase
{
    private readonly IAppReleaseBusinessLogic _appReleaseBusinessLogic;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="appReleaseBusinessLogic"></param>
    public AppReleaseProcessController(IAppReleaseBusinessLogic appReleaseBusinessLogic)
    {
        _appReleaseBusinessLogic = appReleaseBusinessLogic;
    }

    /// <summary>
    /// Upload document for apps in status CREATED and document in status PENDING in the marketplace for given appId for same company as user
    /// </summary>
    /// <param name="appId"></param>
    /// <param name="documentTypeId"></param>
    /// <param name="document"></param>
    /// <param name="cancellationToken"></param>
    /// <remarks>Example: PUT: /api/apps/appreleaseprocess/updateappdoc/{appId}/documentType/{documentTypeId}/documents</remarks>
    /// <response code="204">Successfully uploaded the document</response>
    /// <response code="400">If sub claim is empty/invalid or user does not exist, or any other parameters are invalid.</response>
    /// <response code="404">App does not exist.</response>
    /// <response code="403">The user is not assigned with the app.</response>
    /// <response code="409">Offer is in incorrect state.</response>
    /// <response code="415">Only PDF files are supported.</response>
    [HttpPut]
    [Route("updateappdoc/{appId}/documentType/{documentTypeId}/documents")]
    [Authorize(Roles = "app_management")]
    [Authorize(Policy = PolicyTypes.ValidCompany)]
    [Consumes("multipart/form-data")]
    [RequestFormLimits(ValueLengthLimit = 819200, MultipartBodyLengthLimit = 819200)]
    [ProducesResponseType(typeof(NoContentResult), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status415UnsupportedMediaType)]
    public async Task<NoContentResult> UpdateAppDocumentAsync([FromRoute] Guid appId, [FromRoute] DocumentTypeId documentTypeId, IFormFile document, CancellationToken cancellationToken)
    {
        await _appReleaseBusinessLogic.CreateAppDocumentAsync(appId, documentTypeId, document, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Add role and role description for App 
    /// </summary>
    /// <param name="appId"></param>
    /// <param name="userRoles"></param>
    /// <remarks>Example: POST: /api/apps/appreleaseprocess/{appId}/role</remarks>
    /// <response code="400">If sub claim is empty/invalid or user does not exist, or any other parameters are invalid.</response>
    /// <response code="404">App does not exist.</response>
    /// <response code="200">created role and role description successfully.</response>
    /// <response code="403">User not associated with provider company</response>
    [HttpPost]
    [Route("{appId}/role")]
    [Authorize(Roles = "edit_apps")]
    [Authorize(Policy = PolicyTypes.ValidCompany)]
    [ProducesResponseType(typeof(IEnumerable<AppRoleData>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public Task<IEnumerable<AppRoleData>> AddAppUserRole([FromRoute] Guid appId, [FromBody] IEnumerable<AppUserRole> userRoles) =>
         _appReleaseBusinessLogic.AddAppUserRoleAsync(appId, userRoles);

    /// <summary>
    /// Return Agreement Data for offer_type_id App
    /// </summary>
    /// <param name="languageShortName"></param>
    /// <remarks>Example: GET: /api/apps/appreleaseprocess/agreementData</remarks>
    /// <response code="200">Returns the Cpllection of agreement data</response>
    [HttpGet]
    [Route("agreementData")]
    [Authorize(Roles = "edit_apps")]
    [ProducesResponseType(typeof(IAsyncEnumerable<AgreementDocumentData>), StatusCodes.Status200OK)]
    public IAsyncEnumerable<AgreementDocumentData> GetOfferAgreementDataAsync([FromQuery] string languageShortName = Constants.DefaultLanguage) =>
        _appReleaseBusinessLogic.GetOfferAgreementDataAsync(languageShortName);

    /// <summary>
    /// Gets the agreement consent status for the given app id
    /// </summary>
    /// <param name="appId"></param>
    /// <remarks>Example: GET: /api/apps/appreleaseprocess/consent/{appId}</remarks>
    /// <response code="200">Returns the Offer Agreement Consent data</response>
    /// <response code="404">App does not exist.</response>
    /// <response code="403">User not associated with offer.</response>
    [HttpGet]
    [Route("consent/{appId}")]
    [Authorize(Roles = "edit_apps")]
    [Authorize(Policy = PolicyTypes.ValidCompany)]
    [ProducesResponseType(typeof(OfferAgreementConsent), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public Task<OfferAgreementConsent> GetOfferAgreementConsentById([FromRoute] Guid appId) =>
        _appReleaseBusinessLogic.GetOfferAgreementConsentById(appId);

    /// <summary>
    /// Update or Insert Consent
    /// </summary>
    /// <param name="appId"></param>
    /// <param name="offerAgreementConsents"></param>
    /// <remarks>Example: POST: /api/apps/appreleaseprocess/consent/{appId}/agreementConsents</remarks>
    /// <response code="200">Successfully submitted consent to agreements</response>
    /// <response code="403">Either the user was not found or the user is not assignable to the given application.</response>
    /// <response code="404">App does not exist.</response>
    /// <response code="400">App Id is incorrect.</response>
    [HttpPost]
    [Authorize(Roles = "edit_apps")]
    [Authorize(Policy = PolicyTypes.CompanyUser)]
    [Authorize(Policy = PolicyTypes.ValidCompany)]
    [Route("consent/{appId}/agreementConsents")]
    [ProducesResponseType(typeof(IEnumerable<ConsentStatusData>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public Task<IEnumerable<ConsentStatusData>> SubmitOfferConsentToAgreementsAsync([FromRoute] Guid appId, [FromBody] OfferAgreementConsent offerAgreementConsents) =>
        _appReleaseBusinessLogic.SubmitOfferConsentAsync(appId, offerAgreementConsents);

    /// <summary>
    /// Return app detail with status
    /// </summary>
    /// <param name="appId"></param>
    /// <param name="languageShortName"></param>
    /// <remarks>Example: GET: /api/apps/appreleaseprocess/{appId}/appStatus</remarks>
    /// <response code="200">Return the Offer and status data</response>
    /// <response code="404">App does not exist.</response>
    /// <response code="403">User not associated with provider company.</response>
    [HttpGet]
    [Route("{appId}/appStatus")]
    [Authorize(Roles = "app_management")]
    [Authorize(Policy = PolicyTypes.ValidCompany)]
    [ProducesResponseType(typeof(AppProviderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public Task<AppProviderResponse> GetAppDetailsForStatusAsync([FromRoute] Guid appId, [FromQuery] string languageShortName = Constants.DefaultLanguage) =>
        _appReleaseBusinessLogic.GetAppDetailsForStatusAsync(appId, languageShortName);

    /// <summary>
    /// Removes a role from persistence layer by appId and roleId.
    /// </summary>
    /// <param name="appId" example="5636F9B9-C3DE-4BA5-8027-00D17A2FECFB">ID of the app to be deleted.</param>
    /// <param name="roleId" example="5636F9B9-C3DE-4BA5-8027-00D17A2FECFB">ID of the role to be deleted.</param>
    /// <remarks>Example: DELETE: /api/apps/appreleaseprocess/{appId}/role/{roleId}</remarks>
    /// <response code="204">Empty response on success.</response>
    /// <response code="404">Record not found.</response>
    /// <response code="400">Input is incorrect</response>
    /// <response code="403">User is not associated with provider company</response>
    [HttpDelete]
    [Route("{appId}/role/{roleId}")]
    [Authorize(Roles = "edit_apps")]
    [Authorize(Policy = PolicyTypes.ValidCompany)]
    [ProducesResponseType(typeof(IActionResult), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<NoContentResult> DeleteAppRoleAsync([FromRoute] Guid appId, [FromRoute] Guid roleId)
    {
        await _appReleaseBusinessLogic.DeleteAppRoleAsync(appId, roleId);
        return NoContent();
    }

    /// <summary>
    /// Get All Users with Role of Sales Manager
    /// </summary>
    /// <remarks>Example: GET: /api/apps/appreleaseprocess/ownCompany/salesManager</remarks>
    /// <response code="200">Return the Users with Role of Sales Manager.</response>
    [HttpGet]
    [Route("ownCompany/salesManager")]
    [Authorize(Roles = "add_apps")]
    [Authorize(Policy = PolicyTypes.ValidCompany)]
    [ProducesResponseType(typeof(IAsyncEnumerable<CompanyUserNameData>), StatusCodes.Status200OK)]
    public IAsyncEnumerable<CompanyUserNameData> GetAppProviderSalesManagerAsync() =>
        _appReleaseBusinessLogic.GetAppProviderSalesManagersAsync();

    /// <summary>
    /// Creates an app according to request model
    /// </summary>
    /// <param name="appRequestModel">Request model for app creation.</param>
    /// <returns>ID of created application.</returns> 
    /// <remarks>Example: POST: /api/apps/appreleaseprocess/createapp</remarks>
    /// <response code="201">Returns created app's ID.</response>
    /// <response code="400">Language Code or Use Case or CompanyId is incorrect</response>
    /// <response code="403">User is not associated with provider company.</response>
    [HttpPost]
    [Route("createapp")]
    [Authorize(Roles = "add_apps")]
    [Authorize(Policy = PolicyTypes.ValidCompany)]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<CreatedAtRouteResult> ExecuteAppCreation([FromBody] AppRequestModel appRequestModel)
    {
        var appId = await _appReleaseBusinessLogic.AddAppAsync(appRequestModel).ConfigureAwait(ConfigureAwaitOptions.None);
        return CreatedAtRoute(nameof(AppsController.GetAppDetailsByIdAsync), new { controller = "Apps", appId = appId }, appId);
    }

    /// <summary>
    /// Updates an app according to request model
    /// </summary>
    /// <param name="appId" example="15507472-dfdc-4885-b165-8d4a8970a3e2">Id of the app to update</param>
    /// <param name="appRequestModel">Request model for app creation.</param>
    /// <returns>ID of updated application.</returns> 
    /// <remarks>Example: PUT: /api/apps/appreleaseprocess/15507472-dfdc-4885-b165-8d4a8970a3e2</remarks>
    /// <response code="201">Returns created app's ID.</response>
    /// <response code="404">App does not exist </response>
    /// <response code="400">Language Code or Use Case or CompanyId is incorrect</response>
    /// <response code="403">User don't have permission to change the app.</response>
    /// <response code="409">Offer is in inCorrect State.</response>
    [HttpPut]
    [Route("{appId}")]
    [Authorize(Roles = "edit_apps")]
    [Authorize(Policy = PolicyTypes.ValidCompany)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<NoContentResult> UpdateAppRelease([FromRoute] Guid appId, [FromBody] AppRequestModel appRequestModel)
    {
        await _appReleaseBusinessLogic.UpdateAppReleaseAsync(appId, appRequestModel).ConfigureAwait(ConfigureAwaitOptions.None);
        return NoContent();
    }

    /// <summary>
    /// Retrieves all in review status apps in the marketplace .
    /// </summary>
    /// <param name="page">page index start from 0</param>
    /// <param name="size">size to get number of records</param>
    /// <param name="sorting">sort by</param>
    /// <param name="offerStatusIdFilter">Filter by offerStatusId</param>
    /// <returns>Collection of all in review status marketplace apps.</returns>
    /// <remarks>Example: GET: /api/apps/appreleaseprocess/inReview</remarks>
    /// <response code="200">Returns the list of all in review status marketplace apps.</response>
    [HttpGet]
    [Route("inReview")]
    [Authorize(Policy = PolicyTypes.CompanyUser)]
    [Authorize(Roles = "approve_app_release,decline_app_release")]
    [ProducesResponseType(typeof(Pagination.Response<InReviewAppData>), StatusCodes.Status200OK)]
    public Task<Pagination.Response<InReviewAppData>> GetAllInReviewStatusAppsAsync([FromQuery] int page = 0, [FromQuery] int size = 15, [FromQuery] OfferSorting? sorting = null, OfferStatusIdFilter? offerStatusIdFilter = null) =>
        _appReleaseBusinessLogic.GetAllInReviewStatusAppsAsync(page, size, sorting, offerStatusIdFilter);

    /// <summary>
    /// Submit an app for release
    /// </summary>
    /// <param name="appId" example="D3B1ECA2-6148-4008-9E6C-C1C2AEA5C645">ID of the app.</param>
    /// <remarks>Example: PUT: /api/apps/appreleaseprocess/D3B1ECA2-6148-4008-9E6C-C1C2AEA5C645/submit</remarks>
    /// <response code="204">The app was successfully submitted for release.</response>
    /// <response code="400">Either the sub claim is empty/invalid, user does not exist or the subscription might not have the correct status or the companyID is incorrect.</response>
    /// <response code="404">App does not exist.</response>
    /// <response code="409">User not associated with company</response>
    [HttpPut]
    [Route("{appId}/submit")]
    [Authorize(Roles = "add_apps")]
    [Authorize(Policy = PolicyTypes.ValidIdentity)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<NoContentResult> SubmitAppReleaseRequest([FromRoute] Guid appId)
    {
        await _appReleaseBusinessLogic.SubmitAppReleaseRequestAsync(appId).ConfigureAwait(ConfigureAwaitOptions.None);
        return NoContent();
    }

    /// <summary>
    /// Approve App to change status from IN_REVIEW to Active and create notification
    /// </summary>
    /// <param name="appId"></param>
    /// <remarks>Example: PUT: /api/apps/appreleaseprocess/D3B1ECA2-6148-4008-9E6C-C1C2AEA5C645/approveApp</remarks>
    /// <response code="204">The app was successfully submitted to Active State.</response>
    /// <response code="409">App is in InCorrect Status</response>
    /// <response code="500">Internal Server Error.</response>
    /// <response code="404">App does not exist.</response>
    [HttpPut]
    [Route("{appId}/approveApp")]
    [Authorize(Roles = "approve_app_release")]
    [Authorize(Policy = PolicyTypes.CompanyUser)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<NoContentResult> ApproveAppRequest([FromRoute] Guid appId)
    {
        await _appReleaseBusinessLogic.ApproveAppRequestAsync(appId).ConfigureAwait(ConfigureAwaitOptions.None);
        return NoContent();
    }

    /// <summary>
    /// Retrieve Privacy Policies
    /// </summary>
    /// <returns>Collection of all Privacy Policy.</returns>
    /// <remarks>Example: GET: /api/apps/appreleaseprocess/privacyPolicies</remarks>
    /// <response code="200">Return the privacy policies</response>
    [HttpGet]
    [Route("privacyPolicies")]
    [Authorize(Roles = "add_apps")]
    [ProducesResponseType(typeof(PrivacyPolicyData), StatusCodes.Status200OK)]
    public PrivacyPolicyData GetPrivacyPolicyDataAsync() =>
        _appReleaseBusinessLogic.GetPrivacyPolicyDataAsync();

    /// <summary>
    /// Declines the app request
    /// </summary>
    /// <param name="appId" example="D3B1ECA2-6148-4008-9E6C-C1C2AEA5C645">Id of the app that should be declined</param>
    /// <param name="data">the data of the decline request</param>
    /// <remarks>Example: PUT: /api/apps/appreleaseprocess/D3B1ECA2-6148-4008-9E6C-C1C2AEA5C645/declineApp</remarks>
    /// <response code="204">NoContent.</response>
    /// <response code="400">If sub claim is empty/invalid or user does not exist.</response>
    /// <response code="404">If app does not exists.</response>
    /// <response code="403">User does not have permission to change the app</response>
    /// <response code="409">Offer is in incorrect state</response>
    /// <response code="500">Internal Server Error</response>
    [HttpPut]
    [Route("{appId:guid}/declineApp")]
    [Authorize(Roles = "decline_app_release")]
    [Authorize(Policy = PolicyTypes.CompanyUser)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<NoContentResult> DeclineAppRequest([FromRoute] Guid appId, [FromBody] OfferDeclineRequest data)
    {
        await _appReleaseBusinessLogic.DeclineAppRequestAsync(appId, data).ConfigureAwait(ConfigureAwaitOptions.None);
        return NoContent();
    }

    /// <summary>
    /// Gets InReview app details for an app referenced by id.
    /// </summary>
    /// <param name="appId" example="D3B1ECA2-6148-4008-9E6C-C1C2AEA5C645">ID of the app to retrieve.</param>
    /// <returns>InReviewAppDetails ViewNodel for requested application.</returns>
    /// <remarks>Example: GET: /api/apps/appreleaseprocess/inReview/D3B1ECA2-6148-4008-9E6C-C1C2AEA5C645</remarks>
    /// <response code="200">Returns the requested app details.</response>
    /// <response code="400">If sub claim is empty/invalid.</response>
    /// <response code="404">App not found.</response>
    [HttpGet]
    [Route("inReview/{appId}")]
    [Authorize(Roles = "approve_app_release,decline_app_release")]
    [ProducesResponseType(typeof(InReviewAppDetails), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<InReviewAppDetails> GetInReviewAppDetailsByIdAsync([FromRoute] Guid appId) =>
        await _appReleaseBusinessLogic.GetInReviewAppDetailsByIdAsync(appId);

    /// <summary>
    /// Delete Document Assigned to Offer
    /// </summary>
    /// <param name="documentId">ID of the document to be deleted.</param>
    /// <remarks>Example: DELETE: /api/apps/appreleaseprocess/documents/{documentId}</remarks>
    /// <response code="204">Empty response on success.</response>
    /// <response code="404">Record not found.</response>
    /// <response code="409">Document or App is in InCorrect Status</response>
    /// <response code="403">User is not allowed to delete the document</response>
    /// <response code="400"> parameters are invalid.</response>
    [HttpDelete]
    [Route("documents/{documentId}")]
    [Authorize(Roles = "edit_apps")]
    [Authorize(Policy = PolicyTypes.ValidCompany)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<NoContentResult> DeleteAppDocumentsAsync([FromRoute] Guid documentId)
    {
        await _appReleaseBusinessLogic.DeleteAppDocumentsAsync(documentId);
        return NoContent();
    }

    /// <summary>
    /// Delete App by Id
    /// </summary>
    /// <param name="appId" example="5636F9B9-C3DE-4BA5-8027-00D17A2FECFB">ID of the app to be deleted.</param>
    /// <remarks>Example: DELETE: /api/apps/appreleaseprocess/{appId}</remarks>
    /// <response code="204">Empty response on success.</response>
    /// <response code="404">Record not found.</response>
    /// <response code="400">Input is incorrect.</response>
    /// <response code="403">User is not associated with provider company.</response>
    [HttpDelete]
    [Route("{appId}")]
    [Authorize(Roles = "edit_apps")]
    [Authorize(Policy = PolicyTypes.ValidCompany)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<NoContentResult> DeleteAppAsync([FromRoute] Guid appId)
    {
        await _appReleaseBusinessLogic.DeleteAppAsync(appId).ConfigureAwait(ConfigureAwaitOptions.None);
        return NoContent();
    }

    /// <summary>
    /// Sets the instance type and all related data for the app
    /// </summary>
    /// <param name="appId" example="5636F9B9-C3DE-4BA5-8027-00D17A2FECFB">ID of the app to be deleted.</param>
    /// <param name="data">Data needed for the instance setup</param>
    /// <remarks>Example: PUT: /api/apps/appreleaseprocess/instance-type/{appId}</remarks>
    /// <response code="204">Empty response on success.</response>
    /// <response code="400">Input is incorrect.</response>
    /// <response code="403">User is not associated with provider company.</response>
    /// <response code="404">Record not found.</response>
    [HttpPost]
    [Route("instance-type/{appId}")]
    [Authorize(Roles = "edit_apps")]
    [Authorize(Policy = PolicyTypes.ValidCompany)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<NoContentResult> SetInstanceType([FromRoute] Guid appId, [FromBody] AppInstanceSetupData data)
    {
        await _appReleaseBusinessLogic.SetInstanceType(appId, data).ConfigureAwait(ConfigureAwaitOptions.None);
        return NoContent();
    }

    /// <summary>
    /// Retrieve app specific stored technical user profile configured to get created if a related app subscription is getting activated.
    /// </summary>
    /// <param name="appId">id of the app to receive the technical user profiles for</param>
    /// <remarks>Example: GET: /api/apps/{appId}/appreleaseprocess/technical-user-profiles</remarks>
    /// <response code="200">Returns a list of profiles</response>
    /// <response code="403">Requesting user is not part of the providing company for the service.</response>
    [HttpGet]
    [Route("{appId}/technical-user-profiles")]
    [Authorize(Roles = "add_apps")]
    [Authorize(Policy = PolicyTypes.ValidCompany)]
    [ProducesResponseType(typeof(IEnumerable<TechnicalUserProfileInformation>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public Task<IEnumerable<TechnicalUserProfileInformation>> GetTechnicalUserProfiles([FromRoute] Guid appId) =>
        _appReleaseBusinessLogic.GetTechnicalUserProfilesForOffer(appId);

    /// <summary>
    /// Creates and updates the technical user profiles
    /// </summary>
    /// <param name="appId">id of the app to receive the technical user profiles for</param>
    /// <param name="data">The data for the update of the technical user profile</param>
    /// <remarks>Example: PUT: /api/apps/appreleaseprocess/{appId}/technical-user-profiles</remarks>
    /// <response code="200">Returns a list of profiles</response>
    /// <response code="403">Requesting user is not part of the providing company for the service.</response>
    [HttpPut]
    [Route("{appId}/technical-user-profiles")]
    [Authorize(Roles = "add_apps")]
    [Authorize(Policy = PolicyTypes.ValidCompany)]
    [ProducesResponseType(typeof(NoContentResult), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<NoContentResult> CreateAndUpdateTechnicalUserProfiles([FromRoute] Guid appId, [FromBody] IEnumerable<TechnicalUserProfileData> data)
    {
        await _appReleaseBusinessLogic.UpdateTechnicalUserProfiles(appId, data).ConfigureAwait(ConfigureAwaitOptions.None);
        return NoContent();
    }

    /// <summary>
    /// Gets the app providers an overview of configures app roles.
    /// </summary>
    /// <param name="appId" example="D3B1ECA2-6148-4008-9E6C-C1C2AEA5C645">Id of the app which roles should be returned.</param>
    /// <param name="languageShortName">OPTIONAL: The language short name.</param>
    /// <returns>Returns the app providers an overview of configures app roles.</returns>
    /// <remarks>Example: GET: /api/apps/AppChange/D3B1ECA2-6148-4008-9E6C-C1C2AEA5C645/roles</remarks>
    /// <response code="200">Returns the client roles.</response>
    /// <response code="404">The app was not found.</response>
    /// <response code="403">The app is not the provider of the company</response>
    [HttpGet]
    [Authorize(Roles = "add_apps")]
    [Authorize(Policy = PolicyTypes.ValidCompany)]
    [Route("{appId}/roles")]
    [ProducesResponseType(typeof(IEnumerable<ActiveAppRoleDetails>), StatusCodes.Status200OK)]
    public Task<IEnumerable<ActiveAppRoleDetails>> GetAppProviderRolesAsync([FromRoute] Guid appId, [FromQuery] string? languageShortName = null) =>
        _appReleaseBusinessLogic.GetAppProviderRolesAsync(appId, languageShortName);
}

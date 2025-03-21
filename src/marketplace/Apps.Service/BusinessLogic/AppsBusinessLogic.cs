/********************************************************************************
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

using Microsoft.Extensions.Options;
using Org.Eclipse.TractusX.Portal.Backend.Apps.Service.ErrorHandling;
using Org.Eclipse.TractusX.Portal.Backend.Apps.Service.ViewModels;
using Org.Eclipse.TractusX.Portal.Backend.Framework.ErrorHandling;
using Org.Eclipse.TractusX.Portal.Backend.Framework.Identity;
using Org.Eclipse.TractusX.Portal.Backend.Framework.Models;
using Org.Eclipse.TractusX.Portal.Backend.Offers.Library.Extensions;
using Org.Eclipse.TractusX.Portal.Backend.Offers.Library.Models;
using Org.Eclipse.TractusX.Portal.Backend.Offers.Library.Service;
using Org.Eclipse.TractusX.Portal.Backend.PortalBackend.DBAccess;
using Org.Eclipse.TractusX.Portal.Backend.PortalBackend.DBAccess.Models;
using Org.Eclipse.TractusX.Portal.Backend.PortalBackend.DBAccess.Repositories;
using Org.Eclipse.TractusX.Portal.Backend.PortalBackend.PortalEntities.Entities;
using Org.Eclipse.TractusX.Portal.Backend.PortalBackend.PortalEntities.Enums;

namespace Org.Eclipse.TractusX.Portal.Backend.Apps.Service.BusinessLogic;

/// <summary>
/// Implementation of <see cref="IAppsBusinessLogic"/>.
/// </summary>
public class AppsBusinessLogic : IAppsBusinessLogic
{
    private readonly IPortalRepositories _portalRepositories;
    private readonly IOfferSubscriptionService _offerSubscriptionService;
    private readonly AppsSettings _settings;
    private readonly IOfferService _offerService;
    private readonly IOfferSetupService _offerSetupService;
    private readonly IIdentityData _identityData;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="portalRepositories">Factory to access the repositories</param>
    /// <param name="offerSubscriptionService">OfferSubscription Service.</param>
    /// <param name="offerService">Offer service</param>
    /// <param name="offerSetupService">Offer Setup Service</param>
    /// <param name="settings">Settings</param>
    /// <param name="identityService">Identity </param>
    public AppsBusinessLogic(
        IPortalRepositories portalRepositories,
        IOfferSubscriptionService offerSubscriptionService,
        IOfferService offerService,
        IOfferSetupService offerSetupService,
        IOptions<AppsSettings> settings,
        IIdentityService identityService)
    {
        _portalRepositories = portalRepositories;
        _offerSubscriptionService = offerSubscriptionService;
        _offerService = offerService;
        _offerSetupService = offerSetupService;
        _identityData = identityService.IdentityData;
        _settings = settings.Value;
    }

    /// <inheritdoc/>
    public IAsyncEnumerable<AppData> GetAllActiveAppsAsync(string? languageShortName) =>
        _portalRepositories.GetInstance<IOfferRepository>().GetAllActiveAppsAsync(languageShortName, Constants.DefaultLanguage)
            .Select(app => new AppData(
                    app.Id,
                    app.Name ?? Constants.ErrorString,
                    app.ShortDescription ?? Constants.ErrorString,
                    app.VendorCompanyName,
                    app.LicenseType,
                    app.LicenseText ?? Constants.ErrorString,
                    app.LeadPictureId,
                    app.UseCaseNames));

    /// <inheritdoc/>
    public IAsyncEnumerable<BusinessAppData> GetAllUserUserBusinessAppsAsync() =>
        _portalRepositories.GetInstance<IOfferSubscriptionsRepository>()
            .GetAllBusinessAppDataForUserIdAsync(_identityData.IdentityId)
            .Select(x =>
                new BusinessAppData(
                    x.OfferId,
                    x.SubscriptionId,
                    x.OfferName ?? Constants.ErrorString,
                    x.SubscriptionUrl,
                    x.LeadPictureId,
                    x.Provider));

    /// <inheritdoc/>
    public async Task<AppDetailResponse> GetAppDetailsByIdAsync(Guid appId, string? languageShortName = null)
    {
        var result = await _portalRepositories.GetInstance<IOfferRepository>()
            .GetOfferDetailsByIdAsync(appId, _identityData.CompanyId, languageShortName, Constants.DefaultLanguage, OfferTypeId.APP).ConfigureAwait(ConfigureAwaitOptions.None);
        if (result == null)
        {
            throw NotFoundException.Create(AppErrors.APP_NOT_EXIST, new ErrorParameter[] { new("appId", appId.ToString()) });
        }

        return new AppDetailResponse(
            result.Id,
            result.Title ?? Constants.ErrorString,
            result.LeadPictureId,
            result.Images,
            result.ProviderUri ?? Constants.ErrorString,
            result.Provider,
            result.ContactEmail,
            result.ContactNumber,
            result.UseCases,
            result.LongDescription ?? Constants.ErrorString,
            result.LicenseTypeId,
            result.Price ?? Constants.ErrorString,
            result.Tags,
            result.IsSubscribed == default ? null : result.IsSubscribed,
            result.Languages,
            result.Documents.GroupBy(d => d.DocumentTypeId).ToDictionary(g => g.Key, g => g.Select(d => new DocumentData(d.DocumentId, d.DocumentName))),
            result.PrivacyPolicies,
            result.IsSingleInstance,
            result.TechnicalUserProfile.ToDictionary(g => g.TechnicalUserProfileId, g => g.UserRoles)
        );
    }

    /// <inheritdoc/>
    public IAsyncEnumerable<Guid> GetAllFavouriteAppsForUserAsync() =>
        _portalRepositories
            .GetInstance<IUserRepository>()
            .GetAllFavouriteAppsForUserUntrackedAsync(_identityData.IdentityId);

    /// <inheritdoc/>
    public async Task RemoveFavouriteAppForUserAsync(Guid appId)
    {
        _portalRepositories.Remove(new CompanyUserAssignedAppFavourite(appId, _identityData.IdentityId));
        await _portalRepositories.SaveAsync().ConfigureAwait(ConfigureAwaitOptions.None);
    }

    /// <inheritdoc/>
    public async Task AddFavouriteAppForUserAsync(Guid appId)
    {
        _portalRepositories.GetInstance<IOfferRepository>().CreateAppFavourite(appId, _identityData.IdentityId);
        await _portalRepositories.SaveAsync().ConfigureAwait(ConfigureAwaitOptions.None);
    }

    /// <inheritdoc />
    public Task<Pagination.Response<OfferSubscriptionStatusDetailData>> GetCompanySubscribedAppSubscriptionStatusesForUserAsync(int page, int size, OfferSubscriptionStatusId? statusId, string? name) =>
        _offerService.GetCompanySubscribedOfferSubscriptionStatusesForUserAsync(page, size, OfferTypeId.APP, DocumentTypeId.APP_LEADIMAGE, statusId, name);

    /// <inheritdoc/>
    public async Task<Pagination.Response<OfferCompanySubscriptionStatusResponse>> GetCompanyProvidedAppSubscriptionStatusesForUserAsync(int page, int size, SubscriptionStatusSorting? sorting, OfferSubscriptionStatusId? statusId, Guid? offerId, string? companyName)
    {
        if (companyName != null && !companyName.IsValidCompanyName())
        {
            throw ControllerArgumentException.Create(ValidationExpressionErrors.INCORRECT_COMPANY_NAME, [new ErrorParameter("name", "CompanyName")]);
        }

        async Task<Pagination.Source<OfferCompanySubscriptionStatusResponse>?> GetCompanyProvidedAppSubscriptionStatusData(int skip, int take)
        {
            var offerCompanySubscriptionResponse = await _portalRepositories.GetInstance<IOfferSubscriptionsRepository>()
                .GetOwnCompanyProvidedOfferSubscriptionStatusesUntrackedAsync(_identityData.CompanyId, OfferTypeId.APP, sorting, OfferSubscriptionService.GetOfferSubscriptionFilterStatusIds(statusId), offerId, companyName)(skip, take).ConfigureAwait(ConfigureAwaitOptions.None);

            return offerCompanySubscriptionResponse == null
                ? null
                : new Pagination.Source<OfferCompanySubscriptionStatusResponse>(
                    offerCompanySubscriptionResponse.Count,
                    offerCompanySubscriptionResponse.Data.Select(item =>
                        new OfferCompanySubscriptionStatusResponse(
                            item.OfferId,
                            item.ServiceName,
                            item.CompanySubscriptionStatuses.Select(x => x.GetCompanySubscriptionStatus(item.OfferId)),
                            item.Image == Guid.Empty ? null : item.Image)));
        }
        return await Pagination.CreateResponseAsync(page, size, _settings.ApplicationsMaxPageSize, GetCompanyProvidedAppSubscriptionStatusData).ConfigureAwait(ConfigureAwaitOptions.None);
    }

    /// <inheritdoc/>
    public Task<Guid> AddOwnCompanyAppSubscriptionAsync(Guid appId, IEnumerable<OfferAgreementConsentData> offerAgreementConsentData) =>
        _offerSubscriptionService.AddOfferSubscriptionAsync(appId, offerAgreementConsentData, OfferTypeId.APP, _settings.BasePortalAddress, _settings.SubscriptionManagerRoles, _settings.ServiceManagerRoles);

    /// <inheritdoc/>
    public Task DeclineAppSubscriptionAsync(Guid subscriptionId) =>
        _offerSubscriptionService.RemoveOfferSubscriptionAsync(subscriptionId, OfferTypeId.APP, _settings.BasePortalAddress);

    /// <inheritdoc/>
    public Task TriggerActivateOfferSubscription(Guid subscriptionId) =>
        _offerSetupService.TriggerActivateSubscription(subscriptionId);

    /// <inheritdoc/>
    public Task UnsubscribeOwnCompanyAppSubscriptionAsync(Guid subscriptionId) =>
        _offerService.UnsubscribeOwnCompanySubscriptionAsync(subscriptionId);

    /// <inheritdoc/>
    public Task<Pagination.Response<AllOfferData>> GetCompanyProvidedAppsDataForUserAsync(int page, int size, OfferSorting? sorting, string? offerName, AppStatusIdFilter? statusId) =>
        Pagination.CreateResponseAsync(page, size, 15,
            _portalRepositories.GetInstance<IOfferRepository>().GetProvidedOffersData(GetOfferStatusIds(statusId), OfferTypeId.APP, _identityData.CompanyId, sorting ?? OfferSorting.DateDesc, offerName));

    private static IEnumerable<OfferStatusId> GetOfferStatusIds(AppStatusIdFilter? appStatusIdFilter) =>
        appStatusIdFilter switch
        {
            AppStatusIdFilter.Active => Enumerable.Repeat(OfferStatusId.ACTIVE, 1),
            AppStatusIdFilter.Inactive => Enumerable.Repeat(OfferStatusId.INACTIVE, 1),
            AppStatusIdFilter.WIP => Enumerable.Repeat(OfferStatusId.CREATED, 1),
            _ => Enum.GetValues<OfferStatusId>()
        };

    /// <inheritdoc />
    public Task<OfferAutoSetupResponseData> AutoSetupAppAsync(OfferAutoSetupData data) =>
        _offerSetupService.AutoSetupOfferAsync(data, _settings.ITAdminRoles, OfferTypeId.APP, _settings.UserManagementAddress, _settings.ServiceManagerRoles);

    /// <inheritdoc />
    public Task StartAutoSetupAsync(OfferAutoSetupData data) =>
        _offerSetupService.StartAutoSetupAsync(data, OfferTypeId.APP);

    /// <inheritdoc />
    public Task ActivateSingleInstance(Guid offerSubscriptionId) =>
        _offerSetupService.CreateSingleInstanceSubscriptionDetail(offerSubscriptionId);

    /// <inheritdoc />
    public IAsyncEnumerable<AgreementData> GetAppAgreement(Guid appId) =>
        _offerService.GetOfferAgreementsAsync(appId, OfferTypeId.APP);

    /// <inheritdoc />
    public Task<(byte[] Content, string ContentType, string FileName)> GetAppDocumentContentAsync(Guid appId, Guid documentId, CancellationToken cancellationToken) =>
        _offerService.GetOfferDocumentContentAsync(appId, documentId, _settings.AppImageDocumentTypeIds, OfferTypeId.APP, cancellationToken);

    /// <inheritdoc />
    public Task<OfferProviderSubscriptionDetailData> GetSubscriptionDetailForProvider(Guid appId, Guid subscriptionId) =>
        _offerService.GetOfferSubscriptionDetailsForProviderAsync(appId, subscriptionId, OfferTypeId.APP, _settings.CompanyAdminRoles, new WalletConfigData(_settings.IssuerDid, _settings.BpnDidResolverUrl, _settings.DecentralIdentityManagementAuthUrl));

    /// <inheritdoc />
    public Task<SubscriberSubscriptionDetailData> GetSubscriptionDetailForSubscriber(Guid appId, Guid subscriptionId) =>
        _offerService.GetSubscriptionDetailsForSubscriberAsync(appId, subscriptionId, OfferTypeId.APP, _settings.SalesManagerRoles);

    /// <inheritdoc />
    public IAsyncEnumerable<ActiveOfferSubscriptionStatusData> GetOwnCompanyActiveSubscribedAppSubscriptionStatusesForUserAsync() =>
        _portalRepositories.GetInstance<IOfferSubscriptionsRepository>().GetOwnCompanyActiveSubscribedOfferSubscriptionStatusesUntrackedAsync(_identityData.CompanyId, OfferTypeId.APP, DocumentTypeId.APP_LEADIMAGE);

    /// <inheritdoc />
    public IAsyncEnumerable<OfferSubscriptionData> GetOwnCompanySubscribedAppOfferSubscriptionDataForUserAsync() =>
        _portalRepositories.GetInstance<IOfferSubscriptionsRepository>().GetOwnCompanySubscribedOfferSubscriptionUntrackedAsync(_identityData.CompanyId, OfferTypeId.APP);
}

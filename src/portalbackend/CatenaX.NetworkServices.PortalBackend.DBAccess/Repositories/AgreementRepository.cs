﻿/********************************************************************************
 * Copyright (c) 2021,2022 BMW Group AG
 * Copyright (c) 2021,2022 Contributors to the CatenaX (ng) GitHub Organisation.
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

using CatenaX.NetworkServices.PortalBackend.DBAccess.Models;
using CatenaX.NetworkServices.PortalBackend.PortalEntities;
using CatenaX.NetworkServices.PortalBackend.PortalEntities.Enums;
using Microsoft.EntityFrameworkCore;

namespace CatenaX.NetworkServices.PortalBackend.DBAccess.Repositories;

/// <inheritdoc />
public class AgreementRepository : IAgreementRepository
{
    private readonly PortalDbContext _context;

    /// <summary>
    /// Creates a new instance of <see cref="AgreementRepository"/>
    /// </summary>
    /// <param name="context">Access to the database context</param>
    public AgreementRepository(PortalDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public Task<bool> CheckAgreementExistsForSubscriptionAsync(Guid agreementId, Guid subscriptionId, OfferTypeId offerTypeId) =>
        _context.Agreements.AnyAsync(agreement =>
            agreement.Id == agreementId &&
            agreement.AgreementAssignedOffers.Any(aao =>
                aao.Offer!.OfferTypeId == offerTypeId &&
                aao.Offer.OfferSubscriptions.Any(subscription => subscription.Id == subscriptionId)));

    /// <inheritdoc />
    public IAsyncEnumerable<AgreementData> GetOfferAgreementDataForIamUser(string iamUserId, OfferTypeId offerTypeId) =>
        _context.Agreements
            .Where(x => x.AgreementAssignedOffers
                .Any(app => 
                    app.Offer!.OfferTypeId == offerTypeId &&
                    (app.Offer!.OfferSubscriptions.Any(os => os.Company!.CompanyUsers.Any(cu => cu.IamUser!.UserEntityId == iamUserId)) ||
                        app.Offer!.ProviderCompany!.CompanyUsers.Any(cu => cu.IamUser!.UserEntityId == iamUserId))
                ))
            .Select(x => new AgreementData(x.Id, x.Name))
            .AsAsyncEnumerable();
    
    public IAsyncEnumerable<AgreementData> GetAgreementsForCompanyRolesUntrackedAsync() =>
        _context.Agreements
            .AsNoTracking()
            .Where(agreement => agreement.AgreementAssignedCompanyRoles.Any())
            .Select(agreement => new AgreementData(
                agreement.Id,
                agreement.Name))
            .AsAsyncEnumerable();

    ///<inheritdoc/>
    public IAsyncEnumerable<AgreementData> GetAgreements(AgreementCategoryId categoryId)
    =>
        _context.Agreements
            .AsNoTracking()
            .Where(agreement=>agreement.AgreementCategoryId == categoryId)
            .Select(agreement=> new  AgreementData(
                agreement.Id,
                agreement.Name
            ))
            .AsAsyncEnumerable();

    ///<inheritdoc/>
    public Task<OfferAgreementConsent?> GetOfferAgreementConsentById(Guid appId, string userId, AgreementCategoryId categoryId)
    =>
        _context.Offers
            .AsNoTracking()
            .Where(offer=>offer.Id == appId && offer.ProviderCompany!.CompanyUsers.Any(companyUser => companyUser.IamUser!.UserEntityId == userId))
            .Select(offer=> new OfferAgreementConsent(
                offer.ConsentAssignedOffers!.Where(consentAssignedOffer => consentAssignedOffer.Consent!.Agreement!.AgreementCategoryId == categoryId).Select(consentAssignedOffer => new AgreementConsentStatus(
                consentAssignedOffer.Consent!.AgreementId,
                consentAssignedOffer.Consent!.ConsentStatusId
            ))))
            .SingleOrDefaultAsync();
    
    ///<inheritdoc/>
    public Task<OfferAgreementConsentUpdate?> GetOfferAgreementConsent(Guid appId, string userId, OfferStatusId statusId, AgreementCategoryId categoryId)
    =>
        _context.Offers
            .AsNoTracking()
            .Where(offer=>offer.Id == appId && offer.OfferStatusId == statusId 
            && offer.ProviderCompany!.CompanyUsers.Any(companyUser => companyUser.IamUser!.UserEntityId == userId))
            .Select(offer=> new OfferAgreementConsentUpdate(
                offer.ProviderCompany!.CompanyUsers.Select(companyUser=>companyUser.Id).SingleOrDefault(),
                offer.ProviderCompany.Id,
                offer.ConsentAssignedOffers!.Where(consentAssignedOffer => consentAssignedOffer.Consent!.Agreement!.AgreementCategoryId == categoryId).Select(consentAssignedOffer => new AppAgreementConsentStatus(
                consentAssignedOffer.Consent!.AgreementId,
                consentAssignedOffer.Consent!.Id,
                consentAssignedOffer.Consent!.ConsentStatusId
            ))))
            .SingleOrDefaultAsync();
}

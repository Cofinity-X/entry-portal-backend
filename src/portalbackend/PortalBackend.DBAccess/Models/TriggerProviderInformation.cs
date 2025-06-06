/********************************************************************************
 * Copyright (c) 2023 Contributors to the Eclipse Foundation
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

using Org.Eclipse.TractusX.Portal.Backend.PortalBackend.PortalEntities.Enums;

namespace Org.Eclipse.TractusX.Portal.Backend.PortalBackend.DBAccess.Models;

public record TriggerProviderInformation(
    Guid OfferId,
    string? OfferName,
    string? AutoSetupUrl,
    ProviderAuthInformation? AuthDetails,
    CompanyInformationData CompanyInformationData,
    OfferTypeId OfferTypeId,
    Guid? SalesManagerId,
    Guid CompanyUserId,
    bool IsSingleInstance
);

public record ProviderAuthInformation(
    string AuthUrl,
    string ClientId,
    byte[] ClientSecret,
    byte[]? InitializationVector,
    int EncryptionMode
);

public record SubscriptionActivationData(
    Guid OfferId,
    OfferSubscriptionStatusId Status,
    OfferTypeId OfferTypeId,
    string? OfferName,
    string CompanyName,
    Guid CompanyId,
    string? RequesterEmail,
    string? RequesterFirstname,
    string? RequesterLastname,
    Guid RequesterId,
    (bool IsSingleInstance, string? InstanceUrl) InstanceData,
    IEnumerable<Guid> AppInstanceIds,
    Guid? OfferSubscriptionProcessDataId,
    Guid? SalesManagerId,
    Guid? ProviderCompanyId,
    string? ClientClientId,
    IEnumerable<string> InternalServiceAccountClientIds,
    bool HasCallbackUrl
);

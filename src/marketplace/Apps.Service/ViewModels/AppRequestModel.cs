/********************************************************************************
 * Copyright (c) 2021, 2023 BMW Group AG
 * Copyright (c) 2021, 2023 Contributors to the Eclipse Foundation
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

using Org.Eclipse.TractusX.Portal.Backend.Framework.Models.Validation;
using Org.Eclipse.TractusX.Portal.Backend.PortalBackend.DBAccess.Models;
using Org.Eclipse.TractusX.Portal.Backend.PortalBackend.PortalEntities.Enums;

namespace Org.Eclipse.TractusX.Portal.Backend.Apps.Service.ViewModels;

/// <summary>
/// Request Model for App Creation.
/// </summary>
/// <param name="Title">Title</param>
/// <param name="Provider">Provider</param>
/// <param name="SalesManagerId">SalesManagerId</param>
/// <param name="UseCaseIds">UseCaseIds</param>
/// <param name="Descriptions">Descriptions</param>
/// <param name="SupportedLanguageCodes">SupportedLanguageCodes</param>
/// <param name="Price">Price</param>
/// <param name="PrivacyPolicies">Price</param>
/// <param name="ProviderUri">Price</param>
/// <param name="ContactEmail">Price</param>
/// <param name="ContactNumber">Price</param>
public record AppRequestModel(
    string? Title,
    string Provider,
    Guid? SalesManagerId,
    IEnumerable<Guid> UseCaseIds,
    IEnumerable<LocalizedDescription> Descriptions,
    IEnumerable<string> SupportedLanguageCodes,
    string Price,
    [ValidateEnumValues] IEnumerable<PrivacyPolicyId> PrivacyPolicies,
    string? ProviderUri,
    string? ContactEmail,
    string? ContactNumber
);

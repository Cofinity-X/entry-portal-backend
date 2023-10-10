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

using Org.Eclipse.TractusX.Portal.Backend.PortalBackend.DBAccess.Models;
using Org.Eclipse.TractusX.Portal.Backend.PortalBackend.PortalEntities.Entities;
using Org.Eclipse.TractusX.Portal.Backend.PortalBackend.PortalEntities.Enums;

namespace Org.Eclipse.TractusX.Portal.Backend.PortalBackend.DBAccess.Repositories;

public interface INetworkRepository
{
    NetworkRegistration CreateNetworkRegistration(Guid externalId, Guid companyId, Guid processId, Guid ospId, Guid applicationId);
    Task<bool> CheckExternalIdExists(Guid externalId, Guid onboardingServiceProviderId);
    Task<Guid> GetNetworkRegistrationDataForProcessIdAsync(Guid processId);
    Task<(bool RegistrationIdExists, VerifyProcessData processData)> IsValidRegistration(Guid externalId, IEnumerable<ProcessStepTypeId> processStepTypeIds);
    Task<(bool Exists, IEnumerable<(Guid CompanyApplicationId, CompanyApplicationStatusId CompanyApplicationStatusId, string? CallbackUrl)> CompanyApplications, IEnumerable<(CompanyRoleId CompanyRoleId, IEnumerable<Guid> AgreementIds)> CompanyRoleAgreementIds, Guid? ProcessId)> GetSubmitData(Guid companyId);
    Task<(OspDetails? OspDetails, Guid? ExternalId, string? Bpn, Guid ApplicationId, IEnumerable<string> Comments)> GetCallbackData(Guid networkRegistrationId, ProcessStepTypeId processStepTypeId);
    Task<string?> GetOspCompanyName(Guid networkRegistrationId);
}
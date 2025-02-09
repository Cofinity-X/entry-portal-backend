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

using Org.Eclipse.TractusX.Portal.Backend.Provisioning.Library.Enums;
using Org.Eclipse.TractusX.Portal.Backend.Provisioning.Library.Models;

namespace Org.Eclipse.TractusX.Portal.Backend.Provisioning.Library;

public interface IProvisioningManager
{
    ValueTask<string> GetNextCentralIdentityProviderNameAsync();
    Task<string> CreateSharedRealmUserAsync(string realm, UserProfile profile);
    Task<string> CreateCentralUserAsync(UserProfile profile, IEnumerable<(string Name, IEnumerable<string> Values)> attributes);
    IAsyncEnumerable<(string Client, IEnumerable<string> Roles, Exception? Error)> AssignClientRolesToCentralUserAsync(string centralUserId, IDictionary<string, IEnumerable<string>> clientRoleNames);
    Task AddRolesToClientAsync(string clientName, IEnumerable<string> roleNames);
    Task<string> CreateOwnIdpAsync(string displayName, string organisationName, IamIdentityProviderProtocol providerProtocol);
    Task<string?> GetProviderUserIdForCentralUserIdAsync(string identityProvider, string userId);
    IAsyncEnumerable<IdentityProviderLink> GetProviderUserLinkDataForCentralUserIdAsync(string userId);
    Task AddProviderUserLinkToCentralUserAsync(string userId, IdentityProviderLink identityProviderLink);
    Task DeleteProviderUserLinkToCentralUserAsync(string userId, string alias);
    Task UpdateSharedRealmUserAsync(string realm, string userId, string firstName, string lastName, string email);
    Task UpdateCentralUserAsync(string userId, string firstName, string lastName, string email);
    Task DeleteSharedRealmUserAsync(string realm, string userId);
    Task DeleteCentralRealmUserAsync(string userId);
    Task<string> SetupClientAsync(string redirectUrl, string? baseUrl = null, IEnumerable<string>? optionalRoleNames = null, bool enabled = true);
    Task<ServiceAccountData> SetupCentralServiceAccountClientAsync(string clientId, ClientConfigRolesData config, bool enabled);
    Task<string?> GetServiceAccountUserId(string clientId);
    Task<string> UpdateCentralClientAsync(string clientId, ClientConfigData config);
    Task DeleteCentralClientAsync(string clientId);
    Task UpdateClient(string clientId, string url, string redirectUrl);
    Task EnableClient(string clientId);
    Task<ClientAuthData> GetCentralClientAuthDataAsync(string internalClientId);
    Task<ClientAuthData> ResetCentralClientAuthDataAsync(string clientId);
    Task<string> GetIdOfCentralClientAsync(string clientId);
    Task AddBpnAttributetoUserAsync(string centralUserId, IEnumerable<string> bpns);
    Task AddProtocolMapperAsync(string clientId);
    Task DeleteCentralUserBusinessPartnerNumberAsync(string centralUserId, string businessPartnerNumber);
    Task ResetSharedUserPasswordAsync(string realm, string userId);
    Task<IEnumerable<string>> GetClientRoleMappingsForUserAsync(string userId, string clientId);
    ValueTask<bool> IsCentralIdentityProviderEnabled(string alias);
    Task<string?> GetCentralIdentityProviderDisplayName(string alias);
    ValueTask<IdentityProviderConfigOidc> GetCentralIdentityProviderDataOIDCAsync(string alias);
    ValueTask SetSharedIdentityProviderStatusAsync(string alias, bool enabled);
    ValueTask SetCentralIdentityProviderStatusAsync(string alias, bool enabled);
    ValueTask UpdateSharedIdentityProviderAsync(string alias, string displayName);
    ValueTask UpdateCentralIdentityProviderDataOIDCAsync(IdentityProviderEditableConfigOidc identityProviderConfigOidc, CancellationToken cancellationToken);
    ValueTask<IdentityProviderConfigSaml> GetCentralIdentityProviderDataSAMLAsync(string alias);
    ValueTask UpdateCentralIdentityProviderDataSAMLAsync(IdentityProviderEditableConfigSaml identityProviderEditableConfigSaml);
    Task DeleteCentralIdentityProviderAsync(string alias);
    IAsyncEnumerable<IdentityProviderMapperModel> GetIdentityProviderMappers(string alias);
    ValueTask DeleteSharedIdpRealmAsync(string alias);
    IEnumerable<(string AttributeName, IEnumerable<string> AttributeValues)> GetStandardAttributes(string? organisationName = null, string? businessPartnerNumber = null);
    Task DeleteClientRolesFromCentralUserAsync(string centralUserId, IDictionary<string, IEnumerable<string>> clientRoleNames);
    ValueTask UpdateSharedRealmTheme(string alias, string loginTheme);
    Task<string?> GetUserByUserName(string userName);
    Task<string?> GetIdentityProviderDisplayName(string alias);
    Task DeleteSharedRealmAsync(string alias);
    Task DeleteIdpSharedServiceAccount(string alias);
    Task UpdateOrCreateCentralIdentityProviderOrganisationMapperAsync(string idpAlias, string organisationName);
}

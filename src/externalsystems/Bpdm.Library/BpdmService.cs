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
using Org.Eclipse.TractusX.Portal.Backend.Bpdm.Library.Models;
using Org.Eclipse.TractusX.Portal.Backend.Framework.ErrorHandling;
using Org.Eclipse.TractusX.Portal.Backend.Framework.HttpClientExtensions;
using Org.Eclipse.TractusX.Portal.Backend.Framework.Token;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Org.Eclipse.TractusX.Portal.Backend.Bpdm.Library;

public class BpdmService : IBpdmService
{
    private readonly ITokenService _tokenService;
    private readonly BpdmServiceSettings _settings;
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters =
        {
            new JsonStringEnumConverter(allowIntegerValues: false),
        },
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public BpdmService(ITokenService tokenService, IOptions<BpdmServiceSettings> options)
    {
        _tokenService = tokenService;
        _settings = options.Value;
    }

    /// <inheritdoc />
    public async Task<bool> PutInputLegalEntity(BpdmTransferData data, CancellationToken cancellationToken)
    {
        using var httpClient = await _tokenService.GetAuthorizedClient<BpdmService>(_settings, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);

        var requestData = new BpdmLegalEntityData[]
        {
            new(
                data.ExternalId,                               // Index
                new []
                {
                    data.CompanyName   // LegalName
                },
                data.Identifiers.Select(x =>
                    new BpdmIdentifier(
                        x.BpdmIdentifierId,                    // Type
                        x.Value,                               // Value
                        null)),                                // IssuingBody
                Enumerable.Empty<BpdmState>(),                 // Status
                Enumerable.Empty<string>(),                    // Roles
                new BpdmLegalEntity(
                    null,
                    data.CompanyName,
                    data.ShortName,
                    null,
                    Enumerable.Empty<BpdmState>()
                ),
                null,
                new BpdmAddress(
                    null,
                    null,
                    null,
                    new BpdmPutPhysicalPostalAddress(
                        null,
                        data.AlphaCode2,
                        null,
                        null,
                        null,
                        data.ZipCode,
                        data.City,
                        data.Region,
                        new BpdmPutStreet(
                            null,
                            null,
                            data.StreetName,
                            null,
                            null,
                            data.StreetNumber,
                            null,
                            null,
                            null),
                        null,
                        null,
                        null,
                        null,
                        null
                    ),
                    null,
                    Enumerable.Empty<BpdmState>()
                ),
                true
            )
        };

        await httpClient.PutAsJsonAsync("input/business-partners", requestData, Options, cancellationToken)
            .CatchingIntoServiceExceptionFor("bpdm-put-legal-entities", HttpAsyncResponseMessageExtension.RecoverOptions.INFRASTRUCTURE).ConfigureAwait(false);
        return true;
    }

    public async Task<bool> SetSharingStateToReady(string externalId, CancellationToken cancellationToken)
    {
        using var httpClient = await _tokenService.GetAuthorizedClient<BpdmService>(_settings, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);

        var content = new { externalIds = Enumerable.Repeat(externalId, 1) };
        await httpClient.PostAsJsonAsync("sharing-state/ready", content, Options, cancellationToken)
            .CatchingIntoServiceExceptionFor("bpdm-put-sharing-state-ready", HttpAsyncResponseMessageExtension.RecoverOptions.INFRASTRUCTURE).ConfigureAwait(false);
        return true;
    }

    public async Task<BpdmLegalEntityOutputData> FetchInputLegalEntity(string externalId, CancellationToken cancellationToken)
    {
        using var httpClient = await _tokenService.GetAuthorizedClient<BpdmService>(_settings, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);

        var data = Enumerable.Repeat(externalId, 1);
        var result = await httpClient.PostAsJsonAsync("output/business-partners/search", data, Options, cancellationToken)
            .CatchingIntoServiceExceptionFor("bpdm-search-legal-entities", HttpAsyncResponseMessageExtension.RecoverOptions.INFRASTRUCTURE).ConfigureAwait(false);
        try
        {
            var response = await result.Content
                .ReadFromJsonAsync<PageOutputResponseBpdmLegalEntityData>(Options, cancellationToken)
                .ConfigureAwait(ConfigureAwaitOptions.None);
            if (response?.Content?.Count() != 1)
            {
                throw new ServiceException("Access to external system bpdm did not return a valid legal entity response", true);
            }

            return response.Content.Single();
        }
        catch (JsonException je)
        {
            throw new ServiceException($"Access to external system bpdm did not return a valid json response: {je.Message}");
        }
    }

    public async Task<BpdmSharingState> GetSharingState(Guid applicationId, CancellationToken cancellationToken)
    {
        using var httpClient = await _tokenService.GetAuthorizedClient<BpdmService>(_settings, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);

        var url = $"sharing-state?externalIds={applicationId}";
        var result = await httpClient.GetAsync(url, cancellationToken)
            .CatchingIntoServiceExceptionFor("bpdm-sharing-state", HttpAsyncResponseMessageExtension.RecoverOptions.INFRASTRUCTURE).ConfigureAwait(false);
        try
        {
            var response = await result.Content
                .ReadFromJsonAsync<BpdmPaginationSharingStateOutput>(Options, cancellationToken)
                .ConfigureAwait(ConfigureAwaitOptions.None);
            if (response?.Content?.Count() != 1)
            {
                throw new ServiceException("Access to sharing state did not return a valid legal entity response", true);
            }

            return response.Content.Single();
        }
        catch (JsonException je)
        {
            throw new ServiceException($"Access to sharing state did not return a valid json response: {je.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<bool> SetCxMembership(string businessPartnerNumber, CancellationToken cancellationToken)
    {
        using var httpClient = await _tokenService.GetAuthorizedClient($"{typeof(BpdmService).Name}Pool", _settings, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);

        var requestData = new BpdmCxMembership(
            new BpdmCxMembershipDto[]{
                new(businessPartnerNumber, true)
            }.AsEnumerable()
        );

        async ValueTask<(bool, string?)> CreateErrorMessage(HttpResponseMessage errorResponse) =>
            (false, await errorResponse.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None));

        await httpClient.PutAsJsonAsync("cx-memberships", requestData, Options, cancellationToken)
            .CatchingIntoServiceExceptionFor("bpdm-put-cx-membership", HttpAsyncResponseMessageExtension.RecoverOptions.INFRASTRUCTURE, CreateErrorMessage).ConfigureAwait(false);
        return true;
    }
}

/********************************************************************************
 * Copyright (c) 2025 Contributors to the Eclipse Foundation
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
using Org.Eclipse.TractusX.Portal.Backend.Framework.HttpClientExtensions;
using Org.Eclipse.TractusX.Portal.Backend.UniversalDidResolver.Library.Models;
using System.Net.Http.Json;
using System.Text.Json;
namespace Org.Eclipse.TractusX.Portal.Backend.UniversalDidResolver.Library;

public class UniversalDidResolverService(IHttpClientFactory httpClientFactory) : IUniversalDidResolverService
{
    private static readonly JsonSerializerOptions Options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    public async Task<bool> ValidateDid(string did, CancellationToken cancellationToken)
    {
        using var httpClient = httpClientFactory.CreateClient("universalResolver");
        var result = await httpClient.GetAsync($"1.0/identifiers/{Uri.EscapeDataString(did)}", cancellationToken)
            .CatchingIntoServiceExceptionFor("validate-did", HttpAsyncResponseMessageExtension.RecoverOptions.INFRASTRUCTURE).ConfigureAwait(false);

        if (!result.IsSuccessStatusCode)
        {
            return false;
        }

        var validationResult = await result.Content.ReadFromJsonAsync<DidValidationResult>(Options, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);
        return validationResult != null && string.IsNullOrWhiteSpace(validationResult.DidResolutionMetadata.Error);
    }
}

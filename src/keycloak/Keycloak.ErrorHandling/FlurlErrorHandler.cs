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

using Flurl.Http;
using Microsoft.Extensions.Logging;
using Org.Eclipse.TractusX.Portal.Backend.Framework.ErrorHandling;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Org.Eclipse.TractusX.Portal.Backend.Keycloak.ErrorHandling;

public static class FlurlErrorHandler
{
    public static void ConfigureErrorHandler(ILogger logger)
    {
        FlurlHttp.Clients.WithDefaults(c => c.OnError(call =>
        {
            var message = $"{call.HttpResponseMessage?.ReasonPhrase ?? "ReasonPhrase is null"}: {call.HttpRequestMessage.RequestUri}";

            if (logger.IsEnabled(LogLevel.Debug))
            {
                LogDebug(logger, call);
            }

            if (call.HttpResponseMessage is null)
            {
                throw new ServiceException(message, call.Exception);
            }

            var errorContent = JsonSerializer.Deserialize<KeycloakErrorResponse>(call.HttpResponseMessage.Content.ReadAsStream())?.ErrorMessage;
            if (!string.IsNullOrWhiteSpace(errorContent))
            {
                message = errorContent;
            }

            throw call.HttpResponseMessage.StatusCode switch
            {
                HttpStatusCode.NotFound => new KeycloakEntityNotFoundException(message, call.Exception),
                HttpStatusCode.Conflict => new KeycloakEntityConflictException(message, call.Exception),
                HttpStatusCode.BadRequest => new KeycloakNoSuccessException(message, call.Exception),
                _ => new ServiceException(message, call.Exception, call.HttpResponseMessage.StatusCode),
            };
        }));
    }

    private static void LogDebug(ILogger logger, FlurlCall call)
    {
        var request = call.HttpRequestMessage == null ? "" : $"{call.HttpRequestMessage.Method} {call.HttpRequestMessage.RequestUri} HTTP/{call.HttpRequestMessage.Version}\n{call.HttpRequestMessage.Headers}\n";
        var requestBody = call.RequestBody == null ? "\n" : call.RequestBody + "\n\n";
        var response = call.HttpResponseMessage == null ? "" : call.HttpResponseMessage.ReasonPhrase + "\n";
        var responseContent = call.HttpResponseMessage?.Content == null ? "" : call.HttpResponseMessage.Content.ReadAsStringAsync().Result + "\n";
        logger.LogDebug(call.Exception, "{Request}{Body}{Response}{Content}", request, requestBody, response, responseContent);
    }

    public class KeycloakErrorResponse
    {
        [JsonPropertyName("errorMessage")]
        public string? ErrorMessage { get; set; }
    }
}

/********************************************************************************
 * MIT License
 *
 * Copyright (c) 2019 Luk Vermeulen
 * Copyright (c) 2022 BMW Group AG
 * Copyright (c) 2022 Contributors to the Eclipse Foundation
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 ********************************************************************************/

using System.Text.Json.Serialization;

namespace Org.Eclipse.TractusX.Portal.Backend.Keycloak.Library.Models.Root;

public class PolicyProviders
{
    [JsonPropertyName("role")]
    public HasOrder Role { get; set; }

    [JsonPropertyName("resource")]
    public HasOrder Resource { get; set; }

    [JsonPropertyName("scope")]
    public HasOrder Scope { get; set; }

    [JsonPropertyName("uma")]
    public HasOrder Uma { get; set; }

    [JsonPropertyName("client")]
    public HasOrder Client { get; set; }

    [JsonPropertyName("js")]
    public HasOrder Js { get; set; }

    [JsonPropertyName("time")]
    public HasOrder Time { get; set; }

    [JsonPropertyName("user")]
    public HasOrder User { get; set; }

    [JsonPropertyName("aggregate")]
    public HasOrder Aggregate { get; set; }

    [JsonPropertyName("group")]
    public HasOrder Group { get; set; }
}

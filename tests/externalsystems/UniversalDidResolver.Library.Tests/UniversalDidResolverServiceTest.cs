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

using AutoFixture;
using AutoFixture.AutoFakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Org.Eclipse.TractusX.Portal.Backend.Framework.ErrorHandling;
using Org.Eclipse.TractusX.Portal.Backend.Tests.Shared.Extensions;
using Org.Eclipse.TractusX.Portal.Backend.UniversalDidResolver.Library.DependencyInjection;
using Org.Eclipse.TractusX.Portal.Backend.UniversalDidResolver.Library.Models;
using System.Net;
using System.Text.Json;
using Xunit;

namespace Org.Eclipse.TractusX.Portal.Backend.UniversalDidResolver.Library.Tests;

public class UniversalDidResolverServiceTests
{
    private readonly IFixture _fixture;
    private readonly UniversalDidResolverSettings _settings;
    private readonly IUniversalDidResolverService _sut;
    public UniversalDidResolverServiceTests()
    {
        _fixture = new Fixture().Customize(new AutoFakeItEasyCustomization { ConfigureMembers = true });
        _fixture.ConfigureFixture();

        _settings = new UniversalDidResolverSettings
        {
            UniversalResolverAddress = "https://dev.uniresolver.io/"
        };
        _fixture.Inject(Options.Create(_settings));
        _sut = new UniversalDidResolverService(_fixture.Freeze<IHttpClientFactory>());
    }

    [Fact]
    public async Task ValidateDid_ReturnsTrue_WhenDidIsValid()
    {
        // Arrange
        const string did = "did:web:123";
        HttpRequestMessage? request = null;
        var didValidationResult = new DidValidationResult(new DidResolutionMetadata(null));
        using var responseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(didValidationResult))
        };
        _fixture.ConfigureHttpClientFactoryFixture("universalResolver", responseMessage, requestMessage => request = requestMessage, _settings.UniversalResolverAddress);

        // Act
        var result = await _sut.ValidateDid(did, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        request.Should().NotBeNull();
        request!.RequestUri.Should().NotBeNull();
        request.RequestUri.Should().NotBeNull();
        request.RequestUri!.AbsoluteUri.Should().Be($"https://dev.uniresolver.io/1.0/identifiers/{Uri.EscapeDataString(did)}");
    }

    [Fact]
    public async Task ValidateDid_ReturnsFalse_WhenDidHasError()
    {
        // Arrange
        const string did = "did:web:123";
        HttpRequestMessage? request = null;
        var didValidationResult = new DidValidationResult(new DidResolutionMetadata(null));
        using var responseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NotFound,
            Content = new StringContent(JsonSerializer.Serialize(didValidationResult))
        };
        _fixture.ConfigureHttpClientFactoryFixture("universalResolver", responseMessage, requestMessage => request = requestMessage, _settings.UniversalResolverAddress);

        // Act
        async Task Act() => await _sut.ValidateDid(did, CancellationToken.None);

        // Assert
        var ex = await Assert.ThrowsAsync<ServiceException>(Act);
        ex.Message.Should().Be("call to external system validate-did failed with statuscode 404");
    }

    [Fact]
    public async Task ValidateDid_ReturnsFalse_WhenHttpStatusIsNotSuccess()
    {
        // Arrange
        const string did = "did:web:123";
        HttpRequestMessage? request = null;
        var didValidationResult = new DidValidationResult(new DidResolutionMetadata(null));
        using var responseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.BadRequest,
            Content = new StringContent(JsonSerializer.Serialize(didValidationResult))
        };
        _fixture.ConfigureHttpClientFactoryFixture("universalResolver", responseMessage, requestMessage => request = requestMessage, _settings.UniversalResolverAddress);

        // Act
        async Task Act() => await _sut.ValidateDid(did, CancellationToken.None);

        // Assert
        var ex = await Assert.ThrowsAsync<ServiceException>(Act);
        ex.Message.Should().Be("call to external system validate-did failed with statuscode 400");
    }

    [Fact]
    public async Task ValidateDid_ReturnsFalse_WhenDeserializationReturnsNull()
    {
        // Arrange
        const string did = "did:web:123";
        HttpRequestMessage? request = null;
        var didValidationResult = new DidValidationResult(new DidResolutionMetadata(null));
        using var responseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = null
        };
        _fixture.ConfigureHttpClientFactoryFixture("universalResolver", responseMessage, requestMessage => request = requestMessage, _settings.UniversalResolverAddress);

        // Act
        async Task Act() => await _sut.ValidateDid(did, CancellationToken.None);

        // Assert
        var ex = await Assert.ThrowsAsync<JsonException>(Act);
    }
}

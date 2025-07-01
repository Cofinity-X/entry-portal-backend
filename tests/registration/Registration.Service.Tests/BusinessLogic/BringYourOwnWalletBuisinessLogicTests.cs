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
using FakeItEasy;
using FluentAssertions;
using Org.Eclipse.TractusX.Portal.Backend.Registration.Service.BusinessLogic;
using Org.Eclipse.TractusX.Portal.Backend.Tests.Shared.Extensions;
using Org.Eclipse.TractusX.Portal.Backend.UniversalDidResolver.Library;
using Xunit;

namespace Org.Eclipse.TractusX.Portal.Backend.Registration.Service.Tests.BusinessLogic;

public class BringYourOwnWalletBuisinessLogicTests
{
    private readonly IFixture _fixture;
    private readonly IUniversalDidResolverService _universalDidResolverService;
    private readonly IBringYourOwnWalletBusinessLogic _sut;

    public BringYourOwnWalletBuisinessLogicTests()
    {
        _fixture = new Fixture().Customize(new AutoFakeItEasyCustomization { ConfigureMembers = true });
        _fixture.ConfigureFixture();

        _universalDidResolverService = A.Fake<IUniversalDidResolverService>();
        _sut = new BringYourOwnWalletBusinessLogic(_universalDidResolverService);
    }

    [Fact]
    public async Task ValidateDid_ReturnsTrue_WhenDidIsValid()
    {
        // Arrange
        const string did = "did:web:123";
        A.CallTo(() => _universalDidResolverService.ValidateDid(did, A<CancellationToken>._)).Returns(true);

        // Act
        var result = await _sut.ValidateDid(did, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateDid_ReturnsFalse_WhenDidHasError()
    {
        // Arrange
        const string did = "did:web:123";
        A.CallTo(() => _universalDidResolverService.ValidateDid(did, A<CancellationToken>._)).Returns(false);

        // Act
        var result = await _sut.ValidateDid(did, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ValidateDid_ReturnsFalse_WhenDidThrowsHttp400Exception()
    {
        // Arrange
        const string did = "did:web:404";
        var exception = new HttpRequestException("Bad request", null, System.Net.HttpStatusCode.BadRequest);
        A.CallTo(() => _universalDidResolverService.ValidateDid(did, A<CancellationToken>._)).Throws(exception);

        // Act
        async Task Act() => await _sut.ValidateDid(did, CancellationToken.None);

        // Assert
        var ex = await Assert.ThrowsAsync<HttpRequestException>(Act);
        ex.Message.Should().Be("Bad request");
    }
}

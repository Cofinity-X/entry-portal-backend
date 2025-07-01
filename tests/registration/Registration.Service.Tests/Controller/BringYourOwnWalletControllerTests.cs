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

using FakeItEasy;
using FluentAssertions;
using Org.Eclipse.TractusX.Portal.Backend.Framework.Identity;
using Org.Eclipse.TractusX.Portal.Backend.Registration.Service.BusinessLogic;
using Org.Eclipse.TractusX.Portal.Backend.Registration.Service.Controllers;
using Org.Eclipse.TractusX.Portal.Backend.Tests.Shared.Extensions;
using Xunit;

namespace Org.Eclipse.TractusX.Portal.Backend.Registration.Service.Tests;

public class BringYourOwnWalletControllerTests
{
    private readonly IIdentityData _identity;
    private readonly BringYourOwnWalletController _controller;
    private readonly IBringYourOwnWalletBusinessLogic _bringYourOwnWalletBusinessLogicFake;

    public BringYourOwnWalletControllerTests()
    {
        _identity = A.Fake<IIdentityData>();
        A.CallTo(() => _identity.IdentityId).Returns(Guid.NewGuid());
        A.CallTo(() => _identity.IdentityTypeId).Returns(IdentityTypeId.COMPANY_USER);
        A.CallTo(() => _identity.CompanyId).Returns(Guid.NewGuid());
        _bringYourOwnWalletBusinessLogicFake = A.Fake<IBringYourOwnWalletBusinessLogic>();
        _controller = new BringYourOwnWalletController(_bringYourOwnWalletBusinessLogicFake);
        _controller.AddControllerContextWithClaimAndBearer("ac-token", _identity);
    }

    [Fact]
    public async Task ValidateDid_WhenCalled_ShouldReturnOkResult()
    {
        // Arrange
        var did = "did:web:example.com";
        A.CallTo(() => _bringYourOwnWalletBusinessLogicFake.ValidateDid(did, A<CancellationToken>._)).Returns(true);

        // Act
        var result = await _controller.validateDid(did, CancellationToken.None);

        //Assert
        A.CallTo(() => _bringYourOwnWalletBusinessLogicFake.ValidateDid(did, CancellationToken.None)).MustHaveHappenedOnceExactly();

    }

    [Fact]
    public async Task ValidateDid_WhenDidIsValid_ShouldReturnOkWithTrue()
    {
        // Arrange
        var did = "did:web:example.com";
        A.CallTo(() => _bringYourOwnWalletBusinessLogicFake.ValidateDid(did, A<CancellationToken>._)).Returns(true);

        // Act
        var result = await _controller.validateDid(did, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateDid_WhenDidIsInvalid_ShouldReturnOkWithFalse()
    {
        // Arrange
        var did = "did:web:example.com";
        A.CallTo(() => _bringYourOwnWalletBusinessLogicFake.ValidateDid(did, A<CancellationToken>._)).Returns(false);

        // Act
        var result = await _controller.validateDid(did, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ValidateDid_WhenBusinessLogicThrowsBadRequest_ShouldThrowHttpRequestException()
    {
        // Arrange
        var did = "did:web:badrequest";
        var exception = new HttpRequestException("Bad request", null, System.Net.HttpStatusCode.BadRequest);
        A.CallTo(() => _bringYourOwnWalletBusinessLogicFake.ValidateDid(did, A<CancellationToken>._)).Throws(exception);

        // Act
        Func<Task> act = async () => await _controller.validateDid(did, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>()
            .WithMessage("Bad request");
    }
}

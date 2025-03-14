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

using Org.Eclipse.TractusX.Portal.Backend.PortalBackend.DBAccess.Repositories;
using Org.Eclipse.TractusX.Portal.Backend.PortalBackend.DBAccess.Tests.Setup;
using Org.Eclipse.TractusX.Portal.Backend.PortalBackend.PortalEntities;
using Org.Eclipse.TractusX.Portal.Backend.PortalBackend.PortalEntities.Enums;
using Xunit.Extensions.AssemblyFixture;

namespace Org.Eclipse.TractusX.Portal.Backend.PortalBackend.DBAccess.Tests;

/// <summary>
/// Tests the functionality of the <see cref="ServiceRepositoryTests"/>
/// </summary>
public class ServiceRepositoryTests : IAssemblyFixture<TestDbFixture>
{
    private readonly TestDbFixture _dbTestDbFixture;
    private readonly Guid _offerId = new("ac1cf001-7fbc-1f2f-817f-bce0000c0001");

    public ServiceRepositoryTests(TestDbFixture testDbFixture)
    {
        var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization { ConfigureMembers = true });
        fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => fixture.Behaviors.Remove(b));

        fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        _dbTestDbFixture = testDbFixture;
    }

    #region GetServiceDetailByIdUntrackedAsync

    [Fact]
    public async Task GetServiceDetailByIdUntrackedAsync_WithNotExistingService_ReturnsDefault()
    {
        // Arrange
        var (sut, _) = await CreateSut();

        // Act
        var results = await sut.GetOfferDetailByIdUntrackedAsync(Guid.NewGuid(), "en", new("2dc4249f-b5ca-4d42-bef1-7a7a950a4f88"), OfferTypeId.SERVICE);

        // Assert
        (results == default).Should().BeTrue();
    }

    [Fact]
    public async Task GetServiceDetailByIdUntrackedAsync_ReturnsServiceDetailData()
    {
        // Arrange
        var (sut, _) = await CreateSut();

        // Act
        var result = await sut.GetOfferDetailByIdUntrackedAsync(_offerId, "en", new("2dc4249f-b5ca-4d42-bef1-7a7a950a4f88"), OfferTypeId.SERVICE);

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be("Consulting Service - Data Readiness");
        result.ContactEmail.Should().BeNull();
        result.Provider.Should<string>().Be("CX-Operator");
    }

    #endregion

    #region GetOfferProviderDetailsAsync

    [Fact]
    public async Task GetOfferProviderDetailsAsync_WithExistingOffer_ReturnsOfferProviderDetails()
    {
        // Arrange
        var (sut, _) = await CreateSut();

        // Act
        var result = await sut.GetOfferProviderDetailsAsync(_offerId, OfferTypeId.SERVICE);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetOfferProviderDetailsAsync_WithNotExistingOffer_ReturnsNull()
    {
        // Arrange
        var (sut, _) = await CreateSut();

        // Act
        var result = await sut.GetOfferProviderDetailsAsync(Guid.NewGuid(), OfferTypeId.SERVICE);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    private async Task<(OfferRepository, PortalDbContext)> CreateSut()
    {
        var context = await _dbTestDbFixture.GetPortalDbContext();
        var sut = new OfferRepository(context);
        return (sut, context);
    }
}

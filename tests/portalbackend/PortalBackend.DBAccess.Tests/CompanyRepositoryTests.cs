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

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Org.Eclipse.TractusX.Portal.Backend.Framework.Models;
using Org.Eclipse.TractusX.Portal.Backend.PortalBackend.DBAccess.Models;
using Org.Eclipse.TractusX.Portal.Backend.PortalBackend.DBAccess.Repositories;
using Org.Eclipse.TractusX.Portal.Backend.PortalBackend.DBAccess.Tests.Setup;
using Org.Eclipse.TractusX.Portal.Backend.PortalBackend.PortalEntities;
using Org.Eclipse.TractusX.Portal.Backend.PortalBackend.PortalEntities.Entities;
using Org.Eclipse.TractusX.Portal.Backend.PortalBackend.PortalEntities.Enums;
using System.Collections.Immutable;
using Xunit.Extensions.AssemblyFixture;

namespace Org.Eclipse.TractusX.Portal.Backend.PortalBackend.DBAccess.Tests;

/// <summary>
/// Tests the functionality of the <see cref="CompanyRepository"/>
/// </summary>
public class CompanyRepositoryTests : IAssemblyFixture<TestDbFixture>
{
    private readonly TestDbFixture _dbTestDbFixture;
    private readonly Guid _validCompanyId = new("2dc4249f-b5ca-4d42-bef1-7a7a950a4f87");
    private readonly Guid _validOspCompanyId = new("a45f3b2e-29f7-4b52-8bb2-15b204849d87");

    public CompanyRepositoryTests(TestDbFixture testDbFixture)
    {
        var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization { ConfigureMembers = true });
        fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => fixture.Behaviors.Remove(b));

        fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        _dbTestDbFixture = testDbFixture;
    }

    #region Create ServiceProviderCompanyDetail

    [Fact]
    public async Task CreateServiceProviderCompanyDetail_ReturnsExpectedResult()
    {
        // Arrange
        const string url = "https://service-url.com";
        var autoSetupCallbackUrl = "https://test.de";
        var (sut, context) = await CreateSut();
        const string authUrl = "https://auth-url.com";
        const string clientId = "client-id";
        var secret = Convert.FromHexString("2b7e151628aed2a6abf715892b7e151628aed2a6abf715892b7e151628aed2a6");
        var initializationVector = Convert.FromBase64String("JHcycHPDfRwjT1J1NqBJtQ==");
        var encryptionMode = 5;

        // Act
        var results = sut.CreateProviderCompanyDetail(_validCompanyId, new ProviderDetailsCreationData(url, authUrl, clientId, secret, encryptionMode), entity =>
        {
            entity.AutoSetupCallbackUrl = autoSetupCallbackUrl;
            entity.InitializationVector = initializationVector;
        });

        // Assert
        var changeTracker = context.ChangeTracker;
        results.CompanyId.Should().Be(_validCompanyId);
        results.AutoSetupUrl.Should().Be(url);
        results.AutoSetupCallbackUrl.Should().Be(autoSetupCallbackUrl);
        changeTracker.HasChanges().Should().BeTrue();
        changeTracker.Entries().ToList()
            .Should().ContainSingle()
            .Which.Entity.Should().BeOfType<ProviderCompanyDetail>()
            .Which.Should().Match<ProviderCompanyDetail>(x =>
                x.AutoSetupUrl == url &&
                x.AutoSetupCallbackUrl == autoSetupCallbackUrl &&
                x.AuthUrl == authUrl &&
                x.ClientId == clientId &&
                x.ClientSecret == secret &&
                x.InitializationVector == initializationVector &&
                x.EncryptionMode == encryptionMode
            );
    }

    #endregion

    #region Create Company

    [Fact]
    public async Task CreateCompany_ReturnsExpectedResult()
    {
        // Arrange
        var (sut, context) = await CreateSut();

        // Act
        var results = sut.CreateCompany("Test Company", entity =>
        {
            entity.CompanyStatusId = CompanyStatusId.ACTIVE;
        });

        // Assert
        var changeTracker = context.ChangeTracker;
        results.Name.Should().Be("Test Company");
        results.CompanyStatusId.Should().Be(CompanyStatusId.ACTIVE);
        changeTracker.HasChanges().Should().BeTrue();
        changeTracker.Entries().ToList()
            .Should().ContainSingle()
            .Which.Entity.Should().BeOfType<Company>()
            .Which.Should().Match<Company>(x =>
                x.Name == "Test Company" &&
                x.CompanyStatusId == CompanyStatusId.ACTIVE
            );
    }

    #endregion

    #region Create Address

    [Fact]
    public async Task CreateAddress_ReturnsExpectedResult()
    {
        // Arrange
        var (sut, context) = await CreateSut();

        // Act
        var results = sut.CreateAddress("Munich", "Street", "BY", "DE", a =>
        {
            a.Streetnumber = "5";
        });

        // Assert
        var changeTracker = context.ChangeTracker;
        results.Streetnumber.Should().Be("5");
        results.City.Should().Be("Munich");
        changeTracker.HasChanges().Should().BeTrue();
        changeTracker.Entries().ToList()
            .Should().ContainSingle()
            .Which.Entity.Should().BeOfType<Address>()
            .Which.Should().Match<Address>(x =>
                x.City == "Munich" &&
                x.Streetname == "Street" &&
                x.Region == "BY" &&
                x.CountryAlpha2Code == "DE" &&
                x.Streetnumber == "5"
            );
    }

    #endregion

    #region Create ServiceProviderCompanyDetail

    [Fact]
    public async Task GetServiceProviderCompanyDetailAsync_WithExistingUser_ReturnsExpectedResult()
    {
        // Arrange
        var (sut, _) = await CreateSut();

        // Act
        var result = await sut.GetProviderCompanyDetailAsync([CompanyRoleId.SERVICE_PROVIDER, CompanyRoleId.APP_PROVIDER], new Guid("3390c2d7-75c1-4169-aa27-6ce00e1f3cdd"));

        // Assert
        result.Should().NotBe(default);
        result.ProviderDetailReturnData.Should().NotBeNull();
        result.ProviderDetailReturnData.CompanyId.Should().Be(new Guid("3390c2d7-75c1-4169-aa27-6ce00e1f3cdd"));
        result.IsProviderCompany.Should().BeTrue();
    }

    [Fact]
    public async Task GetServiceProviderCompanyDetailAsync_WithNotExistingDetails_ReturnsDefault()
    {
        // Arrange
        var (sut, _) = await CreateSut();

        // Act
        var result = await sut.GetProviderCompanyDetailAsync([CompanyRoleId.SERVICE_PROVIDER, CompanyRoleId.APP_PROVIDER], Guid.NewGuid());

        // Assert
        result.Should().Be(default);
    }

    [Fact]
    public async Task GetServiceProviderCompanyDetailAsync_WithExistingUserAndNotProvider_ReturnsIsCompanyUserFalse()
    {
        // Arrange
        var (sut, _) = await CreateSut();

        // Act
        var result = await sut.GetProviderCompanyDetailAsync([CompanyRoleId.SERVICE_PROVIDER, CompanyRoleId.APP_PROVIDER], new("41fd2ab8-71cd-4546-9bef-a388d91b2542"));

        // Assert
        result.Should().NotBe(default);
        result.IsProviderCompany.Should().BeFalse();
    }

    #endregion

    #region IsValidCompanyRoleOwner

    [Theory]
    [InlineData("3390c2d7-75c1-4169-aa27-6ce00e1f3cdd", new[] { CompanyRoleId.SERVICE_PROVIDER }, true, true)]
    [InlineData("3390c2d7-75c1-4169-aa27-6ce00e1f3cdd", new[] { CompanyRoleId.SERVICE_PROVIDER, CompanyRoleId.OPERATOR }, true, true)]
    [InlineData("3390c2d7-75c1-4169-aa27-6ce00e1f3cdd", new[] { CompanyRoleId.OPERATOR }, true, false)]
    [InlineData("deadbeef-dead-beef-dead-beefdeadbeef", new[] { CompanyRoleId.SERVICE_PROVIDER }, false, false)]
    public async Task IsValidCompanyRoleOwner_ReturnsExpected(Guid companyId, IEnumerable<CompanyRoleId> companyRoleIds, bool isValidCompany, bool isCompanyRoleOwner)
    {
        // Arrange
        var (sut, _) = await CreateSut();

        // Act
        var results = await sut.IsValidCompanyRoleOwner(companyId, companyRoleIds);

        // Assert
        results.IsValidCompanyId.Should().Be(isValidCompany);
        results.IsCompanyRoleOwner.Should().Be(isCompanyRoleOwner);
    }

    #endregion

    #region GetCompanyBpnAndSelfDescriptionDocumentByIdAsync

    [Fact]
    public async Task GetCompanyBpnByIdAsync_WithValidData_ReturnsExpected()
    {
        // Arrange
        var (sut, _) = await CreateSut();

        // Act
        var results = await sut.GetCompanyBpnAndSelfDescriptionDocumentByIdAsync(new Guid("2dc4249f-b5ca-4d42-bef1-7a7a950a4f87"));

        // Assert
        results.Should().NotBe(default);
        results.Bpn.Should().NotBeNullOrEmpty();
        results.Bpn.Should().Be("BPNL00000003CRHK");
    }

    [Fact]
    public async Task GetCompanyBpnByIdAsync_WithNotExistingId_ReturnsEmpty()
    {
        // Arrange
        var (sut, _) = await CreateSut();

        // Act
        var results = await sut.GetCompanyBpnAndSelfDescriptionDocumentByIdAsync(Guid.NewGuid());

        // Assert
        results.Should().Be(default);
    }

    #endregion

    #region AttachAndModifyServiceProviderDetails

    [Fact]
    public async Task AttachAndModifyServiceProviderDetails_Changed_ReturnsExpectedResult()
    {
        // Arrange
        const string url = "https://service-url.com/new";
        var (sut, context) = await CreateSut();

        // Act
        sut.AttachAndModifyProviderCompanyDetails(new Guid("ee8b4b4a-056e-4f0b-bc2a-cc1adbedf122"),
            detail => { detail.AutoSetupUrl = null!; },
            detail => { detail.AutoSetupUrl = url; });

        // Assert
        var changeTracker = context.ChangeTracker;
        var changedEntries = changeTracker.Entries().ToList();
        changeTracker.HasChanges().Should().BeTrue();
        changedEntries.Should().NotBeEmpty();
        changedEntries.Should().HaveCount(1);
        changedEntries.Single().Entity.Should().BeOfType<ProviderCompanyDetail>().Which.AutoSetupUrl.Should().Be(url);
        var entry = changedEntries.Single();
        entry.Entity.Should().BeOfType<ProviderCompanyDetail>().Which.AutoSetupUrl.Should().Be(url);
        entry.State.Should().Be(EntityState.Modified);
    }

    [Fact]
    public async Task AttachAndModifyServiceProviderDetails_Unchanged_ReturnsExpectedResult()
    {
        // Arrange
        const string url = "https://service-url.com/new";
        var (sut, context) = await CreateSut();

        // Act
        sut.AttachAndModifyProviderCompanyDetails(new Guid("ee8b4b4a-056e-4f0b-bc2a-cc1adbedf122"),
            detail => { detail.AutoSetupUrl = url; },
            detail => { detail.AutoSetupUrl = url; });

        // Assert
        var changeTracker = context.ChangeTracker;
        var changedEntries = changeTracker.Entries().ToList();
        changeTracker.HasChanges().Should().BeFalse();
        changedEntries.Should().NotBeEmpty();
        changedEntries.Should().HaveCount(1);
        var entry = changedEntries.Single();
        entry.Entity.Should().BeOfType<ProviderCompanyDetail>().Which.AutoSetupUrl.Should().Be(url);
        entry.State.Should().Be(EntityState.Unchanged);
    }

    #endregion

    #region AttachAndModifyAddress

    [Fact]
    public async Task AttachAndModifyAddress_Changed_ReturnsExpectedResult()
    {
        // Arrange
        const string city = "Munich";
        var (sut, context) = await CreateSut();

        // Act
        sut.AttachAndModifyAddress(new Guid("b4db3945-19a7-4a50-97d6-e66e8dfd04fb"),
            address => { address.City = null!; },
            address => { address.City = city; });

        // Assert
        var changeTracker = context.ChangeTracker;
        var changedEntries = changeTracker.Entries().ToList();
        changeTracker.HasChanges().Should().BeTrue();
        changedEntries.Should().NotBeEmpty();
        changedEntries.Should().HaveCount(1);
        changedEntries.Single().Entity.Should().BeOfType<Address>().Which.City.Should().Be(city);
        var entry = changedEntries.Single();
        entry.Entity.Should().BeOfType<Address>().Which.City.Should().Be(city);
        entry.State.Should().Be(EntityState.Modified);
    }

    [Fact]
    public async Task AttachAndModifyAddress_Unchanged_ReturnsExpectedResult()
    {
        // Arrange
        const string city = "Munich";
        var (sut, context) = await CreateSut();

        // Act
        sut.AttachAndModifyAddress(new Guid("b4db3945-19a7-4a50-97d6-e66e8dfd04fb"),
            address => { address.City = city; },
            address => { address.City = city; });

        // Assert
        var changeTracker = context.ChangeTracker;
        var changedEntries = changeTracker.Entries().ToList();
        changeTracker.HasChanges().Should().BeFalse();
        changedEntries.Should().NotBeEmpty();
        changedEntries.Should().HaveCount(1);
        var entry = changedEntries.Single();
        entry.Entity.Should().BeOfType<Address>().Which.City.Should().Be(city);
        entry.State.Should().Be(EntityState.Unchanged);
    }

    #endregion

    #region AttachAndModifyServiceProviderDetails

    [Fact]
    public async Task CheckServiceProviderDetailsExistsForUser_WithValidIamUser_ReturnsDetailId()
    {
        // Arrange
        var (sut, _) = await CreateSut();

        // Act
        var result = await sut.GetProviderCompanyDetailsExistsForUser(new("3390c2d7-75c1-4169-aa27-6ce00e1f3cdd"));

        // Assert
        result.Should().NotBe(default);
    }

    [Fact]
    public async Task CheckServiceProviderDetailsExistsForUser_WithNotExistingIamUser_ReturnsEmpty()
    {
        // Arrange
        var (sut, _) = await CreateSut();

        // Act
        var result = await sut.GetProviderCompanyDetailsExistsForUser(Guid.NewGuid());

        // Assert
        result.Should().Be(default);
    }

    #endregion

    #region CreateUpdateDeleteIdentifiers

    [Theory]
    [InlineData(
        new[] { UniqueIdentifierId.COMMERCIAL_REG_NUMBER, UniqueIdentifierId.EORI, UniqueIdentifierId.LEI_CODE, UniqueIdentifierId.VAT_ID }, // initialKeys
        new[] { UniqueIdentifierId.COMMERCIAL_REG_NUMBER, UniqueIdentifierId.EORI, UniqueIdentifierId.LEI_CODE, UniqueIdentifierId.VIES },   // updateKeys
        new[] { "value-1", "value-2", "value-3", "value-4" },                                                                                // initialValues
        new[] { "value-1", "changed-1", "changed-2", "added-1" },                                                                            // updateValues
        new[] { UniqueIdentifierId.VIES },                                                                                                   // addedEntityKeys
        new[] { "added-1" },                                                                                                                 // addedEntityValues
        new[] { UniqueIdentifierId.EORI, UniqueIdentifierId.LEI_CODE },                                                                      // updatedEntityKeys
        new[] { "changed-1", "changed-2" },                                                                                                  // updatedEntityValues
        new[] { UniqueIdentifierId.VAT_ID }                                                                                                  // removedEntityKeys
    )]
    [InlineData(
        new[] { UniqueIdentifierId.EORI, UniqueIdentifierId.LEI_CODE, UniqueIdentifierId.VAT_ID },                                           // initialKeys
        new[] { UniqueIdentifierId.COMMERCIAL_REG_NUMBER, UniqueIdentifierId.EORI, UniqueIdentifierId.VIES },                                // updateKeys
        new[] { "value-1", "value-2", "value-3" },                                                                                           // initialValues
        new[] { "added-1", "changed-1", "added-2" },                                                                                         // updateValues
        new[] { UniqueIdentifierId.COMMERCIAL_REG_NUMBER, UniqueIdentifierId.VIES },                                                         // addedEntityKeys
        new[] { "added-1", "added-2" },                                                                                                      // addedEntityValues
        new[] { UniqueIdentifierId.EORI },                                                                                                   // updatedEntityKeys
        new[] { "changed-1" },                                                                                                               // updatedEntityValues
        new[] { UniqueIdentifierId.LEI_CODE, UniqueIdentifierId.VAT_ID }                                                                     // removedEntityKeys
    )]

    public async Task CreateUpdateDeleteIdentifiers(
        IEnumerable<UniqueIdentifierId> initialKeys, IEnumerable<UniqueIdentifierId> updateKeys,
        IEnumerable<string> initialValues, IEnumerable<string> updateValues,
        IEnumerable<UniqueIdentifierId> addedEntityKeys, IEnumerable<string> addedEntityValues,
        IEnumerable<UniqueIdentifierId> updatedEntityKeys, IEnumerable<string> updatedEntityValues,
        IEnumerable<UniqueIdentifierId> removedEntityKeys)
    {
        var companyId = Guid.NewGuid();
        var initialItems = initialKeys.Zip(initialValues).Select(x => (InitialKey: x.First, InitialValue: x.Second)).ToImmutableArray();
        var updateItems = updateKeys.Zip(updateValues).Select(x => (UpdateKey: x.First, UpdateValue: x.Second)).ToImmutableArray();
        var addedEntities = addedEntityKeys.Zip(addedEntityValues).Select(x => new CompanyIdentifier(companyId, x.First, x.Second)).OrderBy(x => x.UniqueIdentifierId).ToImmutableArray();
        var updatedEntities = updatedEntityKeys.Zip(updatedEntityValues).Select(x => new CompanyIdentifier(companyId, x.First, x.Second)).OrderBy(x => x.UniqueIdentifierId).ToImmutableArray();
        var removedEntities = removedEntityKeys.Select(x => new CompanyIdentifier(companyId, x, null!)).OrderBy(x => x.UniqueIdentifierId).ToImmutableArray();

        var (sut, context) = await CreateSut();

        sut.CreateUpdateDeleteIdentifiers(companyId, initialItems, updateItems);

        var changeTracker = context.ChangeTracker;
        var changedEntries = changeTracker.Entries().ToList();
        changeTracker.HasChanges().Should().BeTrue();
        changedEntries.Should().AllSatisfy(entry => entry.Entity.Should().BeOfType<CompanyIdentifier>());
        changedEntries.Should().HaveCount(addedEntities.Length + updatedEntities.Length + removedEntities.Length);
        var added = changedEntries.Where(entry => entry.State == EntityState.Added).Select(x => (CompanyIdentifier)x.Entity).ToImmutableArray();
        var modified = changedEntries.Where(entry => entry.State == EntityState.Modified).Select(x => (CompanyIdentifier)x.Entity).ToImmutableArray();
        var deleted = changedEntries.Where(entry => entry.State == EntityState.Deleted).Select(x => (CompanyIdentifier)x.Entity).ToImmutableArray();

        added.Should().HaveSameCount(addedEntities);
        added.OrderBy(x => x.UniqueIdentifierId).Zip(addedEntities).Should().AllSatisfy(x => (x.First.UniqueIdentifierId == x.Second.UniqueIdentifierId && x.First.Value == x.Second.Value).Should().BeTrue());
        modified.Should().HaveSameCount(updatedEntities);
        modified.OrderBy(x => x.UniqueIdentifierId).Zip(updatedEntities).Should().AllSatisfy(x => (x.First.UniqueIdentifierId == x.Second.UniqueIdentifierId && x.First.Value == x.Second.Value).Should().BeTrue());
        deleted.Should().HaveSameCount(removedEntities);
        deleted.OrderBy(x => x.UniqueIdentifierId).Zip(removedEntities).Should().AllSatisfy(x => x.First.UniqueIdentifierId.Should().Be(x.Second.UniqueIdentifierId));
    }

    #endregion

    #region GetCompanyDetailsAsync

    [Fact]
    public async Task GetCompanyDetailsAsync_ReturnsExpected()
    {
        var (sut, _) = await CreateSut();

        var result = await sut.GetCompanyDetailsAsync(new("2dc4249f-b5ca-4d42-bef1-7a7a950a4f87"));

        result.Should().NotBeNull();

        result!.CompanyId.Should().Be(new Guid("2dc4249f-b5ca-4d42-bef1-7a7a950a4f87"));
        result.Name.Should().Be("CX-Operator");
        result.ShortName.Should().Be("CX-Operator");
        result.BusinessPartnerNumber.Should().Be("BPNL00000003CRHK");
        result.CountryAlpha2Code.Should().Be("DE");
        result.City.Should().Be("tbd");
        result.StreetName.Should().Be("tbd");
        result.StreetNumber.Should().Be("");
        result.ZipCode.Should().Be("");
        result.CompanyRole.Should().Contain(CompanyRoleId.ACTIVE_PARTICIPANT);
    }

    #endregion

    #region CompanyAssignedUseCaseId

    [Fact]
    public async Task GetCompanyAssigendUseCaseDetailsAsync_ReturnsExpected()
    {
        // Arrange
        var (sut, _) = await CreateSut();

        // Act
        var result = sut.GetCompanyAssigendUseCaseDetailsAsync(new("0dcd8209-85e2-4073-b130-ac094fb47106"));

        // Assert
        result.Should().NotBeNull();
        var data = await result.FirstAsync();
        data!.useCaseId.Should().Be("06b243a4-ba51-4bf3-bc40-5d79a2231b86");
        data.name.Should().Be("Traceability");
    }

    [Fact]
    public async Task GetCompanyStatusAndUseCaseIdAsync_ReturnsExpected()
    {
        // Arrange
        var useCaseId = new Guid("06b243a4-ba51-4bf3-bc40-5d79a2231b86");
        var (sut, _) = await CreateSut();

        // Act
        var result = await sut.GetCompanyStatusAndUseCaseIdAsync(new("0dcd8209-85e2-4073-b130-ac094fb47106"), useCaseId);

        // Assert
        result.Should().NotBeNull();
        result.IsActiveCompanyStatus.Should().BeFalse();
        result.IsUseCaseIdExists.Should().BeTrue();
        result.IsValidCompany.Should().BeTrue();
    }

    [Fact]
    public async Task CreateCompanyAssignedUseCase_ReturnsExpectedResult()
    {
        // Arrange
        var useCaseId = new Guid("1aacde78-35ec-4df3-ba1e-f988cddcbbd8");
        var companyId = new Guid("0dcd8209-85e2-4073-b130-ac094fb47106");
        var (sut, context) = await CreateSut();

        // Act
        sut.CreateCompanyAssignedUseCase(companyId, useCaseId);

        // Assert
        var changeTracker = context.ChangeTracker;
        var changedEntries = changeTracker.Entries().ToList();
        changeTracker.HasChanges().Should().BeTrue();
        changedEntries.Should().NotBeEmpty();
        changedEntries.Should().HaveCount(1);
        changedEntries.Should().AllSatisfy(entry => entry.State.Should().Be(EntityState.Added));
        changedEntries.Select(x => x.Entity).Should().AllBeOfType<CompanyAssignedUseCase>();
        changedEntries.Select(x => x.Entity).Cast<CompanyAssignedUseCase>().Select(x => (x.CompanyId, x.UseCaseId))
            .Should().Contain((companyId, useCaseId));
    }

    [Fact]
    public async Task RemoveCompanyAssignedUseCase_ReturnsExpectedResult()
    {
        // Arrange
        var useCaseId = new Guid("1aacde78-35ec-4df3-ba1e-f988cddcbbd8");
        var companyId = new Guid("0dcd8209-85e2-4073-b130-ac094fb47106");
        var (sut, context) = await CreateSut();

        // Act
        sut.RemoveCompanyAssignedUseCase(companyId, useCaseId);

        // Assert
        var changeTracker = context.ChangeTracker;
        var changedEntries = changeTracker.Entries().ToList();
        changeTracker.HasChanges().Should().BeTrue();
        changedEntries.Should().NotBeEmpty();
        changedEntries.Should().HaveCount(1);
        changedEntries.Select(x => x.Entity).Should().AllBeOfType<CompanyAssignedUseCase>();
        var deletedEntities = changedEntries.Where(x => x.State == EntityState.Deleted).Select(x => (CompanyAssignedUseCase)x.Entity);
        deletedEntities.Should().HaveCount(1);
        deletedEntities.Select(x => (x.CompanyId, x.UseCaseId)).Should().Contain((companyId, useCaseId));
    }

    #endregion

    #region GetCompanyRoleAndConsentAgreement

    [Fact]
    public async Task GetCompanyRoleAndConsentAgreementDetailsAsync_ReturnsExpected()
    {
        var (sut, _) = await CreateSut();
        var companyId = new Guid("3390c2d7-75c1-4169-aa27-6ce00e1f3cdd");
        var activeDescription = "The participant role is covering the data provider, data consumer or app user scenario. As participant you are an active member of the network with enabled services to participate as contributor and user.";
        var serviceDscription = "The Service Provider is able to offer 3rd party services, such as dataspace service offerings to Catena-X Members. Catena-X members can subscribe for those services.";
        var appDescription = "The App Provider is a company which is providing application software via the CX marketplace. As app provider you can participate and use the developer hub, release and offer applications to the network and manage your applications.";
        var onboardingServiceProviderDescription = "The Onboarding service provider is a Catena-X certified role which enables the company to act as onboarding provider inside the network.";

        var result = await sut.GetCompanyRoleAndConsentAgreementDataAsync(companyId, Constants.DefaultLanguage).ToListAsync();

        result.Should().NotBeNull()
            .And.HaveCount(4)
            .And.Satisfy(
                x => x.CompanyRoleId == CompanyRoleId.ACTIVE_PARTICIPANT && x.RoleDescription == activeDescription && !x.CompanyRolesActive && x.Agreements.Count() == 2 && x.Agreements.All(agreement => agreement.ConsentStatus == 0),
                x => x.CompanyRoleId == CompanyRoleId.APP_PROVIDER && x.RoleDescription == appDescription && !x.CompanyRolesActive && x.Agreements.Count() == 1 && x.Agreements.All(agreement => agreement.ConsentStatus == 0),
                x => x.CompanyRoleId == CompanyRoleId.SERVICE_PROVIDER && x.RoleDescription == serviceDscription && x.CompanyRolesActive && x.Agreements.Count() == 3
                     && x.Agreements.Any(agr => agr.AgreementId == new Guid("aa0a0000-7fbc-1f2f-817f-bce0502c1094") && agr.DocumentId == null && agr.AgreementName == "Terms & Conditions - Consultant" && agr.ConsentStatus == ConsentStatusId.ACTIVE && agr.Mandatory)
                     && x.Agreements.Any(agr => agr.AgreementId == new Guid("aa0a0000-7fbc-1f2f-817f-bce0502c1018") && agr.DocumentId == null && agr.AgreementName == "Data Sharing Approval - allow CX to submit company data (company name, requester) to process the subscription" && agr.ConsentStatus == 0 && agr.Mandatory)
                     && x.Agreements.Any(agr => agr.AgreementId == new Guid("aa0a0000-7fbc-1f2f-817f-bce0502c1017") && agr.DocumentId == new Guid("00000000-0000-0000-0000-000000000004") && agr.AgreementName == "Terms & Conditions Service Provider" && agr.ConsentStatus == 0 && agr.Mandatory),
                x => x.CompanyRoleId == CompanyRoleId.ONBOARDING_SERVICE_PROVIDER && x.RoleDescription == onboardingServiceProviderDescription && !x.CompanyRolesActive && x.Agreements.Count() == 1 && x.Agreements.All(agreement => agreement.ConsentStatus == 0 && agreement.Mandatory));
    }

    #endregion

    #region  GetCompanyRolesData

    [Fact]
    public async Task GetCompanyRolesDataAsync_ReturnsExpected()
    {
        // Arrange
        var companyRoleIds = new[] { CompanyRoleId.SERVICE_PROVIDER, CompanyRoleId.ACTIVE_PARTICIPANT };
        var (sut, _) = await CreateSut();

        // Act
        var result = await sut.GetCompanyRolesDataAsync(new("3390c2d7-75c1-4169-aa27-6ce00e1f3cdd"), companyRoleIds);

        // Assert
        result.IsValidCompany.Should().BeTrue();
        result.IsCompanyActive.Should().BeTrue();
        result.CompanyRoleIds.Should().NotBeNull()
            .And.Contain(CompanyRoleId.SERVICE_PROVIDER);
        result.ConsentStatusDetails.Should().NotBeNull()
            .And.Contain(x => x.ConsentStatusId == ConsentStatusId.ACTIVE);
    }

    #endregion

    #region GetAgreementAssignedRolesDataAsync

    [Fact]
    public async Task GetAgreementAssignedRolesDataAsync_ReturnsExpected()
    {
        // Arrange
        var companyRoleIds = new[] { CompanyRoleId.APP_PROVIDER };
        var (sut, _) = await CreateSut();

        // Act
        var result = await sut.GetAgreementAssignedRolesDataAsync(companyRoleIds).ToListAsync();

        // Assert
        result.Should().NotBeNull()
            .And.HaveCount(1)
            .And.Satisfy(
                x => x.agreementStatusData.AgreementId == new Guid("aa0a0000-7fbc-1f2f-817f-bce0502c1011")
                    && x.agreementStatusData.AgreementStatusId == AgreementStatusId.ACTIVE
                    && x.CompanyRoleId == CompanyRoleId.APP_PROVIDER
            );
    }

    [Fact]
    public async Task GetAgreementAssignedRolesDataAsync_ReturnsExpectedOrder()
    {
        // Arrange
        var companyRoleIds = new[] { CompanyRoleId.APP_PROVIDER, CompanyRoleId.SERVICE_PROVIDER, CompanyRoleId.ACTIVE_PARTICIPANT };
        var (sut, _) = await CreateSut();

        // Act
        var result = await sut.GetAgreementAssignedRolesDataAsync(companyRoleIds).ToListAsync();

        // Assert
        result.Should().NotBeNull()
            .And.HaveCount(7)
            .And.Satisfy(
                x => x.agreementStatusData.AgreementId == new Guid("aa0a0000-7fbc-1f2f-817f-bce0502c1090")
                    && x.agreementStatusData.AgreementStatusId == AgreementStatusId.INACTIVE
                    && x.CompanyRoleId == CompanyRoleId.ACTIVE_PARTICIPANT,
                x => x.agreementStatusData.AgreementId == new Guid("aa0a0000-7fbc-1f2f-817f-bce0502c1010")
                    && x.agreementStatusData.AgreementStatusId == AgreementStatusId.ACTIVE
                    && x.CompanyRoleId == CompanyRoleId.ACTIVE_PARTICIPANT,
                x => x.agreementStatusData.AgreementId == new Guid("aa0a0000-7fbc-1f2f-817f-bce0502c1013")
                    && x.agreementStatusData.AgreementStatusId == AgreementStatusId.ACTIVE
                    && x.CompanyRoleId == CompanyRoleId.ACTIVE_PARTICIPANT,
                x => x.agreementStatusData.AgreementId == new Guid("aa0a0000-7fbc-1f2f-817f-bce0502c1011")
                     && x.agreementStatusData.AgreementStatusId == AgreementStatusId.ACTIVE
                     && x.CompanyRoleId == CompanyRoleId.APP_PROVIDER,
                x => x.agreementStatusData.AgreementId == new Guid("aa0a0000-7fbc-1f2f-817f-bce0502c1018")
                     && x.agreementStatusData.AgreementStatusId == AgreementStatusId.ACTIVE
                     && x.CompanyRoleId == CompanyRoleId.SERVICE_PROVIDER,
                x => x.agreementStatusData.AgreementId == new Guid("aa0a0000-7fbc-1f2f-817f-bce0502c1017")
                    && x.agreementStatusData.AgreementStatusId == AgreementStatusId.ACTIVE
                    && x.CompanyRoleId == CompanyRoleId.SERVICE_PROVIDER,
                x => x.agreementStatusData.AgreementId == new Guid("aa0a0000-7fbc-1f2f-817f-bce0502c1094")
                     && x.agreementStatusData.AgreementStatusId == AgreementStatusId.ACTIVE
                     && x.CompanyRoleId == CompanyRoleId.SERVICE_PROVIDER
            );
        result.Select(x => x.CompanyRoleId).Should().BeInAscendingOrder();
    }

    [Fact]
    public async Task IsCompanyStatusActive()
    {
        // Arrange
        var (sut, _) = await CreateSut();

        // Act
        var result = await sut.GetCompanyStatusDataAsync(new("3390c2d7-75c1-4169-aa27-6ce00e1f3cdd"));

        // Assert
        result.IsValid.Should().BeTrue();
        result.IsActive.Should().BeTrue();
    }

    #endregion

    #region GetBpnAndTechnicalUserRoleIds

    [Fact]
    public async Task GetCompanyIdAndBpnForIamUserUntrackedAsync_WithValidData_ReturnsExpected()
    {
        // Arrange
        var (sut, _) = await CreateSut();

        // Act
        var result = await sut.GetBpnAndTechnicalUserRoleIds(new Guid("2dc4249f-b5ca-4d42-bef1-7a7a950a4f87"), "technical_roles_management");

        // Assert
        result.Should().NotBe(default);
        result.Bpn.Should().Be("BPNL00000003CRHK");
        result.TechnicalUserRoleIds.Should().HaveCount(20).And.OnlyHaveUniqueItems();
    }

    #endregion

    #region GetOwnCompanAndCompanyUseryIdWithCompanyNameAndUserEmailAsync

    [Fact]
    public async Task GetOwnCompanAndCompanyUseryIdWithCompanyNameAndUserEmailAsync_WithValidIamUser_ReturnsExpectedResult()
    {
        // Arrange
        var (sut, _) = await CreateSut();

        // Act
        var result = await sut.GetOwnCompanyInformationAsync(new("2dc4249f-b5ca-4d42-bef1-7a7a950a4f87"), new Guid("cd436931-8399-4c1d-bd81-7dffb298c7ca"));

        // Assert
        result.Should().NotBeNull();
        result!.OrganizationName.Should().Be("CX-Operator");
        result.CompanyUserEmail.Should().Be("inactive-user@mail.com");
    }

    [Fact]
    public async Task GetOwnCompanAndCompanyUseryIdWithCompanyNameAndUserEmailAsync_WithNotExistingIamUser_ReturnsDefault()
    {
        // Arrange
        var (sut, _) = await CreateSut();

        // Act
        var result = await sut.GetOwnCompanyInformationAsync(Guid.NewGuid(), Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetOperatorBpns

    [Fact]
    public async Task GetOperatorBpns_ReturnsExpectedResult()
    {
        // Arrange
        var (sut, _) = await CreateSut();

        // Act
        var result = await sut.GetOperatorBpns().ToListAsync();

        // Assert
        result.Should().ContainSingle()
            .Which.Should().BeOfType<OperatorBpnData>()
            .And.Match<OperatorBpnData>(x => x.Bpn == "BPNL00000003CRHK" && x.OperatorName == "CX-Operator");
    }

    #endregion

    #region GetCallbackData

    [Fact]
    public async Task GetCallbackData_WithNotExistingOspData_ReturnsExpectedResult()
    {
        // Arrange
        var (sut, _) = await CreateSut();

        // Act
        var result = await sut.GetCallbackData(_validCompanyId);

        // Assert
        result.CallbackUrl.Should().Be(null);
    }

    [Fact]
    public async Task GetCallbackData_WithExistingOspData_ReturnsExpectedResult()
    {
        // Arrange
        const string url = "https://service-url.com";
        var (sut, _) = await CreateSut();

        // Act
        var result = await sut.GetCallbackData(_validOspCompanyId);

        // Assert
        result.CallbackUrl.Should().Be(url);
    }

    #endregion

    #region GetCallbackEditData

    [Theory]
    [InlineData(CompanyRoleId.ACTIVE_PARTICIPANT, true)]
    [InlineData(CompanyRoleId.ONBOARDING_SERVICE_PROVIDER, false)]
    public async Task GetCallbackEditData_WithNotExistingOspData_ReturnsExpectedResult(CompanyRoleId companyRoleId, bool hasRole)
    {
        // Arrange
        var (sut, _) = await CreateSut();

        // Act
        var result = await sut.GetCallbackEditData(_validCompanyId, companyRoleId);

        // Assert
        result.OnboardingServiceProviderDetailId.Should().BeNull();
        result.OspDetails.Should().BeNull();
        result.HasCompanyRole.Should().Be(hasRole);
    }

    [Fact]
    public async Task GetCallbackEditData_WithValid_ReturnsExpectedResult()
    {
        // Arrange
        const string url = "https://service-url.com";
        var (sut, _) = await CreateSut();

        // Act
        var result = await sut.GetCallbackEditData(_validOspCompanyId, CompanyRoleId.ONBOARDING_SERVICE_PROVIDER);

        // Assert
        result.OnboardingServiceProviderDetailId.Should().Be(new Guid("6e293a28-da95-432a-b10c-9cec44de09e9"));
        result.OspDetails.Should().NotBeNull()
            .And.Match<OspDetails>(x =>
                x.CallbackUrl == url &&
                x.EncryptionMode == 1 &&
                x.InitializationVector == null
            );
        result.HasCompanyRole.Should().BeTrue();
    }

    #endregion

    #region AttachAndModifyOnboardingServiceProvider

    [Fact]
    public async Task AttachAndModifyOnboardingServiceProvider_Changed_ReturnsExpectedResult()
    {
        // Arrange
        var onboardingServiceProviderDetailId = Guid.NewGuid();
        const string url = "https://service-url.com/new";
        var (sut, context) = await CreateSut();

        // Act
        sut.AttachAndModifyOnboardingServiceProvider(onboardingServiceProviderDetailId,
            detail => { detail.CallbackUrl = null!; },
            detail => { detail.CallbackUrl = url; });

        // Assert
        var changeTracker = context.ChangeTracker;
        var changedEntries = changeTracker.Entries().ToList();
        changeTracker.HasChanges().Should().BeTrue();
        changedEntries.Should().ContainSingle()
            .Which.Should().Match<EntityEntry>(x =>
                x.State == EntityState.Modified &&
                x.Entity.GetType() == typeof(OnboardingServiceProviderDetail) &&
                ((OnboardingServiceProviderDetail)x.Entity).CallbackUrl == url);
    }

    [Fact]
    public async Task AttachAndModifyOnboardingServiceProvider_Unchanged_ReturnsExpectedResult()
    {
        // Arrange
        var onboardingServiceProviderDetailId = Guid.NewGuid();
        const string url = "https://service-url.com/new";
        var (sut, context) = await CreateSut();

        // Act
        sut.AttachAndModifyOnboardingServiceProvider(onboardingServiceProviderDetailId,
            detail => { detail.CallbackUrl = url; },
            detail => { detail.CallbackUrl = url; });

        // Assert
        var changeTracker = context.ChangeTracker;
        var changedEntries = changeTracker.Entries().ToList();
        changeTracker.HasChanges().Should().BeFalse();
        changedEntries.Should().ContainSingle()
            .Which.Should().Match<EntityEntry>(x =>
                x.State == EntityState.Unchanged &&
                x.Entity.GetType() == typeof(OnboardingServiceProviderDetail)
            );
    }

    #endregion

    #region CreateOnboardingServiceProviderDetails

    [Theory]
    [InlineData("JHcycHPDfRwjT1J1NqBJtQ==")]
    [InlineData(null)]
    public async Task CreateOnboardingServiceProviderDetails_Changed_ReturnsExpectedResult(string? initVector)
    {
        // Arrange
        const string url = "https://service-url.com/new";
        const string authUrl = "https://auth.url";
        const string clientId = "acmeId";
        var (sut, context) = await CreateSut();
        var secret = Convert.FromHexString("2b7e151628aed2a6abf715892b7e151628aed2a6abf715892b7e151628aed2a6");
        var initializationVector = initVector == null ? null : Convert.FromBase64String(initVector);
        var index = 5;

        // Act
        var result = sut.CreateOnboardingServiceProviderDetails(_validCompanyId, url, authUrl, clientId, secret, initializationVector, index);

        // Assert
        result.CompanyId.Should().Be(_validCompanyId);
        result.CallbackUrl.Should().Be(url);
        var changeTracker = context.ChangeTracker;
        var changedEntries = changeTracker.Entries().ToList();
        changeTracker.HasChanges().Should().BeTrue();
        changedEntries.Should().ContainSingle()
            .Which.Should().Match<EntityEntry>(x =>
                x.State == EntityState.Added &&
                x.Entity.GetType() == typeof(OnboardingServiceProviderDetail) &&
                ((OnboardingServiceProviderDetail)x.Entity).CompanyId.Equals(_validCompanyId) &&
                ((OnboardingServiceProviderDetail)x.Entity).CallbackUrl.Equals(url) &&
                ((OnboardingServiceProviderDetail)x.Entity).AuthUrl.Equals(authUrl) &&
                ((OnboardingServiceProviderDetail)x.Entity).ClientId.Equals(clientId) &&
                ((OnboardingServiceProviderDetail)x.Entity).ClientSecret.SequenceEqual(secret) &&
                ((OnboardingServiceProviderDetail)x.Entity).InitializationVector == null
                    ? initializationVector == null
                    : ((OnboardingServiceProviderDetail)x.Entity).InitializationVector!.SequenceEqual(initializationVector!) &&
                ((OnboardingServiceProviderDetail)x.Entity).EncryptionMode == index);
    }

    #endregion

    #region CheckBpnExists

    [Fact]
    public async Task CheckBpnExists_WithNotExisting_ReturnsFalse()
    {
        // Arrange
        var (sut, _) = await CreateSut();

        // Act
        var result = await sut.CheckBpnExists("TESTNOTEXISTING");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CheckBpnExists_WithValid_ReturnsTrue()
    {
        // Arrange
        var (sut, _) = await CreateSut();

        // Act
        var result = await sut.CheckBpnExists("BPNL00000003LLHA");

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region GetAllMemberCompaniesBPN

    [Fact]
    public async Task GetAllMemberCompaniesBPN()
    {
        // Arrange
        var bpnIds = new[] {
            "BPNL07800HZ01643",
            "BPNL00000003AYRE",
            "BPNL00000003LLHA",
            "BPNL0000000001ON",
            "BPNL07800HZ01645" };
        var (sut, _) = await CreateSut();

        // Act
        var result = await sut.GetAllMemberCompaniesBPNAsync(bpnIds).ToListAsync();

        // Assert
        result.Should().NotBeNull().And.HaveCount(2).And.Satisfy(
            x => x == "BPNL07800HZ01643", x => x == "BPNL00000003AYRE");
    }

    [Fact]
    public async Task GetAllMemberCompaniesBPN_withNull_ReturnsExpected()
    {
        // Arrange
        var (sut, _) = await CreateSut();

        // Act
        var result = await sut.GetAllMemberCompaniesBPNAsync(null).ToListAsync();

        // Assert
        result.Should().NotBeNull().And.HaveCount(5).And.Satisfy(
            x => x == "BPNL07800HZ01643",
            x => x == "BPNL00000003AYRE",
            x => x == "BPNL00000003CRHK",
            x => x == "BPNL00000003CRHL",
            x => x == "BPNL00000001TEST");
    }

    #endregion

    #region GetWalletServiceUrl

    [Fact]
    public async Task GetWalletServiceUrl_ReturnsExpected()
    {
        // Arrange
        var (sut, _) = await CreateSut();

        // Act
        var result = await sut.GetDimServiceUrls(_validCompanyId);

        // Assert
        result.Bpn.Should().Be("BPNL00000003CRHK");
        result.Did.Should().Be("did:web:test");
        result.WalletUrl.Should().Be("https://example.org/auth");
    }

    #endregion

    #region RemoveProviderCompanyDetails

    [Fact]
    public async Task RemoveProviderCompanyDetails_ExecutesExpected()
    {
        // Arrange
        var (sut, context) = await CreateSut();

        // Act
        sut.RemoveProviderCompanyDetails(new Guid("7e86a0b8-6903-496b-96d1-0ef508206833"));

        // Assert
        var changeTracker = context.ChangeTracker;
        var changedEntries = changeTracker.Entries().ToList();
        changeTracker.HasChanges().Should().BeTrue();
        changedEntries.Should().NotBeEmpty();
        changedEntries.Should().HaveCount(1);
        var removedEntity = changedEntries.Single();
        removedEntity.State.Should().Be(EntityState.Deleted);
    }

    #endregion

    #region GetCompaniesWithMissingSdDocument

    [Fact]
    public async Task GetCompaniesWithMissingSdDocument_ReturnsExpectedCompanies()
    {
        // Arrange
        var (sut, _) = await CreateSut();

        // Act
        var result = await Pagination.CreateResponseAsync(
            0,
            10,
            15,
            sut.GetCompaniesWithMissingSdDocument());

        // Assert
        result.Should().NotBeNull();
        result.Content.Should().HaveCount(2).And.Satisfy(
            x => x.Name == "CX-Test-Access",
            x => x.Name == "Bayerische Motorenwerke AG");
    }

    #endregion

    #region IsExistingCompany

    [Fact]
    public async Task IsExistingCompany_WithExistingCompany_ReturnsTrue()
    {
        // Arrange
        var (sut, _) = await CreateSut();

        // Act
        var result = await sut.IsExistingCompany(_validCompanyId).ConfigureAwait(ConfigureAwaitOptions.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsExistingCompany_WithNotExistingCompany_ReturnsFalse()
    {
        // Arrange
        var (sut, _) = await CreateSut();

        // Act
        var result = await sut.IsExistingCompany(Guid.NewGuid()).ConfigureAwait(ConfigureAwaitOptions.None);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region Setup

    private async Task<(ICompanyRepository, PortalDbContext)> CreateSut()
    {
        var context = await _dbTestDbFixture.GetPortalDbContext();
        var sut = new CompanyRepository(context);
        return (sut, context);
    }

    #endregion
}

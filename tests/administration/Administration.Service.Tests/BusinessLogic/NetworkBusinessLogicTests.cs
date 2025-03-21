/********************************************************************************
 * Copyright (c) 2023 Contributors to the Eclipse Foundation
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
using Org.Eclipse.TractusX.Portal.Backend.Administration.Service.BusinessLogic;
using Org.Eclipse.TractusX.Portal.Backend.Administration.Service.DependencyInjection;
using Org.Eclipse.TractusX.Portal.Backend.Administration.Service.ErrorHandling;
using Org.Eclipse.TractusX.Portal.Backend.Administration.Service.Models;
using Org.Eclipse.TractusX.Portal.Backend.Framework.ErrorHandling;
using Org.Eclipse.TractusX.Portal.Backend.Framework.Identity;
using Org.Eclipse.TractusX.Portal.Backend.Framework.Models;
using Org.Eclipse.TractusX.Portal.Backend.Framework.Models.Configuration;
using Org.Eclipse.TractusX.Portal.Backend.Framework.Processes.Library.Concrete.Entities;
using Org.Eclipse.TractusX.Portal.Backend.Framework.Processes.Library.Enums;
using Org.Eclipse.TractusX.Portal.Backend.PortalBackend.DBAccess;
using Org.Eclipse.TractusX.Portal.Backend.PortalBackend.DBAccess.Models;
using Org.Eclipse.TractusX.Portal.Backend.PortalBackend.DBAccess.Repositories;
using Org.Eclipse.TractusX.Portal.Backend.PortalBackend.PortalEntities.Entities;
using Org.Eclipse.TractusX.Portal.Backend.PortalBackend.PortalEntities.Enums;
using Org.Eclipse.TractusX.Portal.Backend.Processes.NetworkRegistration.Library;
using Org.Eclipse.TractusX.Portal.Backend.Provisioning.Library.Models;
using Org.Eclipse.TractusX.Portal.Backend.Provisioning.Library.Service;
using Org.Eclipse.TractusX.Portal.Backend.Registration.Common;
using Org.Eclipse.TractusX.Portal.Backend.Registration.Common.ErrorHandling;
using Org.Eclipse.TractusX.Portal.Backend.Tests.Shared;
using Org.Eclipse.TractusX.Portal.Backend.Tests.Shared.Extensions;
using System.Collections.Immutable;

namespace Org.Eclipse.TractusX.Portal.Backend.Administration.Service.Tests.BusinessLogic;

public class NetworkBusinessLogicTests
{
    private const string Bpn = "BPNL00000001TEST";
    private const string VatId = "DE123456789";
    private static readonly string ExistingExternalId = Guid.NewGuid().ToString();
    private static readonly Guid CompanyId = new("95c4339e-e087-4cd2-a5b8-44d385e64630");
    private static readonly Guid UserRoleId = Guid.NewGuid();
    private static readonly Guid MultiIdpCompanyId = Guid.NewGuid();
    private static readonly Guid NoIdpCompanyId = Guid.NewGuid();
    private static readonly Guid IdpId = Guid.NewGuid();
    private static readonly Guid NoAliasIdpCompanyId = Guid.NewGuid();

    private readonly IFixture _fixture;

    private readonly IIdentityData _identity;
    private readonly IIdentityService _identityService;
    private readonly IUserProvisioningService _userProvisioningService;
    private readonly INetworkRegistrationProcessHelper _networkRegistrationProcessHelper;

    private readonly IPortalRepositories _portalRepositories;
    private readonly ICompanyRepository _companyRepository;
    private readonly ICompanyRolesRepository _companyRolesRepository;
    private readonly IPortalProcessStepRepository _processStepRepository;
    private readonly IApplicationRepository _applicationRepository;
    private readonly INetworkRepository _networkRepository;
    private readonly IIdentityProviderRepository _identityProviderRepository;
    private readonly ICountryRepository _countryRepository;
    private readonly NetworkBusinessLogic _sut;

    public NetworkBusinessLogicTests()
    {
        _fixture = new Fixture().Customize(new AutoFakeItEasyCustomization { ConfigureMembers = true });
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _userProvisioningService = A.Fake<IUserProvisioningService>();
        _portalRepositories = A.Fake<IPortalRepositories>();
        _identityService = A.Fake<IIdentityService>();
        _networkRegistrationProcessHelper = A.Fake<INetworkRegistrationProcessHelper>();

        _companyRepository = A.Fake<ICompanyRepository>();
        _companyRolesRepository = A.Fake<ICompanyRolesRepository>();
        _processStepRepository = A.Fake<IPortalProcessStepRepository>();
        _applicationRepository = A.Fake<IApplicationRepository>();
        _networkRepository = A.Fake<INetworkRepository>();
        _identityProviderRepository = A.Fake<IIdentityProviderRepository>();
        _countryRepository = A.Fake<ICountryRepository>();
        _identity = A.Fake<IIdentityData>();

        var settings = new PartnerRegistrationSettings
        {
            InitialRoles = new[] { new UserRoleConfig("cl1", new[] { "Company Admin" }) },
            ApplicationsMaxPageSize = 20
        };
        var options = A.Fake<IOptions<PartnerRegistrationSettings>>();

        A.CallTo(() => options.Value).Returns(settings);
        A.CallTo(() => _identity.IdentityId).Returns(Guid.NewGuid());
        A.CallTo(() => _identity.IdentityTypeId).Returns(IdentityTypeId.COMPANY_USER);
        A.CallTo(() => _identity.CompanyId).Returns(CompanyId);
        A.CallTo(() => _identityService.IdentityData).Returns(_identity);

        A.CallTo(() => _portalRepositories.GetInstance<ICompanyRepository>()).Returns(_companyRepository);
        A.CallTo(() => _portalRepositories.GetInstance<ICompanyRolesRepository>()).Returns(_companyRolesRepository);
        A.CallTo(() => _portalRepositories.GetInstance<IPortalProcessStepRepository>()).Returns(_processStepRepository);
        A.CallTo(() => _portalRepositories.GetInstance<IApplicationRepository>()).Returns(_applicationRepository);
        A.CallTo(() => _portalRepositories.GetInstance<INetworkRepository>()).Returns(_networkRepository);
        A.CallTo(() => _portalRepositories.GetInstance<IIdentityProviderRepository>()).Returns(_identityProviderRepository);
        A.CallTo(() => _portalRepositories.GetInstance<ICountryRepository>()).Returns(_countryRepository);

        _sut = new NetworkBusinessLogic(_portalRepositories, _identityService, _userProvisioningService, _networkRegistrationProcessHelper, options);

        SetupRepos();
    }

    #region HandlePartnerRegistration

    [Theory]
    [InlineData("TEST00000012")]
    [InlineData("BPNL1234567899")]
    public async Task HandlePartnerRegistration_WithInvalidBusinessPartnerNumber_ThrowsControllerArgumentException(string? bpn)
    {
        // Arrange
        var data = _fixture.Build<PartnerRegistrationData>()
            .With(x => x.BusinessPartnerNumber, bpn)
            .Create();

        // Act
        async Task Act() => await _sut.HandlePartnerRegistration(data);

        // Assert
        var ex = await Assert.ThrowsAsync<ControllerArgumentException>(Act);
        ex.Message.Should().Be(RegistrationValidationErrors.BPN_INVALID.ToString());
    }

    [Fact]
    public async Task HandlePartnerRegistration_WithInvalidUniqueId_ThrowsControllerArgumentException()
    {
        // Arrange
        var data = _fixture.Build<PartnerRegistrationData>()
            .With(x => x.BusinessPartnerNumber, Bpn)
            .With(x => x.CountryAlpha2Code, "DE")
            .With(x => x.Region, "XX")
            .With(x => x.UniqueIds, [new CompanyUniqueIdData(UniqueIdentifierId.VAT_ID, "123"), new CompanyUniqueIdData(UniqueIdentifierId.COMMERCIAL_REG_NUMBER, "12")])
            .Create();

        // Act
        async Task Act() => await _sut.HandlePartnerRegistration(data);

        // Assert
        var ex = await Assert.ThrowsAsync<ControllerArgumentException>(Act);
        ex.Message.Should().Be("Invalid value of uniqueIds: 'VAT_ID, COMMERCIAL_REG_NUMBER' (Parameter 'UniqueIds')");
        ex.ParamName.Should().Be("UniqueIds");
    }

    [Fact]
    public async Task HandlePartnerRegistration_WithoutExistingBusinessPartnerNumber_ThrowsControllerArgumentException()
    {
        // Arrange
        var data = _fixture.Build<PartnerRegistrationData>()
            .With(x => x.CountryAlpha2Code, "DE")
            .With(x => x.Region, "XX")
            .With(x => x.BusinessPartnerNumber, "BPNL00000001FAIL")
            .With(x => x.UniqueIds, [new CompanyUniqueIdData(UniqueIdentifierId.VAT_ID, VatId)])
            .Create();

        // Act
        async Task Act() => await _sut.HandlePartnerRegistration(data);

        // Assert
        var ex = await Assert.ThrowsAsync<ControllerArgumentException>(Act);
        ex.Message.Should().Be(RegistrationValidationErrors.BPN_ALREADY_EXISTS.ToString());
    }

    [Fact]
    public async Task HandlePartnerRegistration_WithInvalidCompanyUserRole_ThrowsControllerArgumentException()
    {
        // Arrange
        var data = _fixture.Build<PartnerRegistrationData>()
            .With(x => x.BusinessPartnerNumber, Bpn)
            .With(x => x.CountryAlpha2Code, "DE")
            .With(x => x.Region, "X")
            .With(x => x.CompanyRoles, Enumerable.Empty<CompanyRoleId>())
            .With(x => x.UniqueIds, [new CompanyUniqueIdData(UniqueIdentifierId.VAT_ID, VatId)])
            .Create();

        // Act
        async Task Act() => await _sut.HandlePartnerRegistration(data);

        // Assert
        var ex = await Assert.ThrowsAsync<ControllerArgumentException>(Act);
        ex.Message.Should().Be(AdministrationNetworkErrors.NETWORK_ARGUMENT_LEAST_ONE_COMP_ROLE_SELECT.ToString());
    }

    [Theory]
    [InlineData("")]
    [InlineData("abc.example.com")]
    [InlineData("a@b@c@example.com")]
    public async Task HandlePartnerRegistration_WithInvalidEmail_ThrowsControllerArgumentException(string email)
    {
        // Arrange
        var data = _fixture.Build<PartnerRegistrationData>()
            .With(x => x.BusinessPartnerNumber, Bpn)
            .With(x => x.CountryAlpha2Code, "DE")
            .With(x => x.Region, "XXX")
            .With(x => x.UserDetails, new[] { new UserDetailData(null, Guid.NewGuid().ToString(), "test", "Test", "test", email) })
            .With(x => x.UniqueIds, [new CompanyUniqueIdData(UniqueIdentifierId.VAT_ID, VatId)])
            .Create();

        // Act
        async Task Act() => await _sut.HandlePartnerRegistration(data);

        // Assert
        var ex = await Assert.ThrowsAsync<ControllerArgumentException>(Act);
        ex.Message.Should().Be(AdministrationNetworkErrors.NETWORK_ARGUMENT_MAIL_NOT_EMPTY_WITH_VALID_FORMAT.ToString());
    }

    [Theory]
    [InlineData("")]
    [InlineData("Berlin")]
    public async Task HandlePartnerRegistration_WithInvalidRegion_ThrowsControllerArgumentException(string region)
    {
        // Arrange
        var data = _fixture.Build<PartnerRegistrationData>()
            .With(x => x.BusinessPartnerNumber, Bpn)
            .With(x => x.CountryAlpha2Code, "DE")
            .With(x => x.Region, region)
            .With(x => x.UserDetails, new[] { new UserDetailData(null, Guid.NewGuid().ToString(), "test", "Test", "test", "test@email.com") })
            .With(x => x.UniqueIds, [new CompanyUniqueIdData(UniqueIdentifierId.VAT_ID, VatId)])
            .Create();

        // Act
        async Task Act() => await _sut.HandlePartnerRegistration(data);

        // Assert
        var ex = await Assert.ThrowsAsync<ControllerArgumentException>(Act);
        ex.Message.Should().Be(RegistrationValidationErrors.REGION_INVALID.ToString());
    }

    [Theory]
    [InlineData("")]
    public async Task HandlePartnerRegistration_WithInvalidFirstnameEmail_ThrowsControllerArgumentException(string firstName)
    {
        // Arrange
        var data = _fixture.Build<PartnerRegistrationData>()
            .With(x => x.BusinessPartnerNumber, Bpn)
            .With(x => x.CountryAlpha2Code, "DE")
            .With(x => x.Region, "XX")
            .With(x => x.UserDetails, new[] { new UserDetailData(null, Guid.NewGuid().ToString(), "test", firstName, "test", "test@email.com") })
            .With(x => x.UniqueIds, [new CompanyUniqueIdData(UniqueIdentifierId.VAT_ID, VatId)])
            .Create();

        // Act
        async Task Act() => await _sut.HandlePartnerRegistration(data);

        // Assert
        var ex = await Assert.ThrowsAsync<ControllerArgumentException>(Act);
        ex.Message.Should().Be(AdministrationNetworkErrors.NETWORK_ARGUMENT_FIRST_NAME_NOT_MATCH_FORMAT.ToString());
    }

    [Theory]
    [InlineData("")]
    public async Task HandlePartnerRegistration_WithInvalidLastnameEmail_ThrowsControllerArgumentException(string lastname)
    {
        // Arrange
        var data = _fixture.Build<PartnerRegistrationData>()
            .With(x => x.BusinessPartnerNumber, Bpn)
            .With(x => x.CountryAlpha2Code, "DE")
            .With(x => x.Region, "X")
            .With(x => x.UserDetails, new[] { new UserDetailData(null, Guid.NewGuid().ToString(), "test", "test", lastname, "test@email.com") })
            .With(x => x.UniqueIds, [new CompanyUniqueIdData(UniqueIdentifierId.VAT_ID, VatId)])
            .Create();

        // Act
        async Task Act() => await _sut.HandlePartnerRegistration(data);

        // Assert
        var ex = await Assert.ThrowsAsync<ControllerArgumentException>(Act);
        ex.Message.Should().Be(AdministrationNetworkErrors.NETWORK_ARGUMENT_LAST_NAME_NOT_MATCH_FORMAT.ToString());
    }

    [Fact]
    public async Task HandlePartnerRegistration_WithExistingExternalId_ThrowsControllerArgumentException()
    {
        // Arrange
        var data = _fixture.Build<PartnerRegistrationData>()
            .With(x => x.BusinessPartnerNumber, Bpn)
            .With(x => x.CountryAlpha2Code, "DE")
            .With(x => x.Region, "XXX")
            .With(x => x.UserDetails, new[] { new UserDetailData(null, Guid.NewGuid().ToString(), "test", "test", "test", "test@email.com") })
            .With(x => x.ExternalId, ExistingExternalId)
            .With(x => x.UniqueIds, [new CompanyUniqueIdData(UniqueIdentifierId.VAT_ID, VatId)])
            .Create();

        // Act
        async Task Act() => await _sut.HandlePartnerRegistration(data);

        // Assert
        var ex = await Assert.ThrowsAsync<ControllerArgumentException>(Act);
        ex.Message.Should().Be(AdministrationNetworkErrors.NETWORK_ARGUMENT_EXTERNALID_EXISTS.ToString());
        ex.Parameters.First().Name.Should().Be("ExternalId");
    }

    [Fact]
    public async Task HandlePartnerRegistration_WithInvalidCountryCode_ThrowsControllerArgumentException()
    {
        // Arrange
        var data = _fixture.Build<PartnerRegistrationData>()
            .With(x => x.BusinessPartnerNumber, Bpn)
            .With(x => x.UserDetails, new[] { new UserDetailData(null, Guid.NewGuid().ToString(), "test", "test", "test", "test@email.com") })
            .With(x => x.CountryAlpha2Code, "XX")
            .With(x => x.Region, "XX")
            .With(x => x.UniqueIds, [new CompanyUniqueIdData(UniqueIdentifierId.VAT_ID, VatId)])
            .Create();

        // Act
        async Task Act() => await _sut.HandlePartnerRegistration(data);

        // Assert
        var ex = await Assert.ThrowsAsync<ControllerArgumentException>(Act);
        ex.Message.Should().Be(RegistrationValidationErrors.COUNTRY_CODE_DOES_NOT_EXIST.ToString());
    }

    [Fact]
    public async Task HandlePartnerRegistration_WithNoIdpIdSetAndNoManagedIdps_ThrowsConflictException()
    {
        // Arrange
        var data = _fixture.Build<PartnerRegistrationData>()
            .With(x => x.ExternalId, Guid.NewGuid().ToString())
            .With(x => x.BusinessPartnerNumber, Bpn)
            .With(x => x.CountryAlpha2Code, "DE")
            .With(x => x.Region, "XX")
            .With(x => x.UserDetails, new[] { new UserDetailData(null, "123", "test", "test", "test", "test@email.com") })
            .With(x => x.UniqueIds, [new CompanyUniqueIdData(UniqueIdentifierId.VAT_ID, VatId)])
            .Create();
        A.CallTo(() => _identity.CompanyId).Returns(NoIdpCompanyId);

        // Act
        async Task Act() => await _sut.HandlePartnerRegistration(data);

        // Assert
        var ex = await Assert.ThrowsAsync<ConflictException>(Act);
        ex.Message.Should().Be(AdministrationNetworkErrors.NETWORK_CONFLICT_NO_MANAGED_PROVIDER.ToString());
    }

    [Fact]
    public async Task HandlePartnerRegistration_WithNoIdpIdSetAndMultipleManagedIdps_ThrowsControllerArgumentException()
    {
        // Arrange
        var data = _fixture.Build<PartnerRegistrationData>()
            .With(x => x.ExternalId, Guid.NewGuid().ToString())
            .With(x => x.BusinessPartnerNumber, Bpn)
            .With(x => x.CountryAlpha2Code, "DE")
            .With(x => x.Region, "XX")
            .With(x => x.UserDetails, new[] { new UserDetailData(null, "123", "test", "test", "test", "test@email.com") })
            .With(x => x.UniqueIds, [new CompanyUniqueIdData(UniqueIdentifierId.VAT_ID, VatId)])
            .Create();
        A.CallTo(() => _identity.CompanyId).Returns(MultiIdpCompanyId);

        // Act
        async Task Act() => await _sut.HandlePartnerRegistration(data);

        // Assert
        var ex = await Assert.ThrowsAsync<ControllerArgumentException>(Act);
        ex.Message.Should().Be(AdministrationNetworkErrors.NETWORK_ARGUMENT_IDENTIFIER_SET_FOR_ALL_USERS.ToString());

    }

    [Fact]
    public async Task HandlePartnerRegistration_WithNotExistingIdpIdSet_ThrowsControllerArgumentException()
    {
        // Arrange
        var notExistingIdpId = Guid.NewGuid();
        var data = _fixture.Build<PartnerRegistrationData>()
            .With(x => x.ExternalId, Guid.NewGuid().ToString())
            .With(x => x.BusinessPartnerNumber, Bpn)
            .With(x => x.CountryAlpha2Code, "DE")
            .With(x => x.Region, "XX")
            .With(x => x.UserDetails, new[] { new UserDetailData(notExistingIdpId, "123", "test", "test", "test", "test@email.com") })
            .With(x => x.UniqueIds, [new CompanyUniqueIdData(UniqueIdentifierId.VAT_ID, VatId)])
            .Create();

        // Act
        async Task Act() => await _sut.HandlePartnerRegistration(data);

        // Assert
        var ex = await Assert.ThrowsAsync<ControllerArgumentException>(Act);
        ex.Message.Should().Be(AdministrationNetworkErrors.NETWORK_ARGUMENT_IDPS_NOT_EXIST.ToString());
    }

    [Fact]
    public async Task HandlePartnerRegistration_WithInvalidInitialRole_ThrowsConfigurationException()
    {
        // Arrange
        var data = _fixture.Build<PartnerRegistrationData>()
            .With(x => x.ExternalId, Guid.NewGuid().ToString())
            .With(x => x.BusinessPartnerNumber, Bpn)
            .With(x => x.CountryAlpha2Code, "DE")
            .With(x => x.Region, "XX")
            .With(x => x.UserDetails, new[] { new UserDetailData(IdpId, "123", "test", "test", "test", "test@email.com") })
            .With(x => x.UniqueIds, [new CompanyUniqueIdData(UniqueIdentifierId.VAT_ID, VatId)])
            .Create();
        A.CallTo(() => _userProvisioningService.GetRoleDatas(A<IEnumerable<UserRoleConfig>>._))
            .Throws(new ControllerArgumentException($"invalid roles: clientId: 'cl1', roles: [Company Admin]"));

        // Act
        async Task Act() => await _sut.HandlePartnerRegistration(data);

        // Assert
        var ex = await Assert.ThrowsAsync<ConfigurationException>(Act);
        ex.Message.Should().Be("InitialRoles: invalid roles: clientId: 'cl1', roles: [Company Admin]");
    }

    [Fact]
    public async Task HandlePartnerRegistration_WithUserCreationThrowsException_ThrowsServiceException()
    {
        // Arrange
        var newCompanyId = Guid.NewGuid();
        var processId = Guid.NewGuid();

        var data = new PartnerRegistrationData(
            Guid.NewGuid().ToString(),
            "Test N2N",
            null,
            Bpn,
            "Munich",
            "Street",
            "DE",
            "BY",
            "5",
            "00001",
            new[] { new CompanyUniqueIdData(UniqueIdentifierId.VAT_ID, "DE123456789") },
            new[] { new UserDetailData(IdpId, "123", "ironman", "tony", "stark", "tony@stark.com") },
            new[] { CompanyRoleId.APP_PROVIDER, CompanyRoleId.SERVICE_PROVIDER }
        );
        A.CallTo(() => _companyRepository.CreateCompany(A<string>._, A<Action<Company>>._))
            .Returns(new Company(newCompanyId, null!, default, default));

        A.CallTo(() => _processStepRepository.CreateProcess(ProcessTypeId.PARTNER_REGISTRATION))
            .Returns(new Process(processId, default, default));

        A.CallTo(() => _userProvisioningService.GetOrCreateCompanyUser(A<IUserRepository>._, A<string>._, A<UserCreationRoleDataIdpInfo>._, A<Guid>._, A<Guid>._, "BPNL00000001TEST"))
            .Throws(new UnexpectedConditionException("Test message"));

        // Act
        async Task Act() => await _sut.HandlePartnerRegistration(data);

        // Assert
        var ex = await Assert.ThrowsAsync<ServiceException>(Act);
        ex.Message.Should().Be(AdministrationNetworkErrors.NETWORK_SERVICE_ERROR_SAVED_USERS.ToString());
    }

    [Fact]
    public async Task HandlePartnerRegistration_WithSingleIdpWithoutAlias_ThrowsServiceException()
    {
        // Arrange
        var data = new PartnerRegistrationData(
            Guid.NewGuid().ToString(),
            "Test N2N",
            "N2N",
            Bpn,
            "Munich",
            "Street",
            "DE",
            "BY",
            "5",
            "00001",
            new[] { new CompanyUniqueIdData(UniqueIdentifierId.VAT_ID, "DE123456789") },
            new[] { new UserDetailData(null, "123", "ironman", "tony", "stark", "tony@stark.com") },
            new[] { CompanyRoleId.APP_PROVIDER, CompanyRoleId.SERVICE_PROVIDER }
        );
        A.CallTo(() => _identity.CompanyId).Returns(NoAliasIdpCompanyId);

        // Act
        async Task Act() => await _sut.HandlePartnerRegistration(data);

        // Assert
        var ex = await Assert.ThrowsAsync<ConflictException>(Act);
        ex.Message.Should().Be(AdministrationNetworkErrors.NETWORK_CONFLICT_IDENTITY_PROVIDER_AS_NO_ALIAS.ToString());
    }

    [Theory]
    // Worldwide
    [InlineData(UniqueIdentifierId.VAT_ID, "WW129273398", "WW")]
    [InlineData(UniqueIdentifierId.VIES, "WW129273398", "WW")]
    [InlineData(UniqueIdentifierId.COMMERCIAL_REG_NUMBER, "München HRB 175450", "WW")]
    [InlineData(UniqueIdentifierId.COMMERCIAL_REG_NUMBER, "F1103R_HRB98814", "WW")]
    [InlineData(UniqueIdentifierId.EORI, "WW12345678912345", "WW")]
    [InlineData(UniqueIdentifierId.LEI_CODE, "529900T8BM49AURSDO55", "WW")]

    // DE
    [InlineData(UniqueIdentifierId.VAT_ID, "DE129273398", "DE")]
    [InlineData(UniqueIdentifierId.COMMERCIAL_REG_NUMBER, "München HRB 175450", "DE")]
    [InlineData(UniqueIdentifierId.COMMERCIAL_REG_NUMBER, "F1103R_HRB98814", "DE")]

    // FR
    [InlineData(UniqueIdentifierId.COMMERCIAL_REG_NUMBER, "849281571", "FR")]

    // MX
    [InlineData(UniqueIdentifierId.VAT_ID, "MX-1234567890", "MX")]
    [InlineData(UniqueIdentifierId.VAT_ID, "MX1234567890", "MX")]
    [InlineData(UniqueIdentifierId.VAT_ID, "MX1234567890&", "MX")]

    // IN
    [InlineData(UniqueIdentifierId.VAT_ID, "IN123456789", "IN")]
    [InlineData(UniqueIdentifierId.VAT_ID, "IN-123456789", "IN")]
    public async Task HandlePartnerRegistration_WithIdpNotSetAndOnlyOneIdp_CallsExpected(UniqueIdentifierId uniqueIdentifierId, string identifierValue, string countryCode)
    {
        // Arrange
        var newCompanyId = Guid.NewGuid();
        var newProcessId = Guid.NewGuid();
        var newApplicationId = Guid.NewGuid();

        var addresses = new List<Address>();
        var companies = new List<Company>();
        var companyAssignedRoles = new List<CompanyAssignedRole>();
        var processes = new List<Process>();
        var processSteps = new List<ProcessStep<Process, ProcessTypeId, ProcessStepTypeId>>();
        var companyApplications = new List<CompanyApplication>();
        var networkRegistrations = new List<NetworkRegistration>();

        var data = new PartnerRegistrationData(
            Guid.NewGuid().ToString(),
            "Test N2N",
            null,
            Bpn,
            "Munich",
            "Street",
            countryCode,
            "BY",
            "5",
            "00001",
            new[] { new CompanyUniqueIdData(uniqueIdentifierId, identifierValue) },
            Enumerable.Range(1, 10).Select(_ => _fixture.Build<UserDetailData>().With(x => x.IdentityProviderId, default(Guid?)).WithEmailPattern(x => x.Email).Create()).ToImmutableArray(),
            new[] { CompanyRoleId.APP_PROVIDER, CompanyRoleId.SERVICE_PROVIDER }
        );
        A.CallTo(() => _companyRepository.CreateAddress(A<string>._, A<string>._, A<string>._, A<string>._, A<Action<Address>>._))
            .Invokes((string city, string streetname, string region, string countryAlpha2Code, Action<Address>? setOptionalParameters) =>
                {
                    var address = new Address(
                        Guid.NewGuid(),
                        city,
                        streetname,
                        region,
                        countryAlpha2Code,
                        DateTimeOffset.UtcNow
                    );
                    setOptionalParameters?.Invoke(address);
                    addresses.Add(address);
                });
        A.CallTo(() => _companyRepository.CreateCompany(A<string>._, A<Action<Company>>._))
            .ReturnsLazily((string name, Action<Company>? setOptionalParameters) =>
            {
                var company = new Company(
                    newCompanyId,
                    name,
                    CompanyStatusId.PENDING,
                    DateTimeOffset.UtcNow
                );
                setOptionalParameters?.Invoke(company);
                companies.Add(company);
                return company;
            });
        A.CallTo(() => _companyRolesRepository.CreateCompanyAssignedRoles(newCompanyId, A<IEnumerable<CompanyRoleId>>._))
            .Invokes((Guid companyId, IEnumerable<CompanyRoleId> companyRoleIds) =>
            {
                companyAssignedRoles.AddRange(companyRoleIds.Select(x => new CompanyAssignedRole(companyId, x)));
            });
        A.CallTo(() => _processStepRepository.CreateProcess(ProcessTypeId.PARTNER_REGISTRATION))
            .ReturnsLazily((ProcessTypeId processTypeId) =>
            {
                var process = new Process(newProcessId, processTypeId, Guid.NewGuid());
                processes.Add(process);
                return process;
            });
        A.CallTo(() => _processStepRepository.CreateProcessStepRange(A<IEnumerable<(ProcessStepTypeId, ProcessStepStatusId, Guid)>>._))
            .Invokes((IEnumerable<(ProcessStepTypeId ProcessStepTypeId, ProcessStepStatusId ProcessStepStatusId, Guid ProcessId)> foo) =>
            {
                processSteps.AddRange(foo.Select(x => new ProcessStep<Process, ProcessTypeId, ProcessStepTypeId>(Guid.NewGuid(), x.ProcessStepTypeId, x.ProcessStepStatusId, x.ProcessId, DateTimeOffset.UtcNow)));
            });
        A.CallTo(() => _applicationRepository.CreateCompanyApplication(A<Guid>._, A<CompanyApplicationStatusId>._, A<CompanyApplicationTypeId>._, A<Action<CompanyApplication>>._))
            .ReturnsLazily((Guid companyId, CompanyApplicationStatusId companyApplicationStatusId, CompanyApplicationTypeId applicationTypeId, Action<CompanyApplication>? setOptionalFields) =>
            {
                var companyApplication = new CompanyApplication(
                    newApplicationId,
                    companyId,
                    companyApplicationStatusId,
                    applicationTypeId,
                    DateTimeOffset.UtcNow);
                setOptionalFields?.Invoke(companyApplication);
                companyApplications.Add(companyApplication);
                return companyApplication;
            });
        A.CallTo(() => _networkRepository.CreateNetworkRegistration(A<string>._, A<Guid>._, A<Guid>._, A<Guid>._, A<Guid>._))
            .Invokes((string externalId, Guid companyId, Guid pId, Guid ospId, Guid companyApplicationId) =>
            {
                networkRegistrations.Add(new NetworkRegistration(Guid.NewGuid(), externalId, companyId, pId, ospId, companyApplicationId, DateTimeOffset.UtcNow));
            });

        // Act
        await _sut.HandlePartnerRegistration(data);

        // Assert
        addresses.Should().ContainSingle()
            .Which.Should().Match<Address>(x =>
                x.Region == data.Region &&
                x.Zipcode == data.ZipCode);
        companies.Should().ContainSingle()
            .Which.Should().Match<Company>(x =>
                x.Name == data.Name &&
                x.Shortname == data.ShortName &&
                x.CompanyStatusId == CompanyStatusId.PENDING);
        processes.Should().ContainSingle()
            .Which.Should().Match<Process>(
                x => x.ProcessTypeId == ProcessTypeId.PARTNER_REGISTRATION);
        processSteps.Should().HaveCount(2).And.Satisfy(
                x => x.ProcessStepStatusId == ProcessStepStatusId.TODO && x.ProcessStepTypeId == ProcessStepTypeId.SYNCHRONIZE_USER,
                x => x.ProcessStepStatusId == ProcessStepStatusId.TODO && x.ProcessStepTypeId == ProcessStepTypeId.MANUAL_DECLINE_OSP);
        companyApplications.Should().ContainSingle()
            .Which.Should().Match<CompanyApplication>(x =>
                x.CompanyId == newCompanyId &&
                x.ApplicationStatusId == CompanyApplicationStatusId.CREATED);
        companyAssignedRoles.Should().HaveCount(2).And.Satisfy(
            x => x.CompanyRoleId == CompanyRoleId.APP_PROVIDER,
            x => x.CompanyRoleId == CompanyRoleId.SERVICE_PROVIDER);
        networkRegistrations.Should().ContainSingle()
            .Which.Should().Match<NetworkRegistration>(x =>
                x.ExternalId == data.ExternalId &&
                x.ProcessId == newProcessId &&
                x.ApplicationId == newApplicationId);

        A.CallTo(() => _userProvisioningService.GetOrCreateCompanyUser(A<IUserRepository>._, "test-alias", A<UserCreationRoleDataIdpInfo>._, newCompanyId, IdpId, Bpn))
            .MustHaveHappened(10, Times.Exactly);
        var idpCreationData = new[] { (newCompanyId, IdpId) };
        A.CallTo(() => _identityProviderRepository.CreateCompanyIdentityProviders(A<IEnumerable<(Guid, Guid)>>.That.IsSameSequenceAs(idpCreationData)))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _portalRepositories.SaveAsync()).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [InlineData(Bpn)]
    [InlineData(null)]
    public async Task HandlePartnerRegistration_WithValidData_CallsExpected(string? businessPartnerNumber)
    {
        // Arrange
        var newCompanyId = Guid.NewGuid();
        var newProcessId = Guid.NewGuid();
        var newApplicationId = Guid.NewGuid();

        var addresses = new List<Address>();
        var companies = new List<Company>();
        var companyAssignedRoles = new List<CompanyAssignedRole>();
        var processes = new List<Process>();
        var processSteps = new List<ProcessStep<Process, ProcessTypeId, ProcessStepTypeId>>();
        var companyApplications = new List<CompanyApplication>();
        var networkRegistrations = new List<NetworkRegistration>();
        var invitations = new List<Invitation>();

        var data = new PartnerRegistrationData(
            Guid.NewGuid().ToString(),
            "Test N2N",
            "N2N",
            businessPartnerNumber,
            "Munich",
            "Street",
            "DE",
            "BY",
            "5",
            "00001",
            new[] { new CompanyUniqueIdData(UniqueIdentifierId.VAT_ID, "DE123456789") },
            new[] { new UserDetailData(IdpId, "123", "ironman", "tony", "stark", "tony@stark.com") },
            new[] { CompanyRoleId.APP_PROVIDER, CompanyRoleId.SERVICE_PROVIDER }
        );
        A.CallTo(() => _companyRepository.CreateAddress(A<string>._, A<string>._, A<string>._, A<string>._, A<Action<Address>>._))
            .Invokes((string city, string streetname, string region, string countryAlpha2Code, Action<Address>? setOptionalParameters) =>
                {
                    var address = new Address(
                        Guid.NewGuid(),
                        city,
                        streetname,
                        region,
                        countryAlpha2Code,
                        DateTimeOffset.UtcNow
                    );
                    setOptionalParameters?.Invoke(address);
                    addresses.Add(address);
                });
        A.CallTo(() => _companyRepository.CreateCompany(A<string>._, A<Action<Company>>._))
            .ReturnsLazily((string name, Action<Company>? setOptionalParameters) =>
            {
                var company = new Company(
                    newCompanyId,
                    name,
                    CompanyStatusId.PENDING,
                    DateTimeOffset.UtcNow
                );
                setOptionalParameters?.Invoke(company);
                companies.Add(company);
                return company;
            });
        A.CallTo(() => _companyRolesRepository.CreateCompanyAssignedRoles(A<Guid>._, A<IEnumerable<CompanyRoleId>>._))
            .Invokes((Guid companyId, IEnumerable<CompanyRoleId> companyRoleIds) =>
            {
                companyAssignedRoles.AddRange(companyRoleIds.Select(x => new CompanyAssignedRole(companyId, x)));
            });
        A.CallTo(() => _processStepRepository.CreateProcess(A<ProcessTypeId>._))
            .ReturnsLazily((ProcessTypeId processTypeId) =>
            {
                var process = new Process(newProcessId, processTypeId, Guid.NewGuid());
                processes.Add(process);
                return process;
            });
        A.CallTo(() => _processStepRepository.CreateProcessStepRange(A<IEnumerable<(ProcessStepTypeId, ProcessStepStatusId, Guid)>>._))
            .Invokes((IEnumerable<(ProcessStepTypeId ProcessStepTypeId, ProcessStepStatusId ProcessStepStatusId, Guid ProcessId)> processStepTypeStatus) =>
            {
                processSteps.AddRange(processStepTypeStatus.Select(x => new ProcessStep<Process, ProcessTypeId, ProcessStepTypeId>(Guid.NewGuid(), x.ProcessStepTypeId, x.ProcessStepStatusId, x.ProcessId, DateTimeOffset.UtcNow)).ToList());
            });
        A.CallTo(() => _applicationRepository.CreateCompanyApplication(A<Guid>._, A<CompanyApplicationStatusId>._, A<CompanyApplicationTypeId>._, A<Action<CompanyApplication>>._))
            .ReturnsLazily((Guid companyId, CompanyApplicationStatusId companyApplicationStatusId, CompanyApplicationTypeId applicationTypeId, Action<CompanyApplication>? setOptionalFields) =>
            {
                var companyApplication = new CompanyApplication(
                    newApplicationId,
                    companyId,
                    companyApplicationStatusId,
                    applicationTypeId,
                    DateTimeOffset.UtcNow);
                setOptionalFields?.Invoke(companyApplication);
                companyApplications.Add(companyApplication);
                return companyApplication;
            });
        A.CallTo(() => _applicationRepository.CreateInvitation(A<Guid>._, A<Guid>._))
            .Invokes((Guid applicationId, Guid companyUserId) =>
            {
                invitations.Add(new Invitation(Guid.NewGuid(), applicationId, companyUserId, InvitationStatusId.CREATED, DateTimeOffset.UtcNow));
            });
        A.CallTo(() => _networkRepository.CreateNetworkRegistration(A<string>._, A<Guid>._, A<Guid>._, A<Guid>._, A<Guid>._))
            .Invokes((string externalId, Guid companyId, Guid pId, Guid ospId, Guid companyApplicationId) =>
            {
                networkRegistrations.Add(new NetworkRegistration(Guid.NewGuid(), externalId, companyId, pId, ospId, companyApplicationId, DateTimeOffset.UtcNow));
            });

        // Act
        await _sut.HandlePartnerRegistration(data);

        // Assert
        addresses.Should().ContainSingle()
            .Which.Should().Match<Address>(x =>
                x.Region == data.Region &&
                x.Zipcode == data.ZipCode);
        companies.Should().ContainSingle()
            .Which.Should().Match<Company>(x =>
                x.Name == data.Name &&
                x.Shortname == data.ShortName &&
                x.CompanyStatusId == CompanyStatusId.PENDING);
        processes.Should().ContainSingle()
            .Which.Should().Match<Process>(x =>
                x.ProcessTypeId == ProcessTypeId.PARTNER_REGISTRATION);
        processSteps.Should().HaveCount(2).And.Satisfy(
            x => x.ProcessStepStatusId == ProcessStepStatusId.TODO && x.ProcessStepTypeId == ProcessStepTypeId.SYNCHRONIZE_USER,
            x => x.ProcessStepStatusId == ProcessStepStatusId.TODO && x.ProcessStepTypeId == ProcessStepTypeId.MANUAL_DECLINE_OSP);
        companyApplications.Should().ContainSingle()
            .Which.Should().Match<CompanyApplication>(x =>
                x.CompanyId == newCompanyId &&
                x.ApplicationStatusId == CompanyApplicationStatusId.CREATED);
        companyAssignedRoles.Should().HaveCount(2).And.Satisfy(
            x => x.CompanyRoleId == CompanyRoleId.APP_PROVIDER,
            x => x.CompanyRoleId == CompanyRoleId.SERVICE_PROVIDER);
        networkRegistrations.Should().ContainSingle()
            .Which.Should().Match<NetworkRegistration>(x =>
                x.ExternalId == data.ExternalId &&
                x.ProcessId == newProcessId &&
                x.ApplicationId == newApplicationId);
        invitations.Should().ContainSingle()
            .Which.Should().Match<Invitation>(x => x.CompanyApplicationId == newApplicationId);

        A.CallTo(() => _userProvisioningService.GetOrCreateCompanyUser(A<IUserRepository>._, "test-alias", A<UserCreationRoleDataIdpInfo>._, newCompanyId, IdpId, businessPartnerNumber))
            .MustHaveHappenedOnceExactly();
        var idpCreationData = new[] { (newCompanyId, IdpId) };
        A.CallTo(() => _identityProviderRepository.CreateCompanyIdentityProviders(A<IEnumerable<(Guid, Guid)>>.That.IsSameSequenceAs(idpCreationData)))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _portalRepositories.SaveAsync()).MustHaveHappenedOnceExactly();
    }

    #endregion

    #region RetriggerSynchronizeUser

    [Fact]
    public async Task RetriggerSynchronizeUser_CallsExpected()
    {
        // Arrange
        var externalId = Guid.NewGuid().ToString();
        const ProcessStepTypeId ProcessStepId = ProcessStepTypeId.RETRIGGER_SYNCHRONIZE_USER;

        // Act
        await _sut.RetriggerProcessStep(externalId, ProcessStepId);

        // Assert
        A.CallTo(() => _networkRegistrationProcessHelper.TriggerProcessStep(externalId, ProcessStepId)).MustHaveHappenedOnceExactly();
    }

    #endregion

    #region GetOSPCompanyApplicationDetailsAsync

    [Theory]
    [InlineData(null, null)]
    [InlineData(null, DateCreatedOrderFilter.ASC)]
    [InlineData(null, DateCreatedOrderFilter.DESC)]
    [InlineData(CompanyApplicationStatusFilter.Closed, null)]
    [InlineData(CompanyApplicationStatusFilter.InReview, null)]
    [InlineData(CompanyApplicationStatusFilter.Closed, DateCreatedOrderFilter.ASC)]
    [InlineData(CompanyApplicationStatusFilter.InReview, DateCreatedOrderFilter.ASC)]
    [InlineData(CompanyApplicationStatusFilter.Closed, DateCreatedOrderFilter.DESC)]
    [InlineData(CompanyApplicationStatusFilter.InReview, DateCreatedOrderFilter.DESC)]
    public async Task GetOspCompanyApplicationDetailsAsync_WithDefaultRequest_GetsExpectedEntries(CompanyApplicationStatusFilter? statusFilter, DateCreatedOrderFilter? dateCreatedOrderFilter)
    {
        // Arrange
        var companyName = "TestCompany";
        var externalId = _fixture.Create<string>();
        var data = _fixture.CreateMany<(Guid Id, Guid CompanyId, CompanyApplicationStatusId CompanyApplicationStatusId, DateTimeOffset Created)>(10)
            .Select(x => new CompanyApplication(x.Id, x.CompanyId, x.CompanyApplicationStatusId, CompanyApplicationTypeId.EXTERNAL, x.Created)
            {
                Company = new Company(x.CompanyId, _fixture.Create<string>(), _fixture.Create<CompanyStatusId>(), x.Created)
                {
                    Name = _fixture.Create<string>(),
                    BusinessPartnerNumber = _fixture.Create<string>(),
                },
                NetworkRegistration = new NetworkRegistration(Guid.NewGuid(), _fixture.Create<string>(), x.CompanyId, Guid.NewGuid(), Guid.NewGuid(), x.Id, x.Created)
                {
                    ExternalId = _fixture.Create<string>(),
                    DateCreated = _fixture.Create<DateTimeOffset>(),
                },
                DateLastChanged = _fixture.Create<DateTimeOffset>()
            }).ToImmutableList();

        var queryData = new AsyncEnumerableStub<CompanyApplication>(data).AsQueryable();

        A.CallTo(() => _applicationRepository.GetExternalCompanyApplicationsFilteredQuery(A<Guid>._, A<string?>._, A<string?>._, A<IEnumerable<CompanyApplicationStatusId>>._))
            .Returns(queryData);

        // Act
        var result = await _sut.GetOspCompanyDetailsAsync(0, 3, statusFilter, companyName, externalId, dateCreatedOrderFilter);

        // Assert
        Assert.IsType<Pagination.Response<CompanyDetailsOspOnboarding>>(result);

        switch (statusFilter)
        {
            case CompanyApplicationStatusFilter.Closed:
                A.CallTo(() => _applicationRepository.GetExternalCompanyApplicationsFilteredQuery(CompanyId, companyName, externalId, A<IEnumerable<CompanyApplicationStatusId>>.That.IsSameSequenceAs(new[] { CompanyApplicationStatusId.CONFIRMED, CompanyApplicationStatusId.DECLINED }))).MustHaveHappenedOnceExactly();
                break;
            case CompanyApplicationStatusFilter.InReview:
                A.CallTo(() => _applicationRepository.GetExternalCompanyApplicationsFilteredQuery(CompanyId, companyName, externalId, A<IEnumerable<CompanyApplicationStatusId>>.That.IsSameSequenceAs(new[] { CompanyApplicationStatusId.SUBMITTED }))).MustHaveHappenedOnceExactly();
                break;
            default:
                A.CallTo(() => _applicationRepository.GetExternalCompanyApplicationsFilteredQuery(CompanyId, companyName, externalId, A<IEnumerable<CompanyApplicationStatusId>>.That.IsSameSequenceAs(new[] { CompanyApplicationStatusId.SUBMITTED, CompanyApplicationStatusId.CONFIRMED, CompanyApplicationStatusId.DECLINED }))).MustHaveHappenedOnceExactly();
                break;
        }

        result.Meta.NumberOfElements.Should().Be(10);

        var sorted = dateCreatedOrderFilter switch
        {
            DateCreatedOrderFilter.ASC => data.OrderBy(application => application.Company!.DateCreated).Take(3).ToImmutableArray(),
            DateCreatedOrderFilter.DESC => data.OrderByDescending(application => application.Company!.DateCreated).Take(3).ToImmutableArray(),
            _ => data.OrderByDescending(application => application.Company!.DateCreated).Take(3).ToImmutableArray()
        };

        result.Content.Should().HaveCount(3).And.Satisfy(
            x => x.ApplicationId == sorted[0].Id && x.CompanyApplicationStatusId == sorted[0].ApplicationStatusId && x.DateCreated == sorted[0].DateCreated && x.DateLastChanged == sorted[0].DateLastChanged && x.CompanyId == sorted[0].CompanyId && x.CompanyName == sorted[0].Company!.Name && x.BusinessPartnerNumber == sorted[0].Company!.BusinessPartnerNumber,
            x => x.ApplicationId == sorted[1].Id && x.CompanyApplicationStatusId == sorted[1].ApplicationStatusId && x.DateCreated == sorted[1].DateCreated && x.DateLastChanged == sorted[1].DateLastChanged && x.CompanyId == sorted[1].CompanyId && x.CompanyName == sorted[1].Company!.Name && x.BusinessPartnerNumber == sorted[1].Company!.BusinessPartnerNumber,
            x => x.ApplicationId == sorted[2].Id && x.CompanyApplicationStatusId == sorted[2].ApplicationStatusId && x.DateCreated == sorted[2].DateCreated && x.DateLastChanged == sorted[2].DateLastChanged && x.CompanyId == sorted[2].CompanyId && x.CompanyName == sorted[2].Company!.Name && x.BusinessPartnerNumber == sorted[2].Company!.BusinessPartnerNumber
        );

        switch (dateCreatedOrderFilter)
        {
            case DateCreatedOrderFilter.ASC:
                result.Content.Should().BeInAscendingOrder(x => x.DateCreated);
                break;
            case null:
            case DateCreatedOrderFilter.DESC:
                result.Content.Should().BeInDescendingOrder(x => x.DateCreated);
                break;
        }
    }

    #endregion

    #region Setup

    private void SetupRepos()
    {
        A.CallTo(() => _networkRepository.CheckExternalIdExists(ExistingExternalId, A<Guid>.That.Matches(x => x == _identity.CompanyId || x == NoIdpCompanyId)))
            .Returns(true);
        A.CallTo(() => _networkRepository.CheckExternalIdExists(A<string>.That.Not.Matches(x => x == ExistingExternalId), A<Guid>.That.Matches(x => x == _identity.CompanyId || x == NoIdpCompanyId)))
            .Returns(false);

        A.CallTo(() => _companyRepository.CheckBpnExists(Bpn)).Returns(false);
        A.CallTo(() => _companyRepository.CheckBpnExists(A<string>.That.Not.Matches(x => x == Bpn))).Returns(true);

        A.CallTo(() => _countryRepository.CheckCountryExistsByAlpha2CodeAsync("XX"))
            .Returns(false);
        A.CallTo(() => _countryRepository.CheckCountryExistsByAlpha2CodeAsync(A<string>.That.Not.Matches(x => x == "XX")))
            .Returns(true);
        A.CallTo(() => _countryRepository.GetCountryAssignedIdentifiers("WW", A<IEnumerable<UniqueIdentifierId>>._))
            .Returns((true, new[] { UniqueIdentifierId.VAT_ID, UniqueIdentifierId.LEI_CODE, UniqueIdentifierId.EORI, UniqueIdentifierId.COMMERCIAL_REG_NUMBER, UniqueIdentifierId.VIES }));
        A.CallTo(() => _countryRepository.GetCountryAssignedIdentifiers("DE", A<IEnumerable<UniqueIdentifierId>>._))
            .Returns((true, new[] { UniqueIdentifierId.VAT_ID, UniqueIdentifierId.COMMERCIAL_REG_NUMBER }));
        A.CallTo(() => _countryRepository.GetCountryAssignedIdentifiers("FR", A<IEnumerable<UniqueIdentifierId>>._))
            .Returns((true, new[] { UniqueIdentifierId.COMMERCIAL_REG_NUMBER }));
        A.CallTo(() => _countryRepository.GetCountryAssignedIdentifiers("MX", A<IEnumerable<UniqueIdentifierId>>._))
            .Returns((true, new[] { UniqueIdentifierId.VAT_ID }));
        A.CallTo(() => _countryRepository.GetCountryAssignedIdentifiers("IN", A<IEnumerable<UniqueIdentifierId>>._))
            .Returns((true, new[] { UniqueIdentifierId.VAT_ID }));
        A.CallTo(() => _countryRepository.GetCountryAssignedIdentifiers(A<string>.That.Not.Matches(x => x == "WW" || x == "DE" || x == "FR" || x == "MX" || x == "IN"), A<IEnumerable<UniqueIdentifierId>>._))
            .Returns((false, Enumerable.Empty<UniqueIdentifierId>()));

        A.CallTo(() => _identityProviderRepository.GetSingleManagedIdentityProviderAliasDataUntracked(_identity.CompanyId))
            .Returns((IdpId, "test-alias"));

        A.CallTo(() => _identityProviderRepository.GetSingleManagedIdentityProviderAliasDataUntracked(NoAliasIdpCompanyId))
            .Returns((IdpId, null));

        A.CallTo(() => _identityProviderRepository.GetSingleManagedIdentityProviderAliasDataUntracked(NoIdpCompanyId))
            .Returns<(Guid, string?)>(default);

        A.CallTo(() => _identityProviderRepository.GetSingleManagedIdentityProviderAliasDataUntracked(MultiIdpCompanyId))
            .Throws(new InvalidOperationException("Sequence contains more than one element."));

        A.CallTo(() => _identityProviderRepository.GetManagedIdentityProviderAliasDataUntracked(A<Guid>.That.Matches(x => x == _identity.CompanyId || x == NoIdpCompanyId), A<IEnumerable<Guid>>._))
            .Returns(new (Guid, string?)[] { (IdpId, "test-alias") }.ToAsyncEnumerable());

        A.CallTo(() => _userProvisioningService.GetRoleDatas(A<IEnumerable<UserRoleConfig>>._))
            .Returns(new[] { new UserRoleData(UserRoleId, "cl1", "Company Admin") }.ToAsyncEnumerable());
    }

    #endregion
}

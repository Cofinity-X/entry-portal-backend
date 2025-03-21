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

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Org.Eclipse.TractusX.Portal.Backend.Administration.Service.BusinessLogic;
using Org.Eclipse.TractusX.Portal.Backend.Administration.Service.ErrorHandling;
using Org.Eclipse.TractusX.Portal.Backend.Administration.Service.Models;
using Org.Eclipse.TractusX.Portal.Backend.Clearinghouse.Library.BusinessLogic;
using Org.Eclipse.TractusX.Portal.Backend.Clearinghouse.Library.Models;
using Org.Eclipse.TractusX.Portal.Backend.Dim.Library.BusinessLogic;
using Org.Eclipse.TractusX.Portal.Backend.Dim.Library.Models;
using Org.Eclipse.TractusX.Portal.Backend.Framework.ErrorHandling;
using Org.Eclipse.TractusX.Portal.Backend.Framework.Models;
using Org.Eclipse.TractusX.Portal.Backend.Framework.Processes.Library.Concrete.Entities;
using Org.Eclipse.TractusX.Portal.Backend.Framework.Processes.Library.DBAccess;
using Org.Eclipse.TractusX.Portal.Backend.Framework.Processes.Library.Entities;
using Org.Eclipse.TractusX.Portal.Backend.Framework.Processes.Library.Enums;
using Org.Eclipse.TractusX.Portal.Backend.Framework.Processes.Library.Models;
using Org.Eclipse.TractusX.Portal.Backend.IssuerComponent.Library.BusinessLogic;
using Org.Eclipse.TractusX.Portal.Backend.IssuerComponent.Library.Models;
using Org.Eclipse.TractusX.Portal.Backend.PortalBackend.DBAccess;
using Org.Eclipse.TractusX.Portal.Backend.PortalBackend.DBAccess.Models;
using Org.Eclipse.TractusX.Portal.Backend.PortalBackend.DBAccess.Repositories;
using Org.Eclipse.TractusX.Portal.Backend.PortalBackend.PortalEntities.Entities;
using Org.Eclipse.TractusX.Portal.Backend.PortalBackend.PortalEntities.Enums;
using Org.Eclipse.TractusX.Portal.Backend.Processes.ApplicationChecklist.Library;
using Org.Eclipse.TractusX.Portal.Backend.Processes.Mailing.Library;
using Org.Eclipse.TractusX.Portal.Backend.Provisioning.Library;
using Org.Eclipse.TractusX.Portal.Backend.Registration.Common;
using Org.Eclipse.TractusX.Portal.Backend.SdFactory.Library.BusinessLogic;
using Org.Eclipse.TractusX.Portal.Backend.SdFactory.Library.Models;
using Org.Eclipse.TractusX.Portal.Backend.Tests.Shared;
using Org.Eclipse.TractusX.Portal.Backend.Tests.Shared.Extensions;
using System.Collections.Immutable;
using System.Text.Json;

namespace Org.Eclipse.TractusX.Portal.Backend.Administration.Service.Tests.BusinessLogic;

public class RegistrationBusinessLogicTest
{
    private const string BusinessPartnerNumber = "CAXLSHAREDIDPZZ";
    private const string AlreadyTakenBpn = "BPNL123698762666";
    private const string ValidBpn = "BPNL123698762345";
    private const string CompanyName = "TestCompany";
    private const string IamAliasId = "idp1";
    private static readonly Guid IdpId = Guid.NewGuid();
    private static readonly Guid IdWithBpn = new("c244f79a-7faf-4c59-bb85-fbfdf72ce46f");
    private static readonly Guid NotExistingApplicationId = new("9f0cfd0d-c512-438e-a07e-3198bce873bf");
    private static readonly Guid ActiveApplicationCompanyId = new("045abf01-7762-468b-98fb-84a30c39b7c7");
    private static readonly Guid CompanyId = new("95c4339e-e087-4cd2-a5b8-44d385e64630");
    private static readonly Guid ExistingExternalId = Guid.NewGuid();
    private static readonly Guid IdWithoutBpn = new("d90995fe-1241-4b8d-9f5c-f3909acc6399");
    private static readonly Guid ApplicationId = new("6084d6e0-0e01-413c-850d-9f944a6c494c");
    private static readonly Guid UserId = Guid.NewGuid();

    private readonly IPortalRepositories _portalRepositories;
    private readonly IApplicationRepository _applicationRepository;
    private readonly IMailingProcessCreation _mailingProcessCreation;
    private readonly IIdentityProviderRepository _identityProviderRepository;
    private readonly IPortalProcessStepRepository _processStepRepository;
    private readonly IUserRepository _userRepository;
    private readonly IFixture _fixture;
    private readonly IRegistrationBusinessLogic _logic;
    private readonly ICompanyRepository _companyRepository;
    private readonly IApplicationChecklistService _checklistService;
    private readonly IClearinghouseBusinessLogic _clearinghouseBusinessLogic;
    private readonly ISdFactoryBusinessLogic _sdFactoryBusinessLogic;
    private readonly IIssuerComponentBusinessLogic _issuerComponentBusinessLogic;
    private readonly IDocumentRepository _documentRepository;
    private readonly IProvisioningManager _provisioningManager;
    private readonly IDimBusinessLogic _dimBusinessLogic;
    private readonly IOptions<RegistrationSettings> _options;
    private readonly ILogger<RegistrationBusinessLogic> _logger;

    public RegistrationBusinessLogicTest()
    {
        _fixture = new Fixture().Customize(new AutoFakeItEasyCustomization { ConfigureMembers = true });
        _fixture.ConfigureFixture();

        _portalRepositories = A.Fake<IPortalRepositories>();
        _applicationRepository = A.Fake<IApplicationRepository>();
        _identityProviderRepository = A.Fake<IIdentityProviderRepository>();
        _documentRepository = A.Fake<IDocumentRepository>();
        _processStepRepository = A.Fake<IPortalProcessStepRepository>();
        _userRepository = A.Fake<IUserRepository>();
        _companyRepository = A.Fake<ICompanyRepository>();
        _mailingProcessCreation = A.Fake<IMailingProcessCreation>();

        _options = A.Fake<IOptions<RegistrationSettings>>();
        var settings = A.Fake<RegistrationSettings>();
        settings.ApplicationsMaxPageSize = 15;
        A.CallTo(() => _options.Value).Returns(settings);

        _clearinghouseBusinessLogic = A.Fake<IClearinghouseBusinessLogic>();
        _sdFactoryBusinessLogic = A.Fake<ISdFactoryBusinessLogic>();
        _dimBusinessLogic = A.Fake<IDimBusinessLogic>();
        _checklistService = A.Fake<IApplicationChecklistService>();
        _issuerComponentBusinessLogic = A.Fake<IIssuerComponentBusinessLogic>();
        _provisioningManager = A.Fake<IProvisioningManager>();

        A.CallTo(() => _portalRepositories.GetInstance<IApplicationRepository>()).Returns(_applicationRepository);
        A.CallTo(() => _portalRepositories.GetInstance<IIdentityProviderRepository>()).Returns(_identityProviderRepository);
        A.CallTo(() => _portalRepositories.GetInstance<IDocumentRepository>()).Returns(_documentRepository);
        A.CallTo(() => _portalRepositories.GetInstance<IUserRepository>()).Returns(_userRepository);
        A.CallTo(() => _portalRepositories.GetInstance<ICompanyRepository>()).Returns(_companyRepository);
        A.CallTo(() => _portalRepositories.GetInstance<IPortalProcessStepRepository>()).Returns(_processStepRepository);
        A.CallTo(() => _portalRepositories.GetInstance<IProcessStepRepository<ProcessTypeId, ProcessStepTypeId>>()).Returns(_processStepRepository);

        _logger = A.Fake<ILogger<RegistrationBusinessLogic>>();

        _logic = new RegistrationBusinessLogic(_portalRepositories, _options, _checklistService, _clearinghouseBusinessLogic, _sdFactoryBusinessLogic, _dimBusinessLogic, _issuerComponentBusinessLogic, _provisioningManager, _mailingProcessCreation, _logger);
    }

    #region GetCompanyApplicationDetailsAsync

    [Fact]
    public async Task GetCompanyApplicationDetailsAsync_WithDefaultRequest_GetsExpectedEntries()
    {
        // Arrange
        var companyAppStatus = new[] { CompanyApplicationStatusId.SUBMITTED, CompanyApplicationStatusId.CONFIRMED, CompanyApplicationStatusId.DECLINED };
        var companyApplicationData = new AsyncEnumerableStub<CompanyApplication>(_fixture.CreateMany<CompanyApplication>(5));
        A.CallTo(() => _applicationRepository.GetCompanyApplicationsFilteredQuery(A<string?>._, A<IEnumerable<CompanyApplicationStatusId>>._))
            .Returns(companyApplicationData.AsQueryable());

        // Act
        var result = await _logic.GetCompanyApplicationDetailsAsync(0, 5, null, null);
        // Assert
        A.CallTo(() => _applicationRepository.GetCompanyApplicationsFilteredQuery(null, A<IEnumerable<CompanyApplicationStatusId>>.That.Matches(x => x.Count() == 3 && x.All(y => companyAppStatus.Contains(y))))).MustHaveHappenedOnceExactly();
        Assert.IsType<Pagination.Response<CompanyApplicationDetails>>(result);
        result.Content.Should().HaveCount(5);
    }

    [Fact]
    public async Task GetCompanyApplicationDetailsAsync_WithInReviewRequest_GetsExpectedEntries()
    {
        // Arrange
        var companyAppStatus = new[] { CompanyApplicationStatusId.SUBMITTED };
        var companyApplicationData = new AsyncEnumerableStub<CompanyApplication>(_fixture.CreateMany<CompanyApplication>(5));
        A.CallTo(() => _applicationRepository.GetCompanyApplicationsFilteredQuery(A<string?>._, A<IEnumerable<CompanyApplicationStatusId>>._))
            .Returns(companyApplicationData.AsQueryable());

        // Act
        var result = await _logic.GetCompanyApplicationDetailsAsync(0, 5, CompanyApplicationStatusFilter.InReview, null);
        // Assert
        A.CallTo(() => _applicationRepository.GetCompanyApplicationsFilteredQuery(null, A<IEnumerable<CompanyApplicationStatusId>>.That.Matches(x => x.Count() == 1 && x.All(y => companyAppStatus.Contains(y))))).MustHaveHappenedOnceExactly();
        Assert.IsType<Pagination.Response<CompanyApplicationDetails>>(result);
        result.Content.Should().HaveCount(5);
    }

    [Fact]
    public async Task GetCompanyApplicationDetailsAsync_WithClosedRequest_GetsExpectedEntries()
    {
        // Arrange
        var companyAppStatus = new[] { CompanyApplicationStatusId.CONFIRMED, CompanyApplicationStatusId.DECLINED };
        var companyApplicationData = new AsyncEnumerableStub<CompanyApplication>(_fixture.CreateMany<CompanyApplication>(5));
        A.CallTo(() => _applicationRepository.GetCompanyApplicationsFilteredQuery(A<string?>._, A<IEnumerable<CompanyApplicationStatusId>>._))
            .Returns(companyApplicationData.AsQueryable());

        // Act
        var result = await _logic.GetCompanyApplicationDetailsAsync(0, 5, CompanyApplicationStatusFilter.Closed, null);

        // Assert
        A.CallTo(() => _applicationRepository.GetCompanyApplicationsFilteredQuery(null, A<IEnumerable<CompanyApplicationStatusId>>.That.Matches(x => x.Count() == 2 && x.All(y => companyAppStatus.Contains(y))))).MustHaveHappenedOnceExactly();
        Assert.IsType<Pagination.Response<CompanyApplicationDetails>>(result);
        result.Content.Should().HaveCount(5);
    }

    #endregion

    #region GetCompanyWithAddressAsync

    [Theory]
    [InlineData(UniqueIdentifierId.VAT_ID, "DE124356789")]
    [InlineData(UniqueIdentifierId.LEI_CODE, "54930084UKLVMY22DS16")]
    [InlineData(UniqueIdentifierId.EORI, "DE123456789012345")]
    [InlineData(UniqueIdentifierId.COMMERCIAL_REG_NUMBER, "HRB123456")]
    [InlineData(UniqueIdentifierId.VIES, "ATU99999999")]
    public async Task GetCompanyWithAddressAsync_WithDefaultRequest_GetsExpectedResult(UniqueIdentifierId identifierIdType, string companyUniqueIds)
    {
        // Arrange
        var applicationId = _fixture.Create<Guid>();
        var data = _fixture.Build<CompanyUserRoleWithAddress>()
            .With(x => x.AgreementsData, _fixture.CreateMany<AgreementsData>(20))
            .With(x => x.CompanyIdentifiers, Enumerable.Repeat(new ValueTuple<UniqueIdentifierId, string>(identifierIdType, companyUniqueIds), 1))
            .Create();
        A.CallTo(() => _applicationRepository.GetCompanyUserRoleWithAddressUntrackedAsync(A<Guid>._, A<IEnumerable<DocumentTypeId>>._))
            .Returns(data);

        // Act
        var result = await _logic.GetCompanyWithAddressAsync(applicationId);

        // Assert
        A.CallTo(() => _applicationRepository.GetCompanyUserRoleWithAddressUntrackedAsync(applicationId, _options.Value.DocumentTypeIds)).MustHaveHappenedOnceExactly();
        result.Should().BeOfType<CompanyWithAddressData>();
        result.Should().Match<CompanyWithAddressData>(r =>
            r.CompanyId == data.CompanyId &&
            r.Name == data.Name &&
            r.ShortName == data.Shortname &&
            r.BusinessPartnerNumber == data.BusinessPartnerNumber &&
            r.City == data.City &&
            r.StreetName == data.StreetName &&
            r.CountryAlpha2Code == data.CountryAlpha2Code &&
            r.Region == data.Region &&
            r.StreetAdditional == data.Streetadditional &&
            r.StreetNumber == data.Streetnumber &&
            r.ZipCode == data.Zipcode
        );
        result.AgreementsRoleData.Should().HaveSameCount(data.AgreementsData.DistinctBy(ad => ad.CompanyRoleId));
        result.InvitedUserData.Should().HaveSameCount(data.InvitedCompanyUserData);
        result.UniqueIds.Should().HaveSameCount(data.CompanyIdentifiers);
        result.UniqueIds.Should().AllSatisfy(u => u.Should().Match<CompanyUniqueIdData>(u => u.UniqueIdentifierId == identifierIdType && u.Value == companyUniqueIds));
    }

    [Fact]
    public async Task GetCompanyWithAddressAsync_WithDefaultRequest_GetsExpectedResult_DefaultValues()
    {
        // Arrange
        var applicationId = _fixture.Create<Guid>();
        var data = _fixture.Build<CompanyUserRoleWithAddress>()
            .With(x => x.Shortname, default(string?))
            .With(x => x.BusinessPartnerNumber, default(string?))
            .With(x => x.City, default(string?))
            .With(x => x.StreetName, default(string?))
            .With(x => x.CountryAlpha2Code, default(string?))
            .With(x => x.Region, default(string?))
            .With(x => x.Streetadditional, default(string?))
            .With(x => x.Streetnumber, default(string?))
            .With(x => x.Zipcode, default(string?))
            .With(x => x.CountryDe, default(string?))
            .With(x => x.InvitedCompanyUserData, _fixture.CreateMany<Guid>().Select(id => new InvitedCompanyUserData(id, null, null, null)))
            .Create();
        A.CallTo(() => _applicationRepository.GetCompanyUserRoleWithAddressUntrackedAsync(A<Guid>._, A<IEnumerable<DocumentTypeId>>._))
            .Returns(data);

        // Act
        var result = await _logic.GetCompanyWithAddressAsync(applicationId);

        // Assert
        A.CallTo(() => _applicationRepository.GetCompanyUserRoleWithAddressUntrackedAsync(applicationId, _options.Value.DocumentTypeIds)).MustHaveHappenedOnceExactly();
        result.Should().BeOfType<CompanyWithAddressData>();
        result.Should().Match<CompanyWithAddressData>(r =>
            r.CompanyId == data.CompanyId &&
            r.Name == data.Name &&
            r.ShortName == "" &&
            r.BusinessPartnerNumber == "" &&
            r.City == "" &&
            r.StreetName == "" &&
            r.CountryAlpha2Code == "" &&
            r.Region == "" &&
            r.StreetAdditional == "" &&
            r.StreetNumber == "" &&
            r.ZipCode == ""
        );
        result.InvitedUserData.Should().HaveSameCount(data.InvitedCompanyUserData)
            .And.AllSatisfy(u => u.Should().Match<InvitedUserData>(u => u.FirstName == "" && u.LastName == "" && u.Email == ""));
    }

    #endregion

    #region UpdateCompanyBpn

    [Fact]
    public async Task UpdateCompanyBpnAsync_WithInvalidBpn_ThrowsControllerArgumentException()
    {
        // Arrange
        var bpn = "123";

        // Act
        Task Act() => _logic.UpdateCompanyBpn(IdWithBpn, bpn);

        // Assert
        var ex = await Assert.ThrowsAsync<ControllerArgumentException>(Act);
        ex.Parameters.First().Name.Should().Be("bpn");
        ex.Message.Should().Be(AdministrationRegistrationErrors.REGISTRATION_ARGUMENT_BPN_MUST_SIXTEEN_CHAR_LONG.ToString());
    }

    [Fact]
    public async Task UpdateCompanyBpnAsync_WithNotExistingApplication_ThrowsNotFoundException()
    {
        // Arrange
        SetupForUpdateCompanyBpn();

        // Act
        Task Act() => _logic.UpdateCompanyBpn(NotExistingApplicationId, ValidBpn);

        // Assert
        var ex = await Assert.ThrowsAsync<NotFoundException>(Act);
        ex.Message.Should().Be(AdministrationRegistrationErrors.REGISTRATION_NOT_APPLICATION_FOUND.ToString());
    }

    [Fact]
    public async Task UpdateCompanyBpnAsync_WithAlreadyTakenBpn_ThrowsConflictException()
    {
        // Arrange
        SetupForUpdateCompanyBpn();

        // Act
        Task Act() => _logic.UpdateCompanyBpn(IdWithoutBpn, AlreadyTakenBpn);

        // Assert
        var ex = await Assert.ThrowsAsync<ConflictException>(Act);
        ex.Message.Should().Be(AdministrationRegistrationErrors.REGISTRATION_BPN_ASSIGN_TO_OTHER_COMP.ToString());
    }

    [Fact]
    public async Task UpdateCompanyBpnAsync_WithActiveCompanyForApplication_ThrowsConflictException()
    {
        // Arrange
        SetupForUpdateCompanyBpn();

        // Act
        Task Act() => _logic.UpdateCompanyBpn(ActiveApplicationCompanyId, ValidBpn);

        // Assert
        var ex = await Assert.ThrowsAsync<ConflictException>(Act);
        ex.Message.Should().Be(AdministrationRegistrationErrors.REGISTRATION_CONFLICT_APPLICATION_FOR_COMPANY_NOT_PENDING.ToString());
    }

    [Fact]
    public async Task UpdateCompanyBpnAsync_WithBpnAlreadySet_ThrowsConflictException()
    {
        // Arrange
        SetupForUpdateCompanyBpn();

        // Act
        Task Act() => _logic.UpdateCompanyBpn(IdWithBpn, ValidBpn);

        // Assert
        var ex = await Assert.ThrowsAsync<ConflictException>(Act);
        ex.Message.Should().Be(AdministrationRegistrationErrors.REGISTRATION_CONFLICT_BPN_OF_COMPANY_SET.ToString());
    }

    [Theory]
    [InlineData(true, ProcessStepTypeId.CREATE_DIM_WALLET)]
    [InlineData(false, ProcessStepTypeId.CREATE_IDENTITY_WALLET)]
    public async Task UpdateCompanyBpnAsync_WithUseDimWalletFalse_CallsExpected(bool useDimWallet, ProcessStepTypeId expectedProcessStepTypeId)
    {
        // Arrange
        var options = Options.Create(new RegistrationSettings { UseDimWallet = useDimWallet });
        A.CallTo(() => _options.Value).Returns(new RegistrationSettings { UseDimWallet = useDimWallet });
        var entry = new ApplicationChecklistEntry(IdWithoutBpn, ApplicationChecklistEntryTypeId.BUSINESS_PARTNER_NUMBER, ApplicationChecklistEntryStatusId.TO_DO, DateTimeOffset.UtcNow);
        SetupForUpdateCompanyBpn(entry);
        var logic = new RegistrationBusinessLogic(_portalRepositories, options, _checklistService, null!, null!, _dimBusinessLogic, null!, _provisioningManager, null!, null!);

        // Act
        await logic.UpdateCompanyBpn(IdWithoutBpn, ValidBpn);

        // Assert
        A.CallTo(() => _companyRepository.AttachAndModifyCompany(CompanyId, null, A<Action<Company>>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _portalRepositories.SaveAsync()).MustHaveHappenedOnceExactly();
        entry.ApplicationChecklistEntryStatusId.Should().Be(ApplicationChecklistEntryStatusId.DONE);
        A.CallTo(() => _checklistService.FinalizeChecklistEntryAndProcessSteps(A<IApplicationChecklistService.ManualChecklistProcessStepData>._, null, A<Action<ApplicationChecklistEntry>>._, A<IEnumerable<ProcessStepTypeId>>.That.Matches(x => x.Count() == 1 && x.Single() == expectedProcessStepTypeId)))
            .MustHaveHappenedOnceExactly();
    }

    #endregion

    #region ProcessClearinghouseResponse

    [Fact]
    public async Task ProcessClearinghouseResponseAsync_WithValidData_CallsExpected()
    {
        // Arrange
        A.CallTo(() => _applicationRepository.GetSubmittedApplicationIdsByBpn(BusinessPartnerNumber))
            .Returns(Enumerable.Repeat(ApplicationId, 1).ToAsyncEnumerable());

        // Act
        var validationUnits = new List<ValidationUnits> {
            new (ClearinghouseResponseStatus.VALID, "vatId", null)
        }.AsEnumerable();
        var data = new ClearinghouseResponseData("COMPLETED", validationUnits);
        await _logic.ProcessClearinghouseResponseAsync(data, BusinessPartnerNumber, CancellationToken.None);

        // Assert
        A.CallTo(() => _clearinghouseBusinessLogic.ProcessEndClearinghouse(ApplicationId, data, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ProcessClearinghouseResponseAsync_WithMultipleApplications_ThrowsConflictException()
    {
        // Arrange
        A.CallTo(() => _applicationRepository.GetSubmittedApplicationIdsByBpn(BusinessPartnerNumber))
            .Returns(new[] { CompanyId, Guid.NewGuid() }.ToAsyncEnumerable());

        // Act
        var validationUnits = new List<ValidationUnits> {
            new (ClearinghouseResponseStatus.VALID, "vatId", null)
        }.AsEnumerable();
        var data = new ClearinghouseResponseData("COMPLETED", validationUnits);
        Task Act() => _logic.ProcessClearinghouseResponseAsync(data, BusinessPartnerNumber, CancellationToken.None);

        // Assert
        var ex = await Assert.ThrowsAsync<ConflictException>(Act);
        ex.Message.Should().Be(AdministrationRegistrationErrors.REGISTRATION_CONFLICT_APP_STATUS_STATUS_SUBMIT_FOUND_BPN.ToString());
    }

    [Fact]
    public async Task ProcessClearinghouseResponseAsync_WithNoApplication_ThrowsNotFoundException()
    {
        // Arrange
        A.CallTo(() => _applicationRepository.GetSubmittedApplicationIdsByBpn(BusinessPartnerNumber))
            .Returns(Enumerable.Empty<Guid>().ToAsyncEnumerable());

        // Act
        var validationUnits = new List<ValidationUnits> {
            new (ClearinghouseResponseStatus.VALID, "vatId", null)
        }.AsEnumerable();
        var data = new ClearinghouseResponseData("COMPLETED", validationUnits);
        Task Act() => _logic.ProcessClearinghouseResponseAsync(data, BusinessPartnerNumber, CancellationToken.None);

        // Assert
        var ex = await Assert.ThrowsAsync<NotFoundException>(Act);
        ex.Message.Should().Be(AdministrationRegistrationErrors.REGISTRATION_NOT_COMP_APP_BPN_STATUS_SUBMIT.ToString());
    }

    #endregion

    #region SetRegistrationVerification

    [Theory]
    [InlineData(true, ProcessStepTypeId.CREATE_DIM_WALLET)]
    [InlineData(false, ProcessStepTypeId.CREATE_IDENTITY_WALLET)]
    public async Task SetRegistrationVerification_WithApproval_CallsExpected(bool useDimWallet, ProcessStepTypeId expectedTypeId)
    {
        // Arrange
        var options = Options.Create(new RegistrationSettings { UseDimWallet = useDimWallet });
        var logic = new RegistrationBusinessLogic(_portalRepositories, options, _checklistService, null!, null!, _dimBusinessLogic, null!, null!, null!, null!);
        var entry = new ApplicationChecklistEntry(IdWithBpn, ApplicationChecklistEntryTypeId.REGISTRATION_VERIFICATION, ApplicationChecklistEntryStatusId.TO_DO, DateTimeOffset.UtcNow);
        SetupForApproveRegistrationVerification(entry);

        // Act
        await logic.ApproveRegistrationVerification(IdWithBpn);

        // Assert
        A.CallTo(() => _portalRepositories.SaveAsync()).MustHaveHappenedOnceExactly();
        entry.Comment.Should().BeNull();
        entry.ApplicationChecklistEntryStatusId.Should().Be(ApplicationChecklistEntryStatusId.DONE);

        A.CallTo(() => _mailingProcessCreation.CreateMailProcess(A<string>._, A<string>._, A<IReadOnlyDictionary<string, string>>._))
            .MustNotHaveHappened();
        A.CallTo(() => _checklistService.FinalizeChecklistEntryAndProcessSteps(
            A<IApplicationChecklistService.ManualChecklistProcessStepData>._,
            null,
            A<Action<ApplicationChecklistEntry>>._,
            A<IEnumerable<ProcessStepTypeId>>.That.Matches(x => x.Count(y => y == expectedTypeId) == 1)))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task SetRegistrationVerification_WithBpnNotDone_CallsExpected()
    {
        // Arrange
        var entry = new ApplicationChecklistEntry(IdWithoutBpn, ApplicationChecklistEntryTypeId.REGISTRATION_VERIFICATION, ApplicationChecklistEntryStatusId.TO_DO, DateTimeOffset.UtcNow);
        SetupForApproveRegistrationVerification(entry);

        // Act
        await _logic.ApproveRegistrationVerification(IdWithoutBpn);

        // Assert
        A.CallTo(() => _portalRepositories.SaveAsync()).MustHaveHappenedOnceExactly();
        entry.Comment.Should().BeNull();
        entry.ApplicationChecklistEntryStatusId.Should().Be(ApplicationChecklistEntryStatusId.DONE);
        A.CallTo(() => _mailingProcessCreation.CreateMailProcess(A<string>._, A<string>._, A<IReadOnlyDictionary<string, string>>._))
            .MustNotHaveHappened();
        A.CallTo(() => _checklistService.FinalizeChecklistEntryAndProcessSteps(A<IApplicationChecklistService.ManualChecklistProcessStepData>._, null, A<Action<ApplicationChecklistEntry>>._, null)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [InlineData(ApplicationChecklistEntryStatusId.TO_DO, IdentityProviderTypeId.SHARED)]
    [InlineData(ApplicationChecklistEntryStatusId.DONE, IdentityProviderTypeId.SHARED)]
    [InlineData(ApplicationChecklistEntryStatusId.TO_DO, IdentityProviderTypeId.OWN)]
    [InlineData(ApplicationChecklistEntryStatusId.DONE, IdentityProviderTypeId.OWN)]
    public async Task DeclineRegistrationVerification_WithDecline_StateAndCommentSetCorrectly(ApplicationChecklistEntryStatusId checklistStatusId, IdentityProviderTypeId idpTypeId)
    {
        // Arrange
        const string comment = "application rejected because of reasons.";
        var entry = new ApplicationChecklistEntry(IdWithBpn, ApplicationChecklistEntryTypeId.REGISTRATION_VERIFICATION, checklistStatusId, DateTimeOffset.UtcNow);
        var company = new Company(CompanyId, null!, CompanyStatusId.PENDING, DateTimeOffset.UtcNow);
        var application = new CompanyApplication(ApplicationId, company.Id, CompanyApplicationStatusId.SUBMITTED, CompanyApplicationTypeId.INTERNAL, DateTimeOffset.UtcNow);
        SetupForDeclineRegistrationVerification(entry, application, company, checklistStatusId, idpTypeId);

        // Act
        await _logic.DeclineRegistrationVerification(IdWithBpn, comment, CancellationToken.None);

        // Assert
        A.CallTo(() => _portalRepositories.SaveAsync()).MustHaveHappenedOnceExactly();
        entry.Comment.Should().Be(comment);
        entry.ApplicationChecklistEntryStatusId.Should().Be(ApplicationChecklistEntryStatusId.FAILED);
        company.CompanyStatusId.Should().Be(CompanyStatusId.REJECTED);
        application.ApplicationStatusId.Should().Be(CompanyApplicationStatusId.DECLINED);
        A.CallTo(() => _checklistService.SkipProcessSteps(
                A<IApplicationChecklistService.ManualChecklistProcessStepData>._,
                A<IEnumerable<ProcessStepTypeId>>.That.Matches(x => x.Single() == ProcessStepTypeId.MANUAL_VERIFY_REGISTRATION)))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _checklistService.FinalizeChecklistEntryAndProcessSteps(
                A<IApplicationChecklistService.ManualChecklistProcessStepData>._,
                A<Action<ApplicationChecklistEntry>>._,
                A<Action<ApplicationChecklistEntry>>._,
                A<IEnumerable<ProcessStepTypeId>?>.That.Matches(x => x != null && x.Count() == 1 && x.Single() == ProcessStepTypeId.TRIGGER_CALLBACK_OSP_DECLINED)))
            .MustHaveHappenedOnceExactly();
        if (idpTypeId == IdentityProviderTypeId.SHARED)
        {
            A.CallTo(() => _provisioningManager.DeleteSharedIdpRealmAsync(IamAliasId))
            .MustHaveHappenedOnceExactly();
        }

        A.CallTo(() => _provisioningManager.DeleteCentralIdentityProviderAsync(IamAliasId))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _provisioningManager.DeleteCentralRealmUserAsync("user123"))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task DeclineRegistrationVerification_WithApplicationNotFound_ThrowsArgumentException()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        A.CallTo(() => _applicationRepository.GetCompanyIdNameForSubmittedApplication(applicationId))
            .Returns<(Guid, string, Guid?, IEnumerable<(Guid, string, IdentityProviderTypeId, IEnumerable<Guid>)>, IEnumerable<Guid>)>(default);
        Task Act() => _logic.DeclineRegistrationVerification(applicationId, "test", CancellationToken.None);

        // Act
        var ex = await Assert.ThrowsAsync<ControllerArgumentException>(Act);

        // Assert
        ex.Message.Should().Be(AdministrationRegistrationErrors.REGISTRATION_ARGUMENT_COMP_APP_STATUS_NOTSUBMITTED.ToString());
    }

    [Fact]
    public async Task DeclineRegistrationVerification_WithNoComment_ThrowsConflictException()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        A.CallTo(() => _applicationRepository.GetCompanyIdNameForSubmittedApplication(applicationId))
            .Returns<(Guid, string, Guid?, IEnumerable<(Guid, string, IdentityProviderTypeId, IEnumerable<Guid>)>, IEnumerable<Guid>)>(default);
        Task Act() => _logic.DeclineRegistrationVerification(applicationId, "", CancellationToken.None);

        // Act
        var ex = await Assert.ThrowsAsync<ConflictException>(Act);

        // Assert
        ex.Message.Should().Be(AdministrationRegistrationErrors.REGISTRATION_CONFLICT_COMMENT_NOT_SET.ToString());
    }

    [Fact]
    public async Task DeclineRegistrationVerification_WithMultipleIdps_CallsExpected()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var sharedIdpId = Guid.NewGuid();
        var managedIdpId = Guid.NewGuid();
        var ownIdpId = Guid.NewGuid();
        var user1 = Guid.NewGuid();
        var user2 = Guid.NewGuid();
        var user3 = Guid.NewGuid();

        A.CallTo(() => _applicationRepository.GetCompanyIdNameForSubmittedApplication(applicationId))
            .Returns((
                companyId,
                "test",
                null,
                new[]
                {
                    (sharedIdpId, "idp1", IdentityProviderTypeId.SHARED, new[] { user1 }.AsEnumerable()),
                    (managedIdpId, "idp2", IdentityProviderTypeId.MANAGED, new[] { user2 }.AsEnumerable()),
                    (ownIdpId, "idp3", IdentityProviderTypeId.OWN, new[] { user3 }.AsEnumerable()),
                },
                new[]
                {
                    user1,
                    user2,
                    user3
                }));

        // Act
        await _logic.DeclineRegistrationVerification(applicationId, "test", CancellationToken.None);

        // Assert
        A.CallTo(() => _identityProviderRepository.DeleteCompanyIdentityProvider(companyId, sharedIdpId)).MustNotHaveHappened();
        A.CallTo(() => _identityProviderRepository.DeleteIamIdentityProvider("idp1")).MustHaveHappenedOnceExactly();
        A.CallTo(() => _identityProviderRepository.DeleteIdentityProvider(sharedIdpId)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _provisioningManager.DeleteSharedIdpRealmAsync("idp1")).MustHaveHappenedOnceExactly();
        A.CallTo(() => _provisioningManager.DeleteCentralIdentityProviderAsync("idp1")).MustHaveHappenedOnceExactly();

        A.CallTo(() => _identityProviderRepository.DeleteCompanyIdentityProvider(companyId, managedIdpId)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _identityProviderRepository.DeleteIamIdentityProvider("idp2")).MustNotHaveHappened();
        A.CallTo(() => _identityProviderRepository.DeleteIdentityProvider(managedIdpId)).MustNotHaveHappened();
        A.CallTo(() => _provisioningManager.DeleteSharedIdpRealmAsync("idp2")).MustNotHaveHappened();
        A.CallTo(() => _provisioningManager.DeleteCentralIdentityProviderAsync("idp2")).MustNotHaveHappened();

        A.CallTo(() => _identityProviderRepository.DeleteCompanyIdentityProvider(companyId, ownIdpId)).MustNotHaveHappened();
        A.CallTo(() => _identityProviderRepository.DeleteIamIdentityProvider("idp3")).MustHaveHappenedOnceExactly();
        A.CallTo(() => _identityProviderRepository.DeleteIdentityProvider(ownIdpId)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _provisioningManager.DeleteSharedIdpRealmAsync("idp3")).MustNotHaveHappened();
        A.CallTo(() => _provisioningManager.DeleteCentralIdentityProviderAsync("idp3")).MustHaveHappenedOnceExactly();

        A.CallTo(() => _userRepository.RemoveCompanyUserAssignedIdentityProviders(A<IEnumerable<(Guid, Guid)>>.That.IsSameSequenceAs(new[] { new ValueTuple<Guid, Guid>(user1, sharedIdpId) }))).MustNotHaveHappened();
        A.CallTo(() => _userRepository.RemoveCompanyUserAssignedIdentityProviders(A<IEnumerable<(Guid, Guid)>>.That.IsSameSequenceAs(new[] { new ValueTuple<Guid, Guid>(user2, managedIdpId) }))).MustHaveHappenedOnceExactly();
        A.CallTo(() => _userRepository.RemoveCompanyUserAssignedIdentityProviders(A<IEnumerable<(Guid, Guid)>>.That.IsSameSequenceAs(new[] { new ValueTuple<Guid, Guid>(user3, ownIdpId) }))).MustNotHaveHappened();
    }

    #endregion

    #region GetChecklistForApplicationAsync

    [Fact]
    public async Task GetChecklistForApplicationAsync_WithNotExistingApplication_ThrowsNotFoundException()
    {
        // Arrange
        var applicationId = _fixture.Create<Guid>();
        A.CallTo(() => _applicationRepository.GetApplicationChecklistData(applicationId, A<IEnumerable<ProcessStepTypeId>>._))
            .Returns<(bool, IEnumerable<(ApplicationChecklistEntryTypeId, ApplicationChecklistEntryStatusId, string?)>, IEnumerable<ProcessStepTypeId>)>(default);

        //Act
        Task Act() => _logic.GetChecklistForApplicationAsync(applicationId);

        // Assert
        var ex = await Assert.ThrowsAsync<NotFoundException>(Act);
        ex.Message.Should().Be(AdministrationRegistrationErrors.APPLICATION_NOT_FOUND.ToString());
    }

    [Fact]
    public async Task GetChecklistForApplicationAsync_WithValidApplication_ReturnsExpected()
    {
        // Arrange
        var applicationId = _fixture.Create<Guid>();
        var list = new (ApplicationChecklistEntryTypeId, ApplicationChecklistEntryStatusId, string?)[]
        {
            new(ApplicationChecklistEntryTypeId.REGISTRATION_VERIFICATION, ApplicationChecklistEntryStatusId.DONE, null),
            new(ApplicationChecklistEntryTypeId.BUSINESS_PARTNER_NUMBER, ApplicationChecklistEntryStatusId.DONE, null),
            new(ApplicationChecklistEntryTypeId.IDENTITY_WALLET, ApplicationChecklistEntryStatusId.FAILED, "error occured"),
            new(ApplicationChecklistEntryTypeId.SELF_DESCRIPTION_LP, ApplicationChecklistEntryStatusId.IN_PROGRESS, null),
            new(ApplicationChecklistEntryTypeId.CLEARING_HOUSE, ApplicationChecklistEntryStatusId.IN_PROGRESS, null),
        };
        var processSteps = new ProcessStepTypeId[]
        {
            ProcessStepTypeId.RETRIGGER_IDENTITY_WALLET
        };
        A.CallTo(() => _applicationRepository.GetApplicationChecklistData(applicationId, A<IEnumerable<ProcessStepTypeId>>._))
            .Returns((true, list, processSteps));

        //Act
        var result = await _logic.GetChecklistForApplicationAsync(applicationId);

        // Assert
        result.Should().NotBeNull().And.NotBeEmpty().And.HaveCount(5);
        result.Where(x => x.RetriggerableProcessSteps.Any()).Should().HaveCount(1);
        result.Where(x => x.Status == ApplicationChecklistEntryStatusId.FAILED).Should().ContainSingle();
    }

    #endregion

    #region TriggerChecklistAsync

    [Fact]
    public async Task TriggerChecklistAsync_WithFailingChecklistServiceCall_ReturnsError()
    {
        // Arrange
        var applicationId = _fixture.Create<Guid>();

        A.CallTo(() => _checklistService.VerifyChecklistEntryAndProcessSteps(applicationId,
            ApplicationChecklistEntryTypeId.CLEARING_HOUSE,
            A<IEnumerable<ApplicationChecklistEntryStatusId>>._,
            A<ProcessStepTypeId>._,
            null,
            A<IEnumerable<ProcessStepTypeId>>._))
            .Throws(new ConflictException("Test"));

        //Act
        Task Act() => _logic.TriggerChecklistAsync(applicationId, ApplicationChecklistEntryTypeId.CLEARING_HOUSE, ProcessStepTypeId.RETRIGGER_CLEARING_HOUSE);

        // Assert
        var ex = await Assert.ThrowsAsync<ConflictException>(Act);
        ex.Message.Should().Be("Test");
    }

    [Theory]
    [InlineData(ApplicationChecklistEntryTypeId.CLEARING_HOUSE, ProcessStepTypeId.RETRIGGER_CLEARING_HOUSE, ProcessStepTypeId.START_CLEARING_HOUSE, ApplicationChecklistEntryStatusId.TO_DO)]
    [InlineData(ApplicationChecklistEntryTypeId.IDENTITY_WALLET, ProcessStepTypeId.RETRIGGER_IDENTITY_WALLET, ProcessStepTypeId.CREATE_IDENTITY_WALLET, ApplicationChecklistEntryStatusId.TO_DO)]
    [InlineData(ApplicationChecklistEntryTypeId.IDENTITY_WALLET, ProcessStepTypeId.RETRIGGER_CREATE_DIM_WALLET, ProcessStepTypeId.CREATE_DIM_WALLET, ApplicationChecklistEntryStatusId.TO_DO)]
    [InlineData(ApplicationChecklistEntryTypeId.IDENTITY_WALLET, ProcessStepTypeId.RETRIGGER_VALIDATE_DID_DOCUMENT, ProcessStepTypeId.VALIDATE_DID_DOCUMENT, ApplicationChecklistEntryStatusId.TO_DO)]
    [InlineData(ApplicationChecklistEntryTypeId.SELF_DESCRIPTION_LP, ProcessStepTypeId.RETRIGGER_SELF_DESCRIPTION_LP, ProcessStepTypeId.START_SELF_DESCRIPTION_LP, ApplicationChecklistEntryStatusId.TO_DO)]
    [InlineData(ApplicationChecklistEntryTypeId.BUSINESS_PARTNER_NUMBER, ProcessStepTypeId.RETRIGGER_BUSINESS_PARTNER_NUMBER_PUSH, ProcessStepTypeId.CREATE_BUSINESS_PARTNER_NUMBER_PUSH, ApplicationChecklistEntryStatusId.TO_DO)]
    [InlineData(ApplicationChecklistEntryTypeId.BUSINESS_PARTNER_NUMBER, ProcessStepTypeId.RETRIGGER_BUSINESS_PARTNER_NUMBER_PULL, ProcessStepTypeId.CREATE_BUSINESS_PARTNER_NUMBER_PULL, ApplicationChecklistEntryStatusId.IN_PROGRESS)]
    [InlineData(ApplicationChecklistEntryTypeId.APPLICATION_ACTIVATION, ProcessStepTypeId.RETRIGGER_ASSIGN_BPN_TO_USERS, ProcessStepTypeId.ASSIGN_BPN_TO_USERS, ApplicationChecklistEntryStatusId.IN_PROGRESS)]
    [InlineData(ApplicationChecklistEntryTypeId.APPLICATION_ACTIVATION, ProcessStepTypeId.RETRIGGER_ASSIGN_INITIAL_ROLES, ProcessStepTypeId.ASSIGN_INITIAL_ROLES, ApplicationChecklistEntryStatusId.IN_PROGRESS)]
    [InlineData(ApplicationChecklistEntryTypeId.APPLICATION_ACTIVATION, ProcessStepTypeId.RETRIGGER_REMOVE_REGISTRATION_ROLES, ProcessStepTypeId.REMOVE_REGISTRATION_ROLES, ApplicationChecklistEntryStatusId.IN_PROGRESS)]
    [InlineData(ApplicationChecklistEntryTypeId.APPLICATION_ACTIVATION, ProcessStepTypeId.RETRIGGER_SET_THEME, ProcessStepTypeId.SET_THEME, ApplicationChecklistEntryStatusId.IN_PROGRESS)]
    [InlineData(ApplicationChecklistEntryTypeId.APPLICATION_ACTIVATION, ProcessStepTypeId.RETRIGGER_SET_MEMBERSHIP, ProcessStepTypeId.SET_MEMBERSHIP, ApplicationChecklistEntryStatusId.IN_PROGRESS)]
    [InlineData(ApplicationChecklistEntryTypeId.APPLICATION_ACTIVATION, ProcessStepTypeId.RETRIGGER_SET_CX_MEMBERSHIP_IN_BPDM, ProcessStepTypeId.SET_CX_MEMBERSHIP_IN_BPDM, ApplicationChecklistEntryStatusId.IN_PROGRESS)]
    public async Task TriggerChecklistAsync_WithValidData_ReturnsExpected(ApplicationChecklistEntryTypeId typeId, ProcessStepTypeId stepId, ProcessStepTypeId nextStepId, ApplicationChecklistEntryStatusId statusId)
    {
        // Arrange
        var checklistEntry = new ApplicationChecklistEntry(Guid.NewGuid(), typeId,
            ApplicationChecklistEntryStatusId.FAILED, DateTimeOffset.UtcNow)
        {
            Comment = "test"
        };
        var applicationId = _fixture.Create<Guid>();
        var context = new IApplicationChecklistService.ManualChecklistProcessStepData(
            applicationId,
            new Process(Guid.NewGuid(), ProcessTypeId.APPLICATION_CHECKLIST, Guid.NewGuid()),
            Guid.NewGuid(),
            typeId,
            ImmutableDictionary.Create<ApplicationChecklistEntryTypeId, (ApplicationChecklistEntryStatusId, string?)>(),
            Enumerable.Empty<ProcessStep<Process, ProcessTypeId, ProcessStepTypeId>>());
        A.CallTo(() => _checklistService.VerifyChecklistEntryAndProcessSteps(applicationId,
                typeId,
                A<IEnumerable<ApplicationChecklistEntryStatusId>>._,
                stepId,
                null,
                A<IEnumerable<ProcessStepTypeId>>._))
            .Returns(context);
        A.CallTo(() => _checklistService.FinalizeChecklistEntryAndProcessSteps(
                A<IApplicationChecklistService.ManualChecklistProcessStepData>._, A<Action<ApplicationChecklistEntry>>._, A<Action<ApplicationChecklistEntry>>._,
                A<IEnumerable<ProcessStepTypeId>>._))
            .Invokes((IApplicationChecklistService.ManualChecklistProcessStepData _, Action<ApplicationChecklistEntry> initial, Action<ApplicationChecklistEntry> modify, IEnumerable<ProcessStepTypeId> _) =>
            {
                initial(checklistEntry);
                modify(checklistEntry);
            });

        //Act

        var settings = A.Fake<RegistrationSettings>();
        A.CallTo(() => _options.Value).Returns(settings);
        var logic = new RegistrationBusinessLogic(_portalRepositories, _options, _checklistService, _clearinghouseBusinessLogic, _sdFactoryBusinessLogic, _dimBusinessLogic, _issuerComponentBusinessLogic, _provisioningManager, _mailingProcessCreation, _logger);
        await logic.TriggerChecklistAsync(applicationId, typeId, stepId);

        // Assert
        A.CallTo(() => _checklistService.FinalizeChecklistEntryAndProcessSteps(context,
                A<Action<ApplicationChecklistEntry>>._,
                A<Action<ApplicationChecklistEntry>>._,
                A<IEnumerable<ProcessStepTypeId>>.That.IsSameSequenceAs(new[] { nextStepId })))
            .MustHaveHappenedOnceExactly();
        checklistEntry.ApplicationChecklistEntryStatusId.Should().Be(statusId);
        checklistEntry.Comment.Should().BeNull();
        A.CallTo(() => _portalRepositories.SaveAsync()).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [InlineData(ApplicationChecklistEntryTypeId.REGISTRATION_VERIFICATION, ProcessStepTypeId.RETRIGGER_BUSINESS_PARTNER_NUMBER_PUSH)]
    [InlineData(ApplicationChecklistEntryTypeId.REGISTRATION_VERIFICATION, ProcessStepTypeId.RETRIGGER_IDENTITY_WALLET)]
    [InlineData(ApplicationChecklistEntryTypeId.REGISTRATION_VERIFICATION, ProcessStepTypeId.RETRIGGER_SELF_DESCRIPTION_LP)]
    [InlineData(ApplicationChecklistEntryTypeId.REGISTRATION_VERIFICATION, ProcessStepTypeId.RETRIGGER_BUSINESS_PARTNER_NUMBER_PULL)]
    [InlineData(ApplicationChecklistEntryTypeId.REGISTRATION_VERIFICATION, ProcessStepTypeId.RETRIGGER_CLEARING_HOUSE)]
    [InlineData(ApplicationChecklistEntryTypeId.APPLICATION_ACTIVATION, ProcessStepTypeId.RETRIGGER_BUSINESS_PARTNER_NUMBER_PUSH)]
    [InlineData(ApplicationChecklistEntryTypeId.APPLICATION_ACTIVATION, ProcessStepTypeId.RETRIGGER_IDENTITY_WALLET)]
    [InlineData(ApplicationChecklistEntryTypeId.APPLICATION_ACTIVATION, ProcessStepTypeId.RETRIGGER_SELF_DESCRIPTION_LP)]
    [InlineData(ApplicationChecklistEntryTypeId.APPLICATION_ACTIVATION, ProcessStepTypeId.RETRIGGER_BUSINESS_PARTNER_NUMBER_PULL)]
    [InlineData(ApplicationChecklistEntryTypeId.APPLICATION_ACTIVATION, ProcessStepTypeId.RETRIGGER_CLEARING_HOUSE)]
    [InlineData(ApplicationChecklistEntryTypeId.CLEARING_HOUSE, ProcessStepTypeId.RETRIGGER_BUSINESS_PARTNER_NUMBER_PUSH)]
    [InlineData(ApplicationChecklistEntryTypeId.CLEARING_HOUSE, ProcessStepTypeId.RETRIGGER_IDENTITY_WALLET)]
    [InlineData(ApplicationChecklistEntryTypeId.CLEARING_HOUSE, ProcessStepTypeId.RETRIGGER_SELF_DESCRIPTION_LP)]
    [InlineData(ApplicationChecklistEntryTypeId.CLEARING_HOUSE, ProcessStepTypeId.RETRIGGER_BUSINESS_PARTNER_NUMBER_PULL)]
    [InlineData(ApplicationChecklistEntryTypeId.IDENTITY_WALLET, ProcessStepTypeId.RETRIGGER_BUSINESS_PARTNER_NUMBER_PUSH)]
    [InlineData(ApplicationChecklistEntryTypeId.IDENTITY_WALLET, ProcessStepTypeId.RETRIGGER_CLEARING_HOUSE)]
    [InlineData(ApplicationChecklistEntryTypeId.IDENTITY_WALLET, ProcessStepTypeId.RETRIGGER_SELF_DESCRIPTION_LP)]
    [InlineData(ApplicationChecklistEntryTypeId.IDENTITY_WALLET, ProcessStepTypeId.RETRIGGER_BUSINESS_PARTNER_NUMBER_PULL)]
    [InlineData(ApplicationChecklistEntryTypeId.SELF_DESCRIPTION_LP, ProcessStepTypeId.RETRIGGER_BUSINESS_PARTNER_NUMBER_PUSH)]
    [InlineData(ApplicationChecklistEntryTypeId.SELF_DESCRIPTION_LP, ProcessStepTypeId.RETRIGGER_CLEARING_HOUSE)]
    [InlineData(ApplicationChecklistEntryTypeId.SELF_DESCRIPTION_LP, ProcessStepTypeId.RETRIGGER_BUSINESS_PARTNER_NUMBER_PULL)]
    [InlineData(ApplicationChecklistEntryTypeId.SELF_DESCRIPTION_LP, ProcessStepTypeId.RETRIGGER_IDENTITY_WALLET)]
    [InlineData(ApplicationChecklistEntryTypeId.BUSINESS_PARTNER_NUMBER, ProcessStepTypeId.RETRIGGER_CLEARING_HOUSE)]
    [InlineData(ApplicationChecklistEntryTypeId.BUSINESS_PARTNER_NUMBER, ProcessStepTypeId.RETRIGGER_SELF_DESCRIPTION_LP)]
    [InlineData(ApplicationChecklistEntryTypeId.BUSINESS_PARTNER_NUMBER, ProcessStepTypeId.RETRIGGER_IDENTITY_WALLET)]
    public async Task TriggerChecklistAsync_WithWrongProcessStepForChecklist_ThrowsConflictException(ApplicationChecklistEntryTypeId typeId, ProcessStepTypeId stepId)
    {
        // Arrange
        var applicationId = _fixture.Create<Guid>();

        //Act
        Task Act() => _logic.TriggerChecklistAsync(applicationId, typeId, stepId);

        // Assert
        var ex = await Assert.ThrowsAsync<ControllerArgumentException>(Act);
        ex.Message.Should().Be(AdministrationRegistrationErrors.REGISTRATION_ARGUMENT_PROCEES_TYPID_NOT_TRIGERABLE.ToString());
        A.CallTo(() => _checklistService.FinalizeChecklistEntryAndProcessSteps(A<IApplicationChecklistService.ManualChecklistProcessStepData>._, A<Action<ApplicationChecklistEntry>>._, A<Action<ApplicationChecklistEntry>>._, A<IEnumerable<ProcessStepTypeId>>._)).MustNotHaveHappened();
        A.CallTo(() => _portalRepositories.SaveAsync()).MustNotHaveHappened();
    }

    [Theory]
    [InlineData(ApplicationChecklistEntryTypeId.APPLICATION_ACTIVATION, ProcessStepTypeId.RETRIGGER_ASSIGN_BPN_TO_USERS)]
    [InlineData(ApplicationChecklistEntryTypeId.APPLICATION_ACTIVATION, ProcessStepTypeId.RETRIGGER_ASSIGN_INITIAL_ROLES)]
    [InlineData(ApplicationChecklistEntryTypeId.APPLICATION_ACTIVATION, ProcessStepTypeId.RETRIGGER_REMOVE_REGISTRATION_ROLES)]
    [InlineData(ApplicationChecklistEntryTypeId.APPLICATION_ACTIVATION, ProcessStepTypeId.RETRIGGER_SET_THEME)]
    [InlineData(ApplicationChecklistEntryTypeId.APPLICATION_ACTIVATION, ProcessStepTypeId.RETRIGGER_SET_MEMBERSHIP)]
    [InlineData(ApplicationChecklistEntryTypeId.APPLICATION_ACTIVATION, ProcessStepTypeId.RETRIGGER_SET_CX_MEMBERSHIP_IN_BPDM)]
    public async Task TriggerChecklistAsync_Success(ApplicationChecklistEntryTypeId typeId, ProcessStepTypeId stepId)
    {
        // Arrange
        var applicationId = _fixture.Create<Guid>();

        //Act
        await _logic.TriggerChecklistAsync(applicationId, typeId, stepId);

        // Assert
        A.CallTo(() => _checklistService.VerifyChecklistEntryAndProcessSteps(A<Guid>._, A<ApplicationChecklistEntryTypeId>._, A<IEnumerable<ApplicationChecklistEntryStatusId>>._, A<ProcessStepTypeId>._, A<IEnumerable<ApplicationChecklistEntryTypeId>>._, A<IEnumerable<ProcessStepTypeId>>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _checklistService.FinalizeChecklistEntryAndProcessSteps(A<IApplicationChecklistService.ManualChecklistProcessStepData>._, A<Action<ApplicationChecklistEntry>>._, A<Action<ApplicationChecklistEntry>>._, A<IEnumerable<ProcessStepTypeId>>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _portalRepositories.SaveAsync())
            .MustHaveHappenedOnceExactly();
    }

    #endregion

    #region ProcessClearinghouseSelfDescription

    [Fact]
    public async Task ProcessClearinghouseSelfDescription_WithValidData_CallsExpected()
    {
        // Arrange
        var data = new SelfDescriptionResponseData(ApplicationId, SelfDescriptionStatus.Confirm, null, "{ \"test\": true }");
        var companyId = Guid.NewGuid();
        A.CallTo(() => _companyRepository.IsExistingCompany(CompanyId))
            .Returns(false);
        A.CallTo(() => _applicationRepository.GetCompanyIdSubmissionStatusForApplication(ApplicationId))
            .Returns((true, companyId, true));

        // Act
        await _logic.ProcessClearinghouseSelfDescription(data, CancellationToken.None);

        // Assert
        A.CallTo(() => _sdFactoryBusinessLogic.ProcessFinishSelfDescriptionLpForApplication(data, companyId, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _sdFactoryBusinessLogic.ProcessFinishSelfDescriptionLpForCompany(A<SelfDescriptionResponseData>._, A<CancellationToken>._))
            .MustNotHaveHappened();
        A.CallTo(() => _portalRepositories.SaveAsync()).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ProcessClearinghouseSelfDescription_WithValidCompany_CallsExpected()
    {
        // Arrange
        var data = new SelfDescriptionResponseData(CompanyId, SelfDescriptionStatus.Confirm, null, "{ \"test\": true }");
        A.CallTo(() => _companyRepository.IsExistingCompany(CompanyId))
            .Returns(true);

        // Act
        await _logic.ProcessClearinghouseSelfDescription(data, CancellationToken.None);

        // Assert
        A.CallTo(() => _sdFactoryBusinessLogic.ProcessFinishSelfDescriptionLpForCompany(data, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _portalRepositories.SaveAsync()).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ProcessClearinghouseSelfDescription_WithNotExistingApplication_ThrowsNotFoundException()
    {
        // Arrange
        var data = new SelfDescriptionResponseData(ApplicationId, SelfDescriptionStatus.Confirm, null, "{ \"test\": true }");
        A.CallTo(() => _applicationRepository.GetCompanyIdSubmissionStatusForApplication(ApplicationId))
            .Returns<(bool, Guid, bool)>(default);
        A.CallTo(() => _companyRepository.IsExistingCompany(CompanyId))
            .Returns(false);

        // Act
        Task Act() => _logic.ProcessClearinghouseSelfDescription(data, CancellationToken.None);

        // Assert
        var ex = await Assert.ThrowsAsync<NotFoundException>(Act);
        ex.Message.Should().Be(AdministrationRegistrationErrors.REGISTRATION_NOT_COMPANY_EXTERNAL_APP_NOT_FOUND.ToString());
    }

    [Fact]
    public async Task ProcessClearinghouseSelfDescription_WithNotSubmittedApplication_ThrowsConflictException()
    {
        // Arrange
        var data = new SelfDescriptionResponseData(ApplicationId, SelfDescriptionStatus.Confirm, null, "{ \"test\": true }");
        A.CallTo(() => _applicationRepository.GetCompanyIdSubmissionStatusForApplication(ApplicationId))
            .Returns((true, Guid.NewGuid(), false));
        A.CallTo(() => _companyRepository.IsExistingCompany(CompanyId))
            .Returns(false);

        // Act
        Task Act() => _logic.ProcessClearinghouseSelfDescription(data, CancellationToken.None);

        // Assert
        var ex = await Assert.ThrowsAsync<ConflictException>(Act);
        ex.Message.Should().Be(AdministrationRegistrationErrors.REGISTRATION_NOT_COMPANY_EXTERNAL_NOT_STATUS_SUBMIT.ToString());
    }

    #endregion

    #region GetDocumentAsync

    [Fact]
    public async Task GetDocumentAsync_WithValidData_ReturnsExpected()
    {
        // Arrange
        var documentId = Guid.NewGuid();
        var content = new byte[7];
        A.CallTo(() => _documentRepository.GetDocumentByIdAsync(A<Guid>._, A<IEnumerable<DocumentTypeId>>._))
            .Returns(new Document(documentId, content, content, "test.pdf", MediaTypeId.JSON, DateTimeOffset.UtcNow, DocumentStatusId.LOCKED, DocumentTypeId.APP_CONTRACT, content.Length));

        // Act
        var result = await _logic.GetDocumentAsync(documentId);

        // Assert
        result.Should().NotBeNull();
        result.fileName.Should().Be("test.pdf");
        A.CallTo(() => _documentRepository.GetDocumentByIdAsync(documentId, A<IEnumerable<DocumentTypeId>>.That.IsSameSequenceAs(new[] { DocumentTypeId.COMMERCIAL_REGISTER_EXTRACT }))).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task GetDocumentAsync_WithNotExistingDocument_ThrowsNotFoundException()
    {
        // Arrange
        var documentId = Guid.NewGuid();
        A.CallTo(() => _documentRepository.GetDocumentByIdAsync(A<Guid>._, A<IEnumerable<DocumentTypeId>>._))
            .Returns<Document?>(null);

        // Act
        Task Act() => _logic.GetDocumentAsync(documentId);

        // Assert
        var ex = await Assert.ThrowsAsync<NotFoundException>(Act);
        ex.Message.Should().Be(AdministrationRegistrationErrors.REGISTRATION_NOT_DOC_NOT_EXIST.ToString());
        A.CallTo(() => _documentRepository.GetDocumentByIdAsync(documentId, A<IEnumerable<DocumentTypeId>>.That.IsSameSequenceAs(new[] { DocumentTypeId.COMMERCIAL_REGISTER_EXTRACT }))).MustHaveHappenedOnceExactly();
    }

    #endregion

    #region ProcessDimResponseAsync

    [Fact]
    public async Task ProcessDimResponseAsync_WithValidData_CallsExpected()
    {
        // Arrange
        var data = new DimWalletData($"did:web:test123:{BusinessPartnerNumber}", _fixture.Create<JsonDocument>(), new AuthenticationDetail("https://example.org", "test123", "test123"));

        // Act
        await _logic.ProcessDimResponseAsync(BusinessPartnerNumber, data, CancellationToken.None);

        // Assert
        A.CallTo(() => _dimBusinessLogic.ProcessDimResponse(BusinessPartnerNumber, data, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _portalRepositories.SaveAsync()).MustHaveHappenedOnceExactly();
    }

    #endregion

    #region ProcessIssuerBpnResponseAsync

    [Fact]
    public async Task ProcessIssuerBpnResponseAsync_WithValidData_CallsExpected()
    {
        // Arrange
        A.CallTo(() => _applicationRepository.GetSubmittedApplicationIdsByBpn(BusinessPartnerNumber))
            .Returns(Enumerable.Repeat(ApplicationId, 1).ToAsyncEnumerable());

        // Act
        var data = new IssuerResponseData(BusinessPartnerNumber, IssuerResponseStatus.SUCCESSFUL, "test Message");
        await _logic.ProcessIssuerBpnResponseAsync(data, CancellationToken.None);

        // Assert
        A.CallTo(() => _issuerComponentBusinessLogic.StoreBpnlCredentialResponse(ApplicationId, data))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ProcessIssuerBpnResponseAsync_WithMultipleApplications_ThrowsConflictException()
    {
        // Arrange
        A.CallTo(() => _applicationRepository.GetSubmittedApplicationIdsByBpn(BusinessPartnerNumber))
            .Returns(new[] { CompanyId, Guid.NewGuid() }.ToAsyncEnumerable());

        // Act
        var data = new IssuerResponseData(BusinessPartnerNumber, IssuerResponseStatus.SUCCESSFUL, "test Message");
        async Task Act() => await _logic.ProcessIssuerBpnResponseAsync(data, CancellationToken.None).ConfigureAwait(false);

        // Assert
        var ex = await Assert.ThrowsAsync<ConflictException>(Act);
        ex.Message.Should().Be(AdministrationRegistrationErrors.REGISTRATION_CONFLICT_APP_STATUS_STATUS_SUBMIT_FOUND_BPN.ToString());
    }

    [Fact]
    public async Task ProcessIssuerBpnResponseAsync_WithNoApplication_ThrowsNotFoundException()
    {
        // Arrange
        A.CallTo(() => _applicationRepository.GetSubmittedApplicationIdsByBpn(BusinessPartnerNumber))
            .Returns(Enumerable.Empty<Guid>().ToAsyncEnumerable());

        // Act
        var data = new IssuerResponseData(BusinessPartnerNumber, IssuerResponseStatus.SUCCESSFUL, "test Message");
        async Task Act() => await _logic.ProcessIssuerBpnResponseAsync(data, CancellationToken.None).ConfigureAwait(false);

        // Assert
        var ex = await Assert.ThrowsAsync<NotFoundException>(Act);
        ex.Message.Should().Be(AdministrationRegistrationErrors.REGISTRATION_NOT_COMP_APP_BPN_STATUS_SUBMIT.ToString());
    }

    #endregion

    #region ProcessIssuerMembershipResponseAsync

    [Fact]
    public async Task ProcessIssuerMembershipResponseAsync_WithValidData_CallsExpected()
    {
        // Arrange
        A.CallTo(() => _applicationRepository.GetSubmittedApplicationIdsByBpn(BusinessPartnerNumber))
            .Returns(Enumerable.Repeat(ApplicationId, 1).ToAsyncEnumerable());

        // Act
        var data = new IssuerResponseData(BusinessPartnerNumber, IssuerResponseStatus.SUCCESSFUL, "test Message");
        await _logic.ProcessIssuerMembershipResponseAsync(data, CancellationToken.None);

        // Assert
        A.CallTo(() => _issuerComponentBusinessLogic.StoreMembershipCredentialResponse(ApplicationId, data))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ProcessIssuerMembershipResponseAsync_WithMultipleApplications_ThrowsConflictException()
    {
        // Arrange
        A.CallTo(() => _applicationRepository.GetSubmittedApplicationIdsByBpn(BusinessPartnerNumber))
            .Returns(new[] { CompanyId, Guid.NewGuid() }.ToAsyncEnumerable());

        // Act
        var data = new IssuerResponseData(BusinessPartnerNumber, IssuerResponseStatus.SUCCESSFUL, "test Message");
        async Task Act() => await _logic.ProcessIssuerMembershipResponseAsync(data, CancellationToken.None).ConfigureAwait(false);

        // Assert
        var ex = await Assert.ThrowsAsync<ConflictException>(Act);
        ex.Message.Should().Be(AdministrationRegistrationErrors.REGISTRATION_CONFLICT_APP_STATUS_STATUS_SUBMIT_FOUND_BPN.ToString());
    }

    [Fact]
    public async Task ProcessIssuerMembershipResponseAsync_WithNoApplication_ThrowsNotFoundException()
    {
        // Arrange
        A.CallTo(() => _applicationRepository.GetSubmittedApplicationIdsByBpn(BusinessPartnerNumber))
            .Returns(Enumerable.Empty<Guid>().ToAsyncEnumerable());

        // Act
        var data = new IssuerResponseData(BusinessPartnerNumber, IssuerResponseStatus.SUCCESSFUL, "test Message");
        async Task Act() => await _logic.ProcessIssuerMembershipResponseAsync(data, CancellationToken.None).ConfigureAwait(false);

        // Assert
        var ex = await Assert.ThrowsAsync<NotFoundException>(Act);
        ex.Message.Should().Be(AdministrationRegistrationErrors.REGISTRATION_NOT_COMP_APP_BPN_STATUS_SUBMIT.ToString());
    }

    #endregion

    #region RetriggerProcessStepsForIdpDeletion

    [Fact]
    public async Task RetriggerDeleteIdpSharedRealm_CallsExpected()
    {
        // Arrange
        var stepToTrigger = ProcessStepTypeId.RETRIGGER_DELETE_IDP_SHARED_REALM;
        var processStepTypeId = ProcessStepTypeId.DELETE_IDP_SHARED_REALM;
        var processSteps = new List<ProcessStep<Process, ProcessTypeId, ProcessStepTypeId>>();
        var process = _fixture.Build<Process>().With(x => x.LockExpiryDate, default(DateTimeOffset?)).Create();
        var processStepId = Guid.NewGuid();
        SetupFakesForRetrigger(processSteps);
        var verifyProcessData = new VerifyProcessData<ProcessTypeId, ProcessStepTypeId>(process, Enumerable.Repeat(new ProcessStep<Process, ProcessTypeId, ProcessStepTypeId>(processStepId, stepToTrigger, ProcessStepStatusId.TODO, process.Id, DateTimeOffset.UtcNow), 1));
        A.CallTo(() => _processStepRepository.IsValidProcess(A<Guid>._, A<ProcessTypeId>._, A<IEnumerable<ProcessStepTypeId>>._))
            .Returns((true, verifyProcessData));

        // Act
        await _logic.RetriggerDeleteIdpSharedRealm(process.Id);

        // Assert
        processSteps.Should().ContainSingle().And.Satisfy(x => x.ProcessStepTypeId == processStepTypeId);
        A.CallTo(() => _processStepRepository.IsValidProcess(process.Id, ProcessTypeId.IDENTITYPROVIDER_PROVISIONING, A<IEnumerable<ProcessStepTypeId>>.That.Matches(x => x.Count() == 1 && x.Single() == stepToTrigger)))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _processStepRepository.AttachAndModifyProcessSteps(A<IEnumerable<(Guid ProcessStepId, Action<IProcessStep<ProcessStepTypeId>>? Initialize, Action<IProcessStep<ProcessStepTypeId>> Modify)>>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _portalRepositories.SaveAsync()).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task RetriggerDeleteIdpSharedRealm_WithNotExistingProcess_ThrowsException()
    {
        // Arrange
        var stepToTrigger = ProcessStepTypeId.RETRIGGER_DELETE_IDP_SHARED_REALM;
        var processId = Guid.NewGuid();
        A.CallTo(() => _processStepRepository.IsValidProcess(A<Guid>._, A<ProcessTypeId>._, A<IEnumerable<ProcessStepTypeId>>._))
            .Returns((false, _fixture.Create<VerifyProcessData<ProcessTypeId, ProcessStepTypeId>>()));

        Task Act() => _logic.RetriggerDeleteIdpSharedRealm(processId);

        // Act
        var ex = await Assert.ThrowsAsync<NotFoundException>(Act);

        // Assert
        ex.Message.Should().Be($"process {processId} does not exist");
        A.CallTo(() => _processStepRepository.IsValidProcess(processId, ProcessTypeId.IDENTITYPROVIDER_PROVISIONING, A<IEnumerable<ProcessStepTypeId>>.That.Matches(x => x.Count() == 1 && x.Single() == stepToTrigger)))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task RetriggerDeleteIdpSharedServiceAccount_CallsExpected()
    {
        // Arrange
        var stepToTrigger = ProcessStepTypeId.RETRIGGER_DELETE_IDP_SHARED_SERVICEACCOUNT;
        var processStepTypeId = ProcessStepTypeId.DELETE_IDP_SHARED_SERVICEACCOUNT;
        var processSteps = new List<ProcessStep<Process, ProcessTypeId, ProcessStepTypeId>>();
        var process = _fixture.Build<Process>().With(x => x.LockExpiryDate, default(DateTimeOffset?)).Create();
        var processStepId = Guid.NewGuid();
        SetupFakesForRetrigger(processSteps);
        var verifyProcessData = new VerifyProcessData<ProcessTypeId, ProcessStepTypeId>(process, Enumerable.Repeat(new ProcessStep<Process, ProcessTypeId, ProcessStepTypeId>(processStepId, stepToTrigger, ProcessStepStatusId.TODO, process.Id, DateTimeOffset.UtcNow), 1));
        A.CallTo(() => _processStepRepository.IsValidProcess(A<Guid>._, A<ProcessTypeId>._, A<IEnumerable<ProcessStepTypeId>>._))
            .Returns((true, verifyProcessData));

        // Act
        await _logic.RetriggerDeleteIdpSharedServiceAccount(process.Id);

        // Assert
        processSteps.Should().ContainSingle().And.Satisfy(x => x.ProcessStepTypeId == processStepTypeId);
        A.CallTo(() => _processStepRepository.IsValidProcess(process.Id, ProcessTypeId.IDENTITYPROVIDER_PROVISIONING, A<IEnumerable<ProcessStepTypeId>>.That.Matches(x => x.Count() == 1 && x.Single() == stepToTrigger)))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _processStepRepository.AttachAndModifyProcessSteps(A<IEnumerable<(Guid ProcessStepId, Action<IProcessStep<ProcessStepTypeId>>? Initialize, Action<IProcessStep<ProcessStepTypeId>> Modify)>>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _portalRepositories.SaveAsync()).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task RetriggerDeleteIdpSharedServiceAccount_WithNotExistingProcess_ThrowsException()
    {
        // Arrange
        var stepToTrigger = ProcessStepTypeId.RETRIGGER_DELETE_IDP_SHARED_SERVICEACCOUNT;
        var processId = Guid.NewGuid();
        A.CallTo(() => _processStepRepository.IsValidProcess(A<Guid>._, A<ProcessTypeId>._, A<IEnumerable<ProcessStepTypeId>>._))
            .Returns((false, _fixture.Create<VerifyProcessData<ProcessTypeId, ProcessStepTypeId>>()));

        Task Act() => _logic.RetriggerDeleteIdpSharedServiceAccount(processId);

        // Act
        var ex = await Assert.ThrowsAsync<NotFoundException>(Act);

        // Assert
        ex.Message.Should().Be($"process {processId} does not exist");
        A.CallTo(() => _processStepRepository.IsValidProcess(processId, ProcessTypeId.IDENTITYPROVIDER_PROVISIONING, A<IEnumerable<ProcessStepTypeId>>.That.Matches(x => x.Count() == 1 && x.Single() == stepToTrigger)))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task RetriggerDeleteCentralIdentityProvider_CallsExpected()
    {
        // Arrange
        var stepToTrigger = ProcessStepTypeId.RETRIGGER_DELETE_CENTRAL_IDENTITY_PROVIDER;
        var processStepTypeId = ProcessStepTypeId.DELETE_CENTRAL_IDENTITY_PROVIDER;
        var processSteps = new List<ProcessStep<Process, ProcessTypeId, ProcessStepTypeId>>();
        var process = _fixture.Build<Process>().With(x => x.LockExpiryDate, default(DateTimeOffset?)).Create();
        var processStepId = Guid.NewGuid();
        SetupFakesForRetrigger(processSteps);
        var verifyProcessData = new VerifyProcessData<ProcessTypeId, ProcessStepTypeId>(process, Enumerable.Repeat(new ProcessStep<Process, ProcessTypeId, ProcessStepTypeId>(processStepId, stepToTrigger, ProcessStepStatusId.TODO, process.Id, DateTimeOffset.UtcNow), 1));
        A.CallTo(() => _processStepRepository.IsValidProcess(A<Guid>._, A<ProcessTypeId>._, A<IEnumerable<ProcessStepTypeId>>._))
            .Returns((true, verifyProcessData));

        // Act
        await _logic.RetriggerDeleteCentralIdentityProvider(process.Id);

        // Assert
        processSteps.Should().ContainSingle().And.Satisfy(x => x.ProcessStepTypeId == processStepTypeId);
        A.CallTo(() => _processStepRepository.IsValidProcess(process.Id, ProcessTypeId.IDENTITYPROVIDER_PROVISIONING, A<IEnumerable<ProcessStepTypeId>>.That.Matches(x => x.Count() == 1 && x.Single() == stepToTrigger)))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _processStepRepository.AttachAndModifyProcessSteps(A<IEnumerable<(Guid ProcessStepId, Action<IProcessStep<ProcessStepTypeId>>? Initialize, Action<IProcessStep<ProcessStepTypeId>> Modify)>>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _portalRepositories.SaveAsync()).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task RetriggerDeleteCentralIdentityProvider_WithNotExistingProcess_ThrowsException()
    {
        // Arrange
        var stepToTrigger = ProcessStepTypeId.RETRIGGER_DELETE_CENTRAL_IDENTITY_PROVIDER;
        var processId = Guid.NewGuid();
        A.CallTo(() => _processStepRepository.IsValidProcess(A<Guid>._, A<ProcessTypeId>._, A<IEnumerable<ProcessStepTypeId>>._))
            .Returns((false, _fixture.Create<VerifyProcessData<ProcessTypeId, ProcessStepTypeId>>()));

        Task Act() => _logic.RetriggerDeleteCentralIdentityProvider(processId);

        // Act
        var ex = await Assert.ThrowsAsync<NotFoundException>(Act);

        // Assert
        ex.Message.Should().Be($"process {processId} does not exist");
        A.CallTo(() => _processStepRepository.IsValidProcess(processId, ProcessTypeId.IDENTITYPROVIDER_PROVISIONING, A<IEnumerable<ProcessStepTypeId>>.That.Matches(x => x.Count() == 1 && x.Single() == stepToTrigger)))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task RetriggerDeleteCentralUser_CallsExpected()
    {
        // Arrange
        var stepToTrigger = ProcessStepTypeId.RETRIGGER_DELETE_CENTRAL_USER;
        var processStepTypeId = ProcessStepTypeId.DELETE_CENTRAL_USER;
        var processSteps = new List<ProcessStep<Process, ProcessTypeId, ProcessStepTypeId>>();
        var process = _fixture.Build<Process>().With(x => x.LockExpiryDate, default(DateTimeOffset?)).Create();
        var processStepId = Guid.NewGuid();
        SetupFakesForRetrigger(processSteps);
        var verifyProcessData = new VerifyProcessData<ProcessTypeId, ProcessStepTypeId>(process, Enumerable.Repeat(new ProcessStep<Process, ProcessTypeId, ProcessStepTypeId>(processStepId, stepToTrigger, ProcessStepStatusId.TODO, process.Id, DateTimeOffset.UtcNow), 1));
        A.CallTo(() => _processStepRepository.IsValidProcess(A<Guid>._, A<ProcessTypeId>._, A<IEnumerable<ProcessStepTypeId>>._))
            .Returns((true, verifyProcessData));

        // Act
        await _logic.RetriggerDeleteCentralUser(process.Id);

        // Assert
        processSteps.Should().ContainSingle().And.Satisfy(x => x.ProcessStepTypeId == processStepTypeId);
        A.CallTo(() => _processStepRepository.IsValidProcess(process.Id, ProcessTypeId.USER_PROVISIONING, A<IEnumerable<ProcessStepTypeId>>.That.Matches(x => x.Count() == 1 && x.Single() == stepToTrigger)))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _processStepRepository.AttachAndModifyProcessSteps(A<IEnumerable<(Guid ProcessStepId, Action<IProcessStep<ProcessStepTypeId>>? Initialize, Action<IProcessStep<ProcessStepTypeId>> Modify)>>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _portalRepositories.SaveAsync()).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task RetriggerDeleteCentralUser_WithNotExistingProcess_ThrowsException()
    {
        // Arrange
        var stepToTrigger = ProcessStepTypeId.RETRIGGER_DELETE_CENTRAL_USER;
        var processId = Guid.NewGuid();
        A.CallTo(() => _processStepRepository.IsValidProcess(A<Guid>._, A<ProcessTypeId>._, A<IEnumerable<ProcessStepTypeId>>._))
            .Returns((false, _fixture.Create<VerifyProcessData<ProcessTypeId, ProcessStepTypeId>>()));

        Task Act() => _logic.RetriggerDeleteCentralUser(processId);

        // Act
        var ex = await Assert.ThrowsAsync<NotFoundException>(Act);

        // Assert
        ex.Message.Should().Be($"process {processId} does not exist");
        A.CallTo(() => _processStepRepository.IsValidProcess(processId, ProcessTypeId.USER_PROVISIONING, A<IEnumerable<ProcessStepTypeId>>.That.Matches(x => x.Count() == 1 && x.Single() == stepToTrigger)))
            .MustHaveHappenedOnceExactly();
    }

    #endregion

    #region Setup

    private void SetupForUpdateCompanyBpn(ApplicationChecklistEntry? applicationChecklistEntry = null)
    {
        if (applicationChecklistEntry != null)
        {
            A.CallTo(() => _checklistService.FinalizeChecklistEntryAndProcessSteps(A<IApplicationChecklistService.ManualChecklistProcessStepData>._, A<Action<ApplicationChecklistEntry>>._, A<Action<ApplicationChecklistEntry>>._, A<IEnumerable<ProcessStepTypeId>?>._))
                .Invokes((IApplicationChecklistService.ManualChecklistProcessStepData _, Action<ApplicationChecklistEntry> initial, Action<ApplicationChecklistEntry> action, IEnumerable<ProcessStepTypeId>? _) =>
                {
                    action.Invoke(applicationChecklistEntry);
                });
        }

        A.CallTo(() => _userRepository.GetBpnForIamUserUntrackedAsync(IdWithoutBpn, ValidBpn))
            .Returns(new (bool, bool, string?, Guid)[]
            {
                new (true, true, null, CompanyId)
            }.ToAsyncEnumerable());
        A.CallTo(() => _userRepository.GetBpnForIamUserUntrackedAsync(NotExistingApplicationId, ValidBpn))
            .Returns(new (bool, bool, string?, Guid)[]
            {
                new (false, true, ValidBpn, CompanyId)
            }.ToAsyncEnumerable());
        A.CallTo(() => _userRepository.GetBpnForIamUserUntrackedAsync(IdWithoutBpn, AlreadyTakenBpn))
            .Returns(new (bool, bool, string?, Guid)[]
            {
                new (true, true, ValidBpn, CompanyId),
                new (false, true, AlreadyTakenBpn, Guid.NewGuid())
            }.ToAsyncEnumerable());
        A.CallTo(() => _userRepository.GetBpnForIamUserUntrackedAsync(ActiveApplicationCompanyId, ValidBpn))
            .Returns(new (bool, bool, string?, Guid)[]
            {
                (true, false, ValidBpn, CompanyId)
            }.ToAsyncEnumerable());
        A.CallTo(() => _userRepository.GetBpnForIamUserUntrackedAsync(IdWithBpn, ValidBpn))
            .Returns(new (bool, bool, string?, Guid)[]
            {
                (true, true, ValidBpn, CompanyId)
            }.ToAsyncEnumerable());

        A.CallTo(() => _checklistService.VerifyChecklistEntryAndProcessSteps(IdWithoutBpn, ApplicationChecklistEntryTypeId.BUSINESS_PARTNER_NUMBER, A<IEnumerable<ApplicationChecklistEntryStatusId>>._, A<ProcessStepTypeId>._, A<IEnumerable<ApplicationChecklistEntryTypeId>?>._, A<IEnumerable<ProcessStepTypeId>?>._))
            .Returns(new IApplicationChecklistService.ManualChecklistProcessStepData(IdWithoutBpn, new Process(Guid.NewGuid(), ProcessTypeId.APPLICATION_CHECKLIST, Guid.NewGuid()), Guid.NewGuid(), ApplicationChecklistEntryTypeId.REGISTRATION_VERIFICATION,
            ImmutableDictionary.CreateRange(new[] { KeyValuePair.Create(ApplicationChecklistEntryTypeId.REGISTRATION_VERIFICATION, (ApplicationChecklistEntryStatusId.DONE, default(string?))) }),
            Enumerable.Empty<ProcessStep<Process, ProcessTypeId, ProcessStepTypeId>>()));
    }

    private void SetupForApproveRegistrationVerification(ApplicationChecklistEntry applicationChecklistEntry)
    {
        A.CallTo(() => _checklistService.FinalizeChecklistEntryAndProcessSteps(A<IApplicationChecklistService.ManualChecklistProcessStepData>._, A<Action<ApplicationChecklistEntry>>._, A<Action<ApplicationChecklistEntry>>._, A<IEnumerable<ProcessStepTypeId>?>._))
            .Invokes((IApplicationChecklistService.ManualChecklistProcessStepData _, Action<ApplicationChecklistEntry>? initial, Action<ApplicationChecklistEntry> action, IEnumerable<ProcessStepTypeId>? _) =>
            {
                action.Invoke(applicationChecklistEntry);
            });

        A.CallTo(() => _checklistService.VerifyChecklistEntryAndProcessSteps(IdWithoutBpn, ApplicationChecklistEntryTypeId.REGISTRATION_VERIFICATION, A<IEnumerable<ApplicationChecklistEntryStatusId>>._, A<ProcessStepTypeId>._, A<IEnumerable<ApplicationChecklistEntryTypeId>?>._, A<IEnumerable<ProcessStepTypeId>?>._))
            .Returns(new IApplicationChecklistService.ManualChecklistProcessStepData(IdWithoutBpn, new Process(Guid.NewGuid(), ProcessTypeId.APPLICATION_CHECKLIST, Guid.NewGuid()), Guid.NewGuid(), ApplicationChecklistEntryTypeId.REGISTRATION_VERIFICATION,
            ImmutableDictionary.CreateRange(new[] { KeyValuePair.Create(ApplicationChecklistEntryTypeId.BUSINESS_PARTNER_NUMBER, (ApplicationChecklistEntryStatusId.IN_PROGRESS, default(string?))) }),
            Enumerable.Empty<ProcessStep<Process, ProcessTypeId, ProcessStepTypeId>>()));
        A.CallTo(() => _checklistService.VerifyChecklistEntryAndProcessSteps(IdWithBpn, ApplicationChecklistEntryTypeId.REGISTRATION_VERIFICATION, A<IEnumerable<ApplicationChecklistEntryStatusId>>._, A<ProcessStepTypeId>._, A<IEnumerable<ApplicationChecklistEntryTypeId>?>._, A<IEnumerable<ProcessStepTypeId>?>._))
            .Returns(new IApplicationChecklistService.ManualChecklistProcessStepData(IdWithoutBpn, new Process(Guid.NewGuid(), ProcessTypeId.APPLICATION_CHECKLIST, Guid.NewGuid()), Guid.NewGuid(), ApplicationChecklistEntryTypeId.REGISTRATION_VERIFICATION,
            ImmutableDictionary.CreateRange(new[] { KeyValuePair.Create(ApplicationChecklistEntryTypeId.BUSINESS_PARTNER_NUMBER, (ApplicationChecklistEntryStatusId.DONE, default(string?))) }),
            Enumerable.Empty<ProcessStep<Process, ProcessTypeId, ProcessStepTypeId>>()));
    }

    private void SetupForDeclineRegistrationVerification(ApplicationChecklistEntry applicationChecklistEntry, CompanyApplication application, Company company, ApplicationChecklistEntryStatusId checklistStatusId, IdentityProviderTypeId idpTypeId)
    {
        A.CallTo(() => _checklistService.FinalizeChecklistEntryAndProcessSteps(A<IApplicationChecklistService.ManualChecklistProcessStepData>._, A<Action<ApplicationChecklistEntry>>._, A<Action<ApplicationChecklistEntry>>._, A<IEnumerable<ProcessStepTypeId>?>._))
            .Invokes((IApplicationChecklistService.ManualChecklistProcessStepData _, Action<ApplicationChecklistEntry>? _, Action<ApplicationChecklistEntry> action, IEnumerable<ProcessStepTypeId>? _) =>
            {
                action.Invoke(applicationChecklistEntry);
            });

        A.CallTo(() => _checklistService.VerifyChecklistEntryAndProcessSteps(IdWithoutBpn, ApplicationChecklistEntryTypeId.REGISTRATION_VERIFICATION, A<IEnumerable<ApplicationChecklistEntryStatusId>>._, A<ProcessStepTypeId>._, A<IEnumerable<ApplicationChecklistEntryTypeId>?>._, A<IEnumerable<ProcessStepTypeId>?>._))
            .Returns(new IApplicationChecklistService.ManualChecklistProcessStepData(IdWithoutBpn, new Process(Guid.NewGuid(), ProcessTypeId.APPLICATION_CHECKLIST, Guid.NewGuid()), Guid.NewGuid(), ApplicationChecklistEntryTypeId.REGISTRATION_VERIFICATION,
            ImmutableDictionary.CreateRange(new[] { KeyValuePair.Create(ApplicationChecklistEntryTypeId.REGISTRATION_VERIFICATION, (checklistStatusId, default(string?))) }),
            Enumerable.Empty<ProcessStep<Process, ProcessTypeId, ProcessStepTypeId>>()));
        A.CallTo(() => _checklistService.VerifyChecklistEntryAndProcessSteps(IdWithBpn, ApplicationChecklistEntryTypeId.REGISTRATION_VERIFICATION, A<IEnumerable<ApplicationChecklistEntryStatusId>>._, A<ProcessStepTypeId>._, A<IEnumerable<ApplicationChecklistEntryTypeId>?>._, A<IEnumerable<ProcessStepTypeId>?>._))
            .Returns(new IApplicationChecklistService.ManualChecklistProcessStepData(IdWithoutBpn, new Process(Guid.NewGuid(), ProcessTypeId.APPLICATION_CHECKLIST, Guid.NewGuid()), Guid.NewGuid(), ApplicationChecklistEntryTypeId.REGISTRATION_VERIFICATION,
            ImmutableDictionary.CreateRange(new[] { KeyValuePair.Create(ApplicationChecklistEntryTypeId.REGISTRATION_VERIFICATION, (ApplicationChecklistEntryStatusId.DONE, default(string?))) }),
            Enumerable.Empty<ProcessStep<Process, ProcessTypeId, ProcessStepTypeId>>()));

        A.CallTo(() => _applicationRepository.GetCompanyIdNameForSubmittedApplication(IdWithBpn))
            .Returns((CompanyId, CompanyName, ExistingExternalId, Enumerable.Repeat((IdpId, IamAliasId, idpTypeId, Enumerable.Repeat(UserId, 1)), 1), Enumerable.Repeat(UserId, 1)));

        A.CallTo(() => _provisioningManager.GetUserByUserName(UserId.ToString()))
            .Returns("user123");

        A.CallTo(() => _checklistService.FinalizeChecklistEntryAndProcessSteps(A<IApplicationChecklistService.ManualChecklistProcessStepData>._, A<Action<ApplicationChecklistEntry>>._, A<Action<ApplicationChecklistEntry>>._, A<IEnumerable<ProcessStepTypeId>?>._))
            .Invokes((IApplicationChecklistService.ManualChecklistProcessStepData _, Action<ApplicationChecklistEntry> _, Action<ApplicationChecklistEntry> action, IEnumerable<ProcessStepTypeId>? _) =>
            {
                action.Invoke(applicationChecklistEntry);
            });

        A.CallTo(() => _applicationRepository.AttachAndModifyCompanyApplication(A<Guid>._, A<Action<CompanyApplication>>._))
            .Invokes((Guid _, Action<CompanyApplication> modify) =>
            {
                modify.Invoke(application);
            });
        A.CallTo(() => _companyRepository.AttachAndModifyCompany(A<Guid>._, null, A<Action<Company>>._))
            .Invokes((Guid _, Action<Company>? _, Action<Company> modify) =>
            {
                modify.Invoke(company);
            });
    }

    private void SetupFakesForRetrigger(List<ProcessStep<Process, ProcessTypeId, ProcessStepTypeId>> processSteps)
    {
        A.CallTo(() => _processStepRepository.CreateProcessStepRange(A<IEnumerable<(ProcessStepTypeId ProcessStepTypeId, ProcessStepStatusId ProcessStepStatusId, Guid ProcessId)>>._))
            .Invokes((IEnumerable<(ProcessStepTypeId ProcessStepTypeId, ProcessStepStatusId ProcessStepStatusId, Guid ProcessId)> processStepTypeStatus) =>
                {
                    processSteps.AddRange(processStepTypeStatus.Select(x => new ProcessStep<Process, ProcessTypeId, ProcessStepTypeId>(Guid.NewGuid(), x.ProcessStepTypeId, x.ProcessStepStatusId, x.ProcessId, DateTimeOffset.UtcNow)).ToList());
                });
    }

    #endregion
}

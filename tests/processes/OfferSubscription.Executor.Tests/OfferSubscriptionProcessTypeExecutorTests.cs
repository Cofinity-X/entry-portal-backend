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
using Org.Eclipse.TractusX.Portal.Backend.Framework.ErrorHandling;
using Org.Eclipse.TractusX.Portal.Backend.Framework.Models.Configuration;
using Org.Eclipse.TractusX.Portal.Backend.Framework.Processes.Library.Enums;
using Org.Eclipse.TractusX.Portal.Backend.OfferProvider.Library.BusinessLogic;
using Org.Eclipse.TractusX.Portal.Backend.Offers.Library.Service;
using Org.Eclipse.TractusX.Portal.Backend.PortalBackend.DBAccess;
using Org.Eclipse.TractusX.Portal.Backend.PortalBackend.DBAccess.Repositories;
using Org.Eclipse.TractusX.Portal.Backend.PortalBackend.PortalEntities.Enums;
using Org.Eclipse.TractusX.Portal.Backend.Processes.OfferSubscription.Executor.DependencyInjection;

namespace Org.Eclipse.TractusX.Portal.Backend.Processes.OfferSubscription.Executor.Tests;

public class OfferSubscriptionProcessTypeExecutorTests
{
    private readonly Guid _processId = Guid.NewGuid();
    private readonly Guid _failingProcessId = Guid.NewGuid();
    private readonly Guid _subscriptionId = Guid.NewGuid();
    private readonly Guid _failingSubscriptionId = Guid.NewGuid();

    private readonly IPortalRepositories _portalRepositories;
    private readonly IOfferSubscriptionsRepository _offerSubscriptionRepository;
    private readonly IOfferProviderBusinessLogic _offerProviderBusinessLogic;
    private readonly IOfferSetupService _offerSetupService;
    private readonly IFixture _fixture;
    private readonly OfferSubscriptionsProcessSettings _settings;
    private readonly OfferSubscriptionProcessTypeExecutor _executor;

    public OfferSubscriptionProcessTypeExecutorTests()
    {
        _fixture = new Fixture().Customize(new AutoFakeItEasyCustomization { ConfigureMembers = true });
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _portalRepositories = A.Fake<IPortalRepositories>();
        _offerProviderBusinessLogic = A.Fake<IOfferProviderBusinessLogic>();
        _offerSetupService = A.Fake<IOfferSetupService>();

        _offerSubscriptionRepository = A.Fake<IOfferSubscriptionsRepository>();

        _settings = new OfferSubscriptionsProcessSettings
        {
            BasePortalAddress = "https://test.com",
            ItAdminRoles = Enumerable.Repeat(new UserRoleConfig("Portal", ["ItAdmin", "Admin"]), 1)
        };
        A.CallTo(() => _portalRepositories.GetInstance<IOfferSubscriptionsRepository>())
            .Returns(_offerSubscriptionRepository);

        _executor = new OfferSubscriptionProcessTypeExecutor(
            _offerProviderBusinessLogic,
            _offerSetupService,
            _portalRepositories,
            Options.Create(_settings));

        SetupFakes();
    }

    #region InitializeProcess

    [Fact]
    public async Task InitializeProcess_InvalidProcessId_Throws()
    {
        // Arrange
        var processId = Guid.NewGuid();

        async Task Act() => await _executor.InitializeProcess(processId, _fixture.CreateMany<ProcessStepTypeId>());

        // Act
        var ex = await Assert.ThrowsAsync<NotFoundException>(Act);

        // Assert
        ex.Message.Should().Be($"process {processId} does not exist or is not associated with an offer subscription");
    }

    [Fact]
    public async Task InitializeProcess_ValidProcessId_ReturnsExpected()
    {
        // Arrange
        var result = await _executor.InitializeProcess(_processId, _fixture.CreateMany<ProcessStepTypeId>());
        ;

        // Assert
        result.Modified.Should().BeFalse();
        result.ScheduleStepTypeIds.Should().BeNull();
    }

    #endregion

    #region ExecuteProcessStep

    [Fact]
    public async Task ExecuteProcessStep_InitializeNotCalled_Throws()
    {
        // Arrange
        var processStepTypeId = _fixture.Create<ProcessStepTypeId>();
        var processStepTypeIds = _fixture.CreateMany<ProcessStepTypeId>();

        var Act = async () => await _executor.ExecuteProcessStep(processStepTypeId, processStepTypeIds, CancellationToken.None);

        // Act
        var result = await Assert.ThrowsAsync<UnexpectedConditionException>(Act);

        // Assert
        result.Message.Should().Be("offerSubscriptionId should never be empty here");
    }

    [Fact]
    public async Task ExecuteProcessStep_WithUnrecoverableServiceException_Throws()
    {
        // Act initialize
        var initializationResult = await _executor.InitializeProcess(_failingProcessId, _fixture.CreateMany<ProcessStepTypeId>());

        // Assert initialize
        initializationResult.Should().NotBeNull();
        initializationResult.Modified.Should().BeFalse();
        initializationResult.ScheduleStepTypeIds.Should().BeNull();

        // Arrange
        const ProcessStepTypeId processStepTypeId = ProcessStepTypeId.TRIGGER_PROVIDER;
        var processStepTypeIds = _fixture.CreateMany<ProcessStepTypeId>();
        A.CallTo(() => _offerProviderBusinessLogic.TriggerProvider(_failingSubscriptionId, A<CancellationToken>._))
            .ThrowsAsync(new ServiceException("test"));

        // Act
        var result = await _executor.ExecuteProcessStep(processStepTypeId, processStepTypeIds, CancellationToken.None);

        // Assert
        result.Modified.Should().BeTrue();
        result.ProcessStepStatusId.Should().Be(ProcessStepStatusId.FAILED);
        result.ProcessMessage.Should().Be("test");
    }

    [Fact]
    public async Task ExecuteProcessStep_WithConflictException_Throws()
    {
        // Act initialize
        var initializationResult = await _executor.InitializeProcess(_failingProcessId, _fixture.CreateMany<ProcessStepTypeId>());

        // Assert initialize
        initializationResult.Should().NotBeNull();
        initializationResult.Modified.Should().BeFalse();
        initializationResult.ScheduleStepTypeIds.Should().BeNull();

        // Arrange
        const ProcessStepTypeId processStepTypeId = ProcessStepTypeId.TRIGGER_PROVIDER;
        var processStepTypeIds = _fixture.CreateMany<ProcessStepTypeId>();
        A.CallTo(() => _offerProviderBusinessLogic.TriggerProvider(_failingSubscriptionId, A<CancellationToken>._))
            .ThrowsAsync(new ConflictException("test"));

        // Act
        var result = await _executor.ExecuteProcessStep(processStepTypeId, processStepTypeIds, CancellationToken.None);

        // Assert
        result.Modified.Should().BeTrue();
        result.ProcessStepStatusId.Should().Be(ProcessStepStatusId.FAILED);
        result.ProcessMessage.Should().Be("test");
    }

    [Fact]
    public async Task ExecuteProcessStep_WithRecoverableServiceException_Throws()
    {
        // Act initialize
        var initializationResult = await _executor.InitializeProcess(_failingProcessId, _fixture.CreateMany<ProcessStepTypeId>());

        // Assert initialize
        initializationResult.Should().NotBeNull();
        initializationResult.Modified.Should().BeFalse();
        initializationResult.ScheduleStepTypeIds.Should().BeNull();

        // Arrange
        const ProcessStepTypeId processStepTypeId = ProcessStepTypeId.TRIGGER_PROVIDER;
        var processStepTypeIds = _fixture.CreateMany<ProcessStepTypeId>();
        A.CallTo(() => _offerProviderBusinessLogic.TriggerProvider(_failingSubscriptionId, A<CancellationToken>._))
            .ThrowsAsync(new ServiceException("test", true));

        // Act
        var result = await _executor.ExecuteProcessStep(processStepTypeId, processStepTypeIds, CancellationToken.None);

        // Assert
        result.Modified.Should().BeTrue();
        result.ProcessStepStatusId.Should().Be(ProcessStepStatusId.TODO);
    }

    [Theory]
    [InlineData(ProcessStepTypeId.TRIGGER_PROVIDER, ProcessStepTypeId.AWAIT_START_AUTOSETUP)]
    [InlineData(ProcessStepTypeId.OFFERSUBSCRIPTION_CLIENT_CREATION, ProcessStepTypeId.OFFERSUBSCRIPTION_TECHNICALUSER_CREATION)]
    [InlineData(ProcessStepTypeId.OFFERSUBSCRIPTION_TECHNICALUSER_CREATION, ProcessStepTypeId.ACTIVATE_SUBSCRIPTION)]
    [InlineData(ProcessStepTypeId.ACTIVATE_SUBSCRIPTION, ProcessStepTypeId.TRIGGER_PROVIDER_CALLBACK)]
    [InlineData(ProcessStepTypeId.TRIGGER_PROVIDER_CALLBACK, null)]
    public async Task ExecuteProcessStep_ValidSubscription_ReturnsExpected(ProcessStepTypeId processStepTypeId, ProcessStepTypeId? expectedResult)
    {
        // Act initialize
        var initializationResult = await _executor.InitializeProcess(_processId, _fixture.CreateMany<ProcessStepTypeId>());

        // Assert initialize
        initializationResult.Should().NotBeNull();
        initializationResult.Modified.Should().BeFalse();
        initializationResult.ScheduleStepTypeIds.Should().BeNull();

        // Arrange execute
        var executeProcessStepTypeIds = _fixture.CreateMany<ProcessStepTypeId>();

        // Act
        var result = await _executor.ExecuteProcessStep(processStepTypeId, executeProcessStepTypeIds, CancellationToken.None);

        // Assert
        result.Modified.Should().BeTrue();
        if (expectedResult == null)
        {
            result.ScheduleStepTypeIds.Should().BeNull();
        }
        else
        {
            result.ScheduleStepTypeIds.Should().ContainSingle().And.Satisfy(x => x == expectedResult);
        }
    }

    #endregion

    #region GetProcessTypeId

    [Fact]
    public void GetProcessTypeId_ReturnsExpected()
    {
        // Act
        var result = _executor.GetProcessTypeId();

        // Assert
        result.Should().Be(ProcessTypeId.OFFER_SUBSCRIPTION);
    }

    #endregion

    #region GetProcessTypeId

    [Fact]
    public async Task IsLockRequested_ReturnsExpected()
    {
        // Act
        var result = await _executor.IsLockRequested(_fixture.Create<ProcessStepTypeId>());

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region IsExecutableStepTypeId

    [Theory]
    [InlineData(ProcessStepTypeId.TRIGGER_PROVIDER, true)]
    [InlineData(ProcessStepTypeId.OFFERSUBSCRIPTION_CLIENT_CREATION, true)]
    [InlineData(ProcessStepTypeId.OFFERSUBSCRIPTION_TECHNICALUSER_CREATION, true)]
    [InlineData(ProcessStepTypeId.ACTIVATE_SUBSCRIPTION, true)]
    [InlineData(ProcessStepTypeId.TRIGGER_PROVIDER_CALLBACK, true)]
    [InlineData(ProcessStepTypeId.SINGLE_INSTANCE_SUBSCRIPTION_DETAILS_CREATION, false)]
    [InlineData(ProcessStepTypeId.AWAIT_START_AUTOSETUP, false)]
    [InlineData(ProcessStepTypeId.AWAIT_CLEARING_HOUSE_RESPONSE, false)]
    [InlineData(ProcessStepTypeId.START_CLEARING_HOUSE, false)]
    [InlineData(ProcessStepTypeId.MANUAL_DECLINE_APPLICATION, false)]
    [InlineData(ProcessStepTypeId.CREATE_IDENTITY_WALLET, false)]
    [InlineData(ProcessStepTypeId.MANUAL_TRIGGER_ACTIVATE_SUBSCRIPTION, false)]
    public void IsExecutableProcessStep_ReturnsExpected(ProcessStepTypeId processStepTypeId, bool expectedResult)
    {
        // Act
        var result = _executor.IsExecutableStepTypeId(processStepTypeId);

        // Assert
        result.Should().Be(expectedResult);
    }

    #endregion

    #region GetExecutableStepTypeIds

    [Fact]
    public void GetExecutableStepTypeIds_ReturnsExpected()
    {
        //Act
        var result = _executor.GetExecutableStepTypeIds();

        // Assert
        result.Should().HaveCount(6)
            .And.Satisfy(
                x => x == ProcessStepTypeId.TRIGGER_PROVIDER,
                x => x == ProcessStepTypeId.OFFERSUBSCRIPTION_CLIENT_CREATION,
                x => x == ProcessStepTypeId.OFFERSUBSCRIPTION_TECHNICALUSER_CREATION,
                x => x == ProcessStepTypeId.OFFERSUBSCRIPTION_CREATE_DIM_TECHNICAL_USER,
                x => x == ProcessStepTypeId.ACTIVATE_SUBSCRIPTION,
                x => x == ProcessStepTypeId.TRIGGER_PROVIDER_CALLBACK
            );
    }

    #endregion

    #region Setup

    private void SetupFakes()
    {
        A.CallTo(() => _offerSubscriptionRepository.GetOfferSubscriptionDataForProcessIdAsync(_failingProcessId))
            .Returns(_failingSubscriptionId);
        A.CallTo(() => _offerSubscriptionRepository.GetOfferSubscriptionDataForProcessIdAsync(_processId))
            .Returns(_subscriptionId);
        A.CallTo(() => _offerSubscriptionRepository.GetOfferSubscriptionDataForProcessIdAsync(A<Guid>.That.Not.Matches(x => x == _processId || x == _failingProcessId)))
            .Returns(Guid.Empty);

        A.CallTo(() => _offerProviderBusinessLogic.TriggerProvider(_subscriptionId, A<CancellationToken>._))
            .Returns(([ProcessStepTypeId.AWAIT_START_AUTOSETUP], ProcessStepStatusId.DONE, true, null));
        A.CallTo(() => _offerSetupService.CreateClient(_subscriptionId))
            .Returns(([ProcessStepTypeId.OFFERSUBSCRIPTION_TECHNICALUSER_CREATION], ProcessStepStatusId.DONE, true, null));
        A.CallTo(() => _offerSetupService.CreateTechnicalUser(_processId, _subscriptionId, A<IEnumerable<UserRoleConfig>>._))
            .Returns(([ProcessStepTypeId.ACTIVATE_SUBSCRIPTION], ProcessStepStatusId.DONE, true, null));
        A.CallTo(() => _offerSetupService.ActivateSubscription(_subscriptionId, A<IEnumerable<UserRoleConfig>>._, A<IEnumerable<UserRoleConfig>>._, A<string>._))
            .Returns(([ProcessStepTypeId.TRIGGER_PROVIDER_CALLBACK], ProcessStepStatusId.DONE, true, null));
        A.CallTo(() => _offerProviderBusinessLogic.TriggerProviderCallback(_subscriptionId, A<CancellationToken>._))
            .Returns((null, ProcessStepStatusId.DONE, true, null));
    }

    #endregion
}

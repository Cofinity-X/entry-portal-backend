/********************************************************************************
 * Copyright (c) 2022 BMW Group AG
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

using Org.Eclipse.TractusX.Portal.Backend.Processes.ApplicationChecklist.Library;

namespace Org.Eclipse.TractusX.Portal.Backend.ApplicationActivation.Library;

public interface IApplicationActivationService
{
    /// <summary>
    /// Handles the application activation
    /// </summary>
    /// <param name="context">The context for the application activation</param>
    /// <param name="cancellationToken">The cancellation Token</param>
    Task<IApplicationChecklistService.WorkerChecklistProcessStepExecutionResult> StartApplicationActivation(IApplicationChecklistService.WorkerChecklistProcessStepData context, CancellationToken cancellationToken);
    Task<IApplicationChecklistService.WorkerChecklistProcessStepExecutionResult> AssignRoles(IApplicationChecklistService.WorkerChecklistProcessStepData context, CancellationToken cancellationToken);
    Task<IApplicationChecklistService.WorkerChecklistProcessStepExecutionResult> AssignBpn(IApplicationChecklistService.WorkerChecklistProcessStepData context, CancellationToken cancellationToken);
    Task<IApplicationChecklistService.WorkerChecklistProcessStepExecutionResult> RemoveRegistrationRoles(IApplicationChecklistService.WorkerChecklistProcessStepData context, CancellationToken cancellationToken);
    Task<IApplicationChecklistService.WorkerChecklistProcessStepExecutionResult> SetTheme(IApplicationChecklistService.WorkerChecklistProcessStepData context, CancellationToken cancellationToken);
    Task<IApplicationChecklistService.WorkerChecklistProcessStepExecutionResult> SetMembership(IApplicationChecklistService.WorkerChecklistProcessStepData context, CancellationToken cancellationToken);
    Task<IApplicationChecklistService.WorkerChecklistProcessStepExecutionResult> SetCxMembership(IApplicationChecklistService.WorkerChecklistProcessStepData context, CancellationToken cancellationToken);
    Task<IApplicationChecklistService.WorkerChecklistProcessStepExecutionResult> SaveApplicationActivationToDatabase(IApplicationChecklistService.WorkerChecklistProcessStepData context, CancellationToken cancellationToken);
}

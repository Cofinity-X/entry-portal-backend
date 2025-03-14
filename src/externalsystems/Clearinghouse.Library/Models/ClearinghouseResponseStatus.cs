/********************************************************************************
 * Copyright (c) 2022 Microsoft and BMW Group AG
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

namespace Org.Eclipse.TractusX.Portal.Backend.Clearinghouse.Library.Models;

public enum ClearinghouseResponseStatus
{
    /// <summary>
    /// In case the identifier has been found in the trust sources of clearing house.
    /// </summary>
    VALID = 1,

    /// <summary>
    /// In case the identifier format is not valid or the identifier was not found in the trust source of clearing house.
    /// </summary>
    INVALID = 2,

    /// <summary>
    /// In case the validation can't be performed, due to the unavailablity of the trust source of clearing house.
    /// </summary>
    INCONCLUSIVE = 3
}

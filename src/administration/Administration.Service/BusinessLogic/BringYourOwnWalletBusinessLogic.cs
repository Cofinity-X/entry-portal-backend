using Microsoft.Extensions.Options;
using Org.Eclipse.TractusX.Portal.Backend.Dim.Library;
using Org.Eclipse.TractusX.Portal.Backend.Dim.Library.DependencyInjection;
using Org.Eclipse.TractusX.Portal.Backend.Framework.DateTimeProvider;
using Org.Eclipse.TractusX.Portal.Backend.Framework.ErrorHandling;
using Org.Eclipse.TractusX.Portal.Backend.PortalBackend.DBAccess;
using Org.Eclipse.TractusX.Portal.Backend.PortalBackend.DBAccess.Repositories;
using Org.Eclipse.TractusX.Portal.Backend.PortalBackend.PortalEntities.Enums;
using Org.Eclipse.TractusX.Portal.Backend.Processes.ApplicationChecklist.Library;
using System.Runtime;

namespace Org.Eclipse.TractusX.Portal.Backend.Administration.Service.BusinessLogic
{
    public sealed class BringYourOwnWalletBusinessLogic : IBringYourOwnWalletBusinessLogic
    {
        private readonly IDimService _dimService;

        public BringYourOwnWalletBusinessLogic(IDimService dimService)
        {
            _dimService = dimService;
        }
        public async Task<bool> ValidateDid(string did, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(did))
            {
                throw new ConflictException("There must be a did set");
            }
            return (await _dimService.ValidateDid(did, cancellationToken));
        }
    }
}

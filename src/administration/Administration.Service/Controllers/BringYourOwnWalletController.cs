using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.Eclipse.TractusX.Portal.Backend.Administration.Service.BusinessLogic;
using Org.Eclipse.TractusX.Portal.Backend.Administration.Service.Models;
using Org.Eclipse.TractusX.Portal.Backend.Framework.ErrorHandling.Web;
using Org.Eclipse.TractusX.Portal.Backend.Framework.Web;

namespace Org.Eclipse.TractusX.Portal.Backend.Administration.Service.Controllers
{
    [ApiController]
    [EnvironmentRoute("MVC_ROUTING_BASEPATH", "bringyourownwallet")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class BringYourOwnWalletController(IBringYourOwnWalletBusinessLogic logic) : ControllerBase
    {
        [HttpGet]
        [Authorize(Roles = "invite_user")]
        [Route("validateDID")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public Task<bool> GetCompanyWithAddressAsync([FromRoute] string did, CancellationToken cancellationToken) =>
        logic.ValidateDid(did, cancellationToken);
    }
}

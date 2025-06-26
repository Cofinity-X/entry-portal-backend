namespace Org.Eclipse.TractusX.Portal.Backend.Administration.Service.BusinessLogic
{
    public interface IBringYourOwnWalletBusinessLogic
    {
        Task<bool> ValidateDid(string did, CancellationToken cancellationToken);
    }
}

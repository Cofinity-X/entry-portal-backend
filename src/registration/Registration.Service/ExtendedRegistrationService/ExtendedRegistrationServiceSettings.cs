using Org.Eclipse.TractusX.Portal.Backend.Framework.Token;

namespace Org.Eclipse.TractusX.Portal.Backend.ExtendedRegistration;

public class ExtendedRegistrationServiceSettings : KeyVaultAuthSettings
{
    public string BaseAddress { get; set; } = string.Empty;
}

using System.Text.Json.Serialization;

namespace Org.Eclipse.TractusX.Portal.Backend.ExtendedRegistration.Service.Model;

public class HubspotCompanyResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("isCatenaXCustomer")]
    public bool IsCatenaXCustomer { get; set; }

    [JsonPropertyName("properties")]
    public Dictionary<string, string>? Properties { get; set; }
}

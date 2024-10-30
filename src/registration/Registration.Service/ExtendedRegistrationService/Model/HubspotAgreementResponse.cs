using System.Text.Json.Serialization;

namespace Org.Eclipse.TractusX.Portal.Backend.ExtendedRegistration.Service.Model;

public class HubspotAgreementResponse
{
    [JsonPropertyName("agreement_name")]
    public string? AgreementName { get; set; }

    [JsonPropertyName("hs_object_id")]
    public string? HsObjectId { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

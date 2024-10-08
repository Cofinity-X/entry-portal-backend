using System.Text.Json.Serialization;

namespace Org.Eclipse.TractusX.Portal.Backend.ExtendedRegistration.Service.Model;

public record HubspotInboundRequest(HubspotInboundCompany? Company, HubspotContact? Contact);
public record HubspotOutboundRequest(HubspotOutboundCompany? Company, HubspotContact? Contact);

public class HubspotInboundCompany
{
    [JsonPropertyName("properties")]
    public Dictionary<string, string>? Properties { get; set; }

    [JsonPropertyName("agreements")]
    public List<string>? Agreements { get; set; }
}

public class HubspotOutboundCompany : HubspotInboundCompany
{

    [JsonPropertyName("portal_company_id")]
    public string? PortalCompanyId { get; set; }
}

public class HubspotContact
{
    [JsonPropertyName("email")]
    public required string Email { get; set; }

    [JsonPropertyName("properties")]
    public Dictionary<string, string>? Properties { get; set; }
}

public class HubspotCompanyUpdateResponse
{
    [JsonPropertyName("companyId")]
    public string? CompanyId { get; set; }

    [JsonPropertyName("contactId")]
    public string? ContactId { get; set; }
}

